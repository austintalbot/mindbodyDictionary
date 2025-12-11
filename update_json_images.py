import json
import os

json_path = 'MindBodyDictionaryMobile/Resources/Raw/conditionData.json'
images_dir = 'MindBodyDictionaryMobile/Resources/Raw/images'

with open(json_path, 'r') as f:
    data = json.load(f)

files_set = set(os.listdir(images_dir))

for item in data:
    name = item.get('name', '')
    # Apply all sanitization rules
    safe_name = name.replace(':', '-').replace('/', '-').replace(' ', '-').replace("'s", "")
    
    neg_img = f"{safe_name}-Negative.png"
    pos_img = f"{safe_name}-Positive.png"
    
    # Verify existence (optional but good for data integrity)
    if neg_img in files_set:
        item['cachedImageOne'] = neg_img
    else:
        # Fallback logic or keep null?
        # Maybe log it?
        print(f"Warning: Image not found: {neg_img} for condition '{name}'")
        
    if pos_img in files_set:
        item['cachedImageTwo'] = pos_img
    else:
        print(f"Warning: Image not found: {pos_img} for condition '{name}'")

with open(json_path, 'w') as f:
    json.dump(data, f, indent=4)
