import json
import os
import shutil

conditions_path = 'MindBodyDictionaryMobile/Resources/Raw/conditionData.json'
images_dir = 'MindBodyDictionaryMobile/Resources/Raw/images'

with open(conditions_path, 'r') as f:
    conditions = json.load(f)

# Refresh file list helper
def get_files_map():
    files = os.listdir(images_dir)
    return {f.lower(): f for f in files}, set(files)

files_lower, files_set = get_files_map()

# Special mappings based on previous observation
special_map = {
    "blood pressure (low)": "LowBloodPressure",
    "stomach problems (digestive)": "Stomach problems (digestive)",
}

for c in conditions:
    name = c['name']
    # Target name base
    safe_name = name.replace(':', '-').replace('/', '-')

    # Mapping 1 -> Negative, 2 -> Positive
    suffix_map = {1: "-Negative.png", 2: "-Positive.png"}

    for i, suffix in suffix_map.items():
        target_name = f"{safe_name}{suffix}"

        # If target already exists, skip
        if target_name in files_set:
            continue

        found_src = None

        # Candidates to look for (source files still have old naming or intermediate naming):
        candidates = []

        # 1. Intermediate rename state (safe_name + number + .png) - e.g. "Lung Problems1.png"
        candidates.append(f"{safe_name}{i}.png")

        # 2. Original/Other potential states
        if name.lower() in special_map:
            candidates.append(f"{special_map[name.lower()]}{i}.png")

        first_word = name.split(' ')[0].replace(':', '').replace('/', '')
        candidates.append(f"{first_word}{i}.png")

        compressed = "".join(x for x in name if x.isalnum())
        candidates.append(f"{compressed}{i}.png")

        candidates.append(f"{name}{i}.png")

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
            print(f"Could not find source image for '{target_name}'")
