import json
import os

json_path = 'MindBodyDictionaryMobile/Resources/Raw/conditionData.json'
images_dir = 'MindBodyDictionaryMobile/Resources/Raw/images'

with open(json_path, 'r') as f:
    data = json.load(f)

files_set = set(os.listdir(images_dir))

for item in data:
    name = item.get('name', '')
    # Apply all sanitization rules used in rename scripts:
    # 1. replace : with -
    # 2. replace / with -
    # 3. replace ' ' (space) with -
    # 4. remove 's
    safe_name = name.replace(':', '-').replace('/', '-').replace(' ', '-').replace("'s", "")
    
    neg_img = f"{safe_name}-Negative.png"
    pos_img = f"{safe_name}-Positive.png"
    
    # Update JSON properties
    # Remove old keys if present
    item.pop('cachedImageOne', None)
    item.pop('cachedImageTwo', None)
    
    if neg_img in files_set:
        item['imageNegative'] = neg_img
    else:
        # Check if maybe the file exists with slightly different name?
        # For now, just warn.
        # But we want to persist null if not found so code doesn't try to load invalid path.
        item['imageNegative'] = None
        print(f"Warning: Image not found: {neg_img}")
        
    if pos_img in files_set:
        item['imagePositive'] = pos_img
    else:
        item['imagePositive'] = None
        print(f"Warning: Image not found: {pos_img}")

with open(json_path, 'w') as f:
    json.dump(data, f, indent=4)
