import json
import os
import re
import shutil

# Paths
json_file_path = 'MindBodyDictionaryMobile/Resources/Raw/conditionData.json'
downloaded_images_dir = '/Users/austintalbot/.gemini/tmp/047329e71b7c4de3a480ad3272b15625bb8ca1b477a4861c1b2144f616851ed5/azure_images'

# Load condition data
with open(json_file_path, 'r') as f:
    conditions_data = json.load(f)

# Helper function to generate a normalized, searchable base string from a filename or condition name
def create_searchable_base_name(input_name):
    if input_name is None:
        return ""
    # Remove file extension if present
    name_without_ext = os.path.splitext(input_name)[0]
    # Replace common separators and special characters with a single space, then lowercase
    searchable_name = re.sub(r"[,\s_()-]+", " ", name_without_ext).strip()
    searchable_name = re.sub(r"'", "", searchable_name) # Remove apostrophes
    searchable_name = re.sub(r'%2A', '', searchable_name) # Remove %2A (encoded asterisk)
    searchable_name = re.sub(r'\s+', ' ', searchable_name) # Replace multiple spaces with single space
    return searchable_name.lower()

# Build a map of normalized downloaded Azure filenames to their full original name
# This will be used to find the source file to rename
azure_searchable_to_original_full_name = {}
for downloaded_filename in os.listdir(downloaded_images_dir):
    base_name, ext = os.path.splitext(downloaded_filename)
    # Handle the '1' and '2' suffixes from Azure blobs
    if base_name.endswith('1'):
        normalized_base_name = create_searchable_base_name(base_name[:-1]) # Remove '1'
        azure_searchable_to_original_full_name[(normalized_base_name, '1')] = downloaded_filename
    elif base_name.endswith('2'):
        normalized_base_name = create_searchable_base_name(base_name[:-1]) # Remove '2'
        azure_searchable_to_original_full_name[(normalized_base_name, '2')] = downloaded_filename
    else: # For other files not ending in 1 or 2, like MBDIcon.png
        normalized_base_name = create_searchable_base_name(base_name)
        azure_searchable_to_original_full_name[(normalized_base_name, 'other')] = downloaded_filename

rename_operations = []
for condition in conditions_data:
    condition_display_name = condition.get('name')
    if not condition_display_name:
        continue

    # Clean the display name to match the base of expected Azure filenames (before 1/2 suffix)
    normalized_display_name_for_search = create_searchable_base_name(condition_display_name)

    expected_negative_image_from_json = condition.get('imageNegative')
    expected_positive_image_from_json = condition.get('imagePositive')
    
    # Handle cases where imageNegative/Positive might be null or need regeneration based on condition_display_name
    if not expected_negative_image_from_json:
        # Construct expected filename from condition_display_name in the app's standard format
        clean_name_for_expected = re.sub(r"[,'()]", "", condition_display_name)
        clean_name_for_expected = clean_name_for_expected.replace(" - ", "-").replace(" / ", "-")
        clean_name_for_expected = clean_name_for_expected.replace(" ", "-")
        clean_name_for_expected = re.sub(r'-+', '-', clean_name_for_expected).strip('-')
        expected_negative_image_from_json = f"{clean_name_for_expected}-Negative.png"

    if not expected_positive_image_from_json:
        # Construct expected filename from condition_display_name in the app's standard format
        clean_name_for_expected = re.sub(r"[,'()]", "", condition_display_name)
        clean_name_for_expected = clean_name_for_expected.replace(" - ", "-").replace(" / ", "-")
        clean_name_for_expected = clean_name_for_expected.replace(" ", "-")
        clean_name_for_expected = re.sub(r'-+', '-', clean_name_for_expected).strip('-')
        expected_positive_image_from_json = f"{clean_name_for_expected}-Positive.png"

    # Find the Azure source file for the negative image
    # Try to match the normalized display name with '1' suffix for negative
    original_azure_negative_filename = azure_searchable_to_original_full_name.get((normalized_display_name_for_search, '1'))
    
    if original_azure_negative_filename:
        if original_azure_negative_filename != expected_negative_image_from_json:
            rename_operations.append((
                os.path.join(downloaded_images_dir, original_azure_negative_filename),
                os.path.join(downloaded_images_dir, expected_negative_image_from_json)
            ))
    else:
        print(f"Warning: No Azure blob found for condition '{condition_display_name}' (expected negative, suffix '1')")

    # Find the Azure source file for the positive image
    # Try to match the normalized display name with '2' suffix for positive
    original_azure_positive_filename = azure_searchable_to_original_full_name.get((normalized_display_name_for_search, '2'))
    
    if original_azure_positive_filename:
        if original_azure_positive_filename != expected_positive_image_from_json:
            rename_operations.append((
                os.path.join(downloaded_images_dir, original_azure_positive_filename),
                os.path.join(downloaded_images_dir, expected_positive_image_from_json)
            ))
    else:
        print(f"Warning: No Azure blob found for condition '{condition_display_name}' (expected positive, suffix '2')")

# Execute renames
executed_renames = 0
for old_path, new_path in rename_operations:
    if os.path.exists(old_path) and not os.path.exists(new_path):
        shutil.move(old_path, new_path)
        print(f"Renamed: '{os.path.basename(old_path)}' to '{os.path.basename(new_path)}'")
        executed_renames += 1
    elif os.path.exists(new_path):
        print(f"Skipping rename: Target file '{os.path.basename(new_path)}' already exists.")
    else:
        print(f"Skipping rename: Source file '{os.path.basename(old_path)}' not found. This may indicate a problem with normalization or missing file.")

print(f"Finished renaming. Total files renamed: {executed_renames}")
