import os
import shutil

images_dir = 'MindBodyDictionaryMobile/Resources/Raw/images'

files = os.listdir(images_dir)

for filename in files:
    if "'s" in filename:
        # Replace 's with empty string
        new_name = filename.replace("'s", "")
        
        src_path = os.path.join(images_dir, filename)
        dst_path = os.path.join(images_dir, new_name)
        
        if src_path != dst_path:
            print(f"Renaming '{filename}' -> '{new_name}'")
            shutil.move(src_path, dst_path)
