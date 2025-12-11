import os
import shutil

images_dir = 'MindBodyDictionaryMobile/Resources/Raw/images'

files = os.listdir(images_dir)

for filename in files:
    if ' ' in filename:
        # Replace spaces with dashes
        new_name = filename.replace(' ', '-')
        
        src_path = os.path.join(images_dir, filename)
        dst_path = os.path.join(images_dir, new_name)
        
        # Ensure we don't overwrite existing file unless intended (unlikely here)
        if src_path != dst_path:
            print(f"Renaming '{filename}' -> '{new_name}'")
            shutil.move(src_path, dst_path)
