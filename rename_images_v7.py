import json
import os
import shutil
import re

json_path = 'MindBodyDictionaryMobile/Resources/Raw/conditionData.json'
images_dir = 'MindBodyDictionaryMobile/Resources/Raw/images'

with open(json_path, 'r') as f:
    conditions = json.load(f)

files = set(os.listdir(images_dir))
files_lower = {f.lower(): f for f in files}

def to_camel_case(text):
    # Remove special chars and split by spaces/hyphens
    # Keep alphanumeric
    # 1. Replace special chars with space
    text = re.sub(r'[^a-zA-Z0-9\s]', ' ', text)
    words = text.split()
    if not words:
        return ""

    # 2. Lowercase first word, capitalize subsequent words
    res = [words[0].lower()]
    for w in words[1:]:
        res.append(w.capitalize())

    return "".join(res)

updates = 0

for c in conditions:
    name = c['name']

    # Generate camelCase base name
    camel_name = to_camel_case(name)

    # Expected names
    target_neg = f"{camel_name}Negative.png"
    target_pos = f"{camel_name}Positive.png"

    # Update JSON
    c['imageNegative'] = target_neg
    c['imagePositive'] = target_pos

    # Check Negative
    if target_neg not in files:
        # Try to find candidate from previous iterations or loose matches
        candidates = []

        # Previous convention: Lung-Problems-Negative.png
        safe_name_hyphen = name.replace(':', '-').replace('/', '-').replace(' ', '-').replace("'s", "")
        candidates.append(f"{safe_name_hyphen}-Negative.png")

        # "1" suffix: Lung-Problems1.png
        candidates.append(f"{safe_name_hyphen}1.png")
        candidates.append(f"{name}1.png")

        # Underscores
        safe_name_underscore = safe_name_hyphen.replace('-', '_')
        candidates.append(f"{safe_name_underscore}1.png")

        # "Swelling-_-Edema" type weirdness
        weird_name = name.replace(':', '-').replace('/', '-').replace(' ', '-_-').replace("'s", "")
        candidates.append(f"{weird_name}1.png")
        candidates.append(f"{weird_name}-Negative.png")

        found = None
        for cand in candidates:
             if cand in files:
                 found = cand
                 break
             if cand.lower() in files_lower:
                 found = files_lower[cand.lower()]
                 break

        if found and found != target_neg:
            print(f"Renaming {found} -> {target_neg}")
            shutil.move(os.path.join(images_dir, found), os.path.join(images_dir, target_neg))
            files.add(target_neg)
            files.remove(found)
            if found.lower() in files_lower:
                del files_lower[found.lower()]
            files_lower[target_neg.lower()] = target_neg
            updates += 1

    # Check Positive
    if target_pos not in files:
        candidates = []

        safe_name_hyphen = name.replace(':', '-').replace('/', '-').replace(' ', '-').replace("'s", "")
        candidates.append(f"{safe_name_hyphen}-Positive.png")

        candidates.append(f"{safe_name_hyphen}2.png")
        candidates.append(f"{name}2.png")

        safe_name_underscore = safe_name_hyphen.replace('-', '_')
        candidates.append(f"{safe_name_underscore}2.png")

        weird_name = name.replace(':', '-').replace('/', '-').replace(' ', '-_-').replace("'s", "")
        candidates.append(f"{weird_name}2.png")
        candidates.append(f"{weird_name}-Positive.png")

        found = None
        for cand in candidates:
             if cand in files:
                 found = cand
                 break
             if cand.lower() in files_lower:
                 found = files_lower[cand.lower()]
                 break

        if found and found != target_pos:
            print(f"Renaming {found} -> {target_pos}")
            shutil.move(os.path.join(images_dir, found), os.path.join(images_dir, target_pos))
            files.add(target_pos)
            files.remove(found)
            if found.lower() in files_lower:
                del files_lower[found.lower()]
            files_lower[target_pos.lower()] = target_pos
            updates += 1

# Write updated JSON
with open(json_path, 'w') as f:
    json.dump(conditions, f, indent=4)

print(f"Renamed {updates} images and updated JSON.")
