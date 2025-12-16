import json
import os
import shutil

json_path = 'MindBodyDictionaryMobile/Resources/Raw/conditionData.json'
images_dir = 'MindBodyDictionaryMobile/Resources/Raw/images'

with open(json_path, 'r') as f:
    conditions = json.load(f)

files = set(os.listdir(images_dir))
files_lower = {f.lower(): f for f in files}

def get_safe_name(name):
    # Same logic as update_json_images_final.py
    return name.replace(':', '-').replace('/', '-').replace(' ', '-').replace("'s", "")

updates = 0

for c in conditions:
    name = c['name']
    safe_name = get_safe_name(name)
    
    # Expected names
    target_neg = f"{safe_name}-Negative.png"
    target_pos = f"{safe_name}-Positive.png"
    
    # Check Negative
    if target_neg not in files:
        # Try to find candidate
        candidates = []
        # Swelling-_-Edema1.png case: spaces became -_-, 1 suffix
        # Replace spaces with -_-
        cand1 = name.replace(':', '-').replace('/', '-').replace(' ', '-_-').replace("'s", "") + "1.png"
        candidates.append(cand1)
        
        # Just 1 suffix on safe_name
        candidates.append(f"{safe_name}1.png")
        
        # Original name with 1
        candidates.append(f"{name}1.png")
        
        # Try safe_name with spaces replaced by _
        safe_underscore = safe_name.replace('-', '_')
        candidates.append(f"{safe_underscore}1.png")
        
        found = None
        for cand in candidates:
             if cand in files:
                 found = cand
                 break
             if cand.lower() in files_lower:
                 found = files_lower[cand.lower()]
                 break
        
        if found:
            print(f"Renaming {found} -> {target_neg}")
            shutil.move(os.path.join(images_dir, found), os.path.join(images_dir, target_neg))
            files.add(target_neg)
            files.remove(found)
            updates += 1

    # Check Positive
    if target_pos not in files:
        # Try to find candidate
        candidates = []
        # Swelling-_-Edema2.png case
        cand1 = name.replace(':', '-').replace('/', '-').replace(' ', '-_-').replace("'s", "") + "2.png"
        candidates.append(cand1)
        
        candidates.append(f"{safe_name}2.png")
        candidates.append(f"{name}2.png")
        
        safe_underscore = safe_name.replace('-', '_')
        candidates.append(f"{safe_underscore}2.png")

        found = None
        for cand in candidates:
             if cand in files:
                 found = cand
                 break
             if cand.lower() in files_lower:
                 found = files_lower[cand.lower()]
                 break
        
        if found:
            print(f"Renaming {found} -> {target_pos}")
            shutil.move(os.path.join(images_dir, found), os.path.join(images_dir, target_pos))
            files.add(target_pos)
            files.remove(found)
            updates += 1

print(f"Fixed {updates} images.")
