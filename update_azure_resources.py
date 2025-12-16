import json
import os
import time
import sys
import re
import argparse
from azure.cosmos import CosmosClient, PartitionKey
from azure.storage.blob import BlobServiceClient, PublicAccess

# --- Configuration ---
# User must provide these via environment variables or edit the script
COSMOS_CONNECTION_STRING = os.environ.get("COSMOS_CONNECTION_STRING")
STORAGE_CONNECTION_STRING = os.environ.get("STORAGE_CONNECTION_STRING")

DATABASE_NAME = "MindBodyDictionary"
COSMOS_CONTAINER_NAME = "MbdConditions"

SOURCE_BLOB_CONTAINER_NAME = "mbd-images"
# Azure container names must be lowercase
DEST_BLOB_CONTAINER_NAME = "mbdconditionimages"

JSON_PATH = 'MindBodyDictionaryMobile/Resources/Raw/conditionData.json'

def get_cosmos_container():
    if not COSMOS_CONNECTION_STRING:
        print("Error: COSMOS_CONNECTION_STRING environment variable is not set.")
        sys.exit(1)
    client = CosmosClient.from_connection_string(COSMOS_CONNECTION_STRING)
    database = client.get_database_client(DATABASE_NAME)
    return database.get_container_client(COSMOS_CONTAINER_NAME)

def get_blob_service_client():
    if not STORAGE_CONNECTION_STRING:
        print("Error: STORAGE_CONNECTION_STRING environment variable is not set.")
        sys.exit(1)
    return BlobServiceClient.from_connection_string(STORAGE_CONNECTION_STRING)

def normalize_name_for_search(name):
    # Same logic as previous scripts to find "old" blobs
    # Remove extension
    base = os.path.splitext(name)[0]
    # Replace separators with space and lowercase
    s = re.sub(r"[,\s_()-]+", " ", base).strip().lower()
    return s

def main():
    parser = argparse.ArgumentParser(description="Update image names in Cosmos DB and move/rename images to new Azure Storage container.")
    parser.add_argument('--dry-run', action='store_true', help="Run without making any actual changes.")
    args = parser.parse_args()

    dry_run = args.dry_run

    if dry_run:
        print("--- DRY RUN MODE --- No changes will be made to Azure resources.")

    print(f"Configuration:")
    print(f"  Source Container: {SOURCE_BLOB_CONTAINER_NAME}")
    print(f"  Dest Container:   {DEST_BLOB_CONTAINER_NAME}")
    print("-" * 30)

    print("Loading condition data...")
    with open(JSON_PATH, 'r') as f:
        conditions = json.load(f)

    print(f"Loaded {len(conditions)} conditions.")

    print("Connecting to Azure services...")
    try:
        cosmos_container = get_cosmos_container()
        blob_service_client = get_blob_service_client()

        source_container_client = blob_service_client.get_container_client(SOURCE_BLOB_CONTAINER_NAME)
        dest_container_client = blob_service_client.get_container_client(DEST_BLOB_CONTAINER_NAME)

        print("Connected successfully.")
    except Exception as e:
        print(f"Connection failed: {e}")
        sys.exit(1)

    # Ensure destination container exists
    try:
        if not dest_container_client.exists():
            print(f"Destination container '{DEST_BLOB_CONTAINER_NAME}' does not exist.")
            if dry_run:
                print(f"  [Blob DRY RUN] Would create container '{DEST_BLOB_CONTAINER_NAME}' with public blob access.")
            else:
                print(f"Creating container '{DEST_BLOB_CONTAINER_NAME}'...")
                dest_container_client.create_container(public_access=PublicAccess.Blob)
    except Exception as e:
        print(f"Error checking/creating destination container: {e}")
        if not dry_run:
            sys.exit(1)

    # Pre-fetch all existing blobs to avoid listing thousands of times
    print("Listing existing blobs in SOURCE container (this may take a moment)...")
    try:
        source_blobs = {b.name: b.name for b in source_container_client.list_blobs()}
    except Exception as e:
        print(f"Error listing source blobs: {e}")
        source_blobs = {}

    print("Listing existing blobs in DESTINATION container...")
    try:
        if dest_container_client.exists():
            dest_blobs = {b.name: b.name for b in dest_container_client.list_blobs()}
        else:
            dest_blobs = {}
    except Exception as e:
        print(f"Error listing dest blobs (container might not exist yet): {e}")
        dest_blobs = {}

    # Create a normalized map for fuzzy finding in SOURCE
    # map: normalized_name -> actual_blob_name
    normalized_source_blob_map = {}
    for b_name in source_blobs:
        normalized_source_blob_map[normalize_name_for_search(b_name)] = b_name

    updated_cosmos_count = 0
    moved_blobs_count = 0

    for i, condition in enumerate(conditions):
        condition_id = condition.get('id')
        name = condition.get('name')

        target_neg_img = condition.get('imageNegative')
        target_pos_img = condition.get('imagePositive')

        if not condition_id or not name:
            # print(f"Skipping invalid condition at index {i}")
            continue

        # print(f"Processing '{name}' ({condition_id})...")

        # 1. Update Cosmos DB
        try:
            # We try to read using id as partition key first
            item = cosmos_container.read_item(item=condition_id, partition_key=condition_id)

            # Check if update is needed
            current_neg = item.get('imageNegative')
            current_pos = item.get('imagePositive')

            if current_neg != target_neg_img or current_pos != target_pos_img:
                if dry_run:
                    print(f"  [Cosmos DRY RUN] Would update image paths for '{name}'.")
                else:
                    item['imageNegative'] = target_neg_img
                    item['imagePositive'] = target_pos_img
                    cosmos_container.replace_item(item=condition_id, body=item)
                    print(f"  [Cosmos] Updated image paths for '{name}'.")
                updated_cosmos_count += 1
            # else:
                # print(f"  [Cosmos] Already up to date.")

        except Exception as e:
            print(f"  [Cosmos] Error updating item '{name}': {e}")


        # 2. Update Blob Storage (Move/Rename to New Container)

        def ensure_blob_moved(target_name, suffix_search):
            nonlocal moved_blobs_count
            if not target_name:
                return

            # Check if already in DEST
            if target_name in dest_blobs:
                # print(f"  [Blob] {target_name} already in destination.")
                return

            # Need to find the source blob in SOURCE container
            normalized_base = normalize_name_for_search(name)

            candidates = []

            # Add existing file name format (local image folder format)
            safe_name_hyphen = name.replace(':', '-').replace('/', '-').replace(' ', '-').replace("'s", "")
            if suffix_search == 1:
                candidates.append(f"{safe_name_hyphen}-Negative.png")
            else:
                candidates.append(f"{safe_name_hyphen}-Positive.png")

            # Fuzzy matches
            if suffix_search == 1:
                candidates.append(f"{normalized_base} 1")
                candidates.append(f"{normalized_base} negative")
                candidates.append(f"{normalized_base}1")
            else:
                candidates.append(f"{normalized_base} 2")
                candidates.append(f"{normalized_base} positive")
                candidates.append(f"{normalized_base}2")

            source_blob_name = None
            for cand_raw in candidates:
                # Check exact match in source
                if cand_raw in source_blobs:
                    source_blob_name = cand_raw
                    break
                # Check normalized match in source
                normalized_cand = normalize_name_for_search(cand_raw)
                if normalized_cand in normalized_source_blob_map:
                    source_blob_name = normalized_source_blob_map[normalized_cand]
                    break

            if source_blob_name:
                if dry_run:
                    print(f"  [Blob DRY RUN] Would COPY '{source_blob_name}' (from {SOURCE_BLOB_CONTAINER_NAME}) -> '{target_name}' (in {DEST_BLOB_CONTAINER_NAME})")
                else:
                    print(f"  [Blob] Moving '{source_blob_name}' -> '{DEST_BLOB_CONTAINER_NAME}/{target_name}'")
                    try:
                        # Get clients
                        source_blob_client = source_container_client.get_blob_client(source_blob_name)
                        dest_blob_client = dest_container_client.get_blob_client(target_name)

                        # Start Copy
                        dest_blob_client.start_copy_from_url(source_blob_client.url)

                        # Wait for copy
                        props = dest_blob_client.get_blob_properties()
                        start_time = time.time()
                        while props.copy.status == 'pending':
                            if time.time() - start_time > 30: # timeout
                                print("    Copy timed out.")
                                return
                            time.sleep(0.2)
                            props = dest_blob_client.get_blob_properties()

                        if props.copy.status != 'success':
                            print(f"    Copy failed status: {props.copy.status}")
                            return

                        # Delete from source
                        source_blob_client.delete_blob()

                        # Update local cache
                        dest_blobs[target_name] = target_name
                        if source_blob_name in source_blobs:
                            del source_blobs[source_blob_name]

                        moved_blobs_count += 1

                    except Exception as e:
                        print(f"    Error moving blob: {e}")
            else:
                print(f"  [Blob] Warning: Source blob not found for '{target_name}' (Condition: {name})")

        ensure_blob_moved(target_neg_img, 1)
        ensure_blob_moved(target_pos_img, 2)

    print("-" * 30)
    print(f"Completed. Cosmos updates: {updated_cosmos_count}, Blobs moved: {moved_blobs_count}")

if __name__ == "__main__":
    main()
