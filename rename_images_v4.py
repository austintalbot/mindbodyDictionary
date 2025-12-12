import json
import os
import shutil
import re

conditions_path = 'MindBodyDictionaryMobile/Resources/Raw/conditionData.json'
images_dir = 'MindBodyDictionaryMobile/Resources/Raw/images'

with open(conditions_path, 'r') as f:
    conditions = json.load(f)

def get_files_map():
    files = os.listdir(images_dir)
    return {f.lower(): f for f in files}, set(files)

files_lower, files_set = get_files_map()

# Special mappings
special_map = {
    "blood pressure (low)": "LowBloodPressure",
    "stomach problems (digestive)": "Stomach problems (digestive)",
}

for c in conditions:
    name = c['name']
    # The app logic sanitization:
    safe_name = name.replace(':', '-').replace('/', '-')

    # We want files to be exactly: safe_name-Negative.png

    suffix_map = {1: "-Negative.png", 2: "-Positive.png"}

    for i, suffix in suffix_map.items():
        target_name = f"{safe_name}{suffix}"

        if target_name in files_set:
            continue

        found_src = None
        candidates = []

        # 1. Existing bad rename?
        candidates.append(f"{safe_name}{i}.png")

        # 2. Original special/first word
        if name.lower() in special_map:
            candidates.append(f"{special_map[name.lower()]}{i}.png")
            # Also check if special map got renamed with suffix
            candidates.append(f"{special_map[name.lower()]}{suffix}")

        # 3. First word split logic
        parts = re.split(r'[ _-]', name)
        if parts:
            first_word = parts[0].replace(':', '').replace('/', '')
            candidates.append(f"{first_word}{i}.png")
            candidates.append(f"{first_word}{suffix}")

            if len(parts) > 1:
                 combined = f"{parts[0]}{parts[1]}"
                 candidates.append(f"{combined}{i}.png")
                 candidates.append(f"{combined}{suffix}")

        compressed = "".join(x for x in name if x.isalnum())
        candidates.append(f"{compressed}{i}.png")
        candidates.append(f"{compressed}{suffix}")

        candidates.append(f"{name}{i}.png")
        candidates.append(f"{name}{suffix}")

        # Also check for files that are just "ConditionName-Negative.png"
        # but ConditionName might have slight variation in spaces/case

        for cand in candidates:
            if cand in files_set:
                found_src = cand
                break
            if cand.lower() in files_lower:
                found_src = files_lower[cand.lower()]
                break

        if found_src:
            if found_src != target_name:
                src_path = os.path.join(images_dir, found_src)
                dst_path = os.path.join(images_dir, target_name)
                print(f"Renaming '{found_src}' -> '{target_name}'")
                shutil.move(src_path, dst_path)
                files_lower, files_set = get_files_map()
        else:
            # Check if maybe the file exists with spaces when safe_name has dashes?
            pass
            # print(f"Missing: {target_name}")
