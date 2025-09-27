import os

# Ścieżki
img_dir = r"C:\PythonDev\yolov8_premodel\dataset\images"
lbl_dir = r"C:\PythonDev\yolov8_premodel\dataset\labels"

# lista labeli
labels = [f for f in os.listdir(lbl_dir) if f.endswith(".txt")]
labels.sort()

counter = 1
for lbl_file in labels:
    # część wspólna (np. 12_00966.MTS_frame45)
    base_key = lbl_file.split("__")[-1].replace(".txt", "")
    img_file = None

    # szukamy obrazu z tym fragmentem w nazwie
    for f in os.listdir(img_dir):
        if base_key in f:
            img_file = f
            break

    if img_file:
        new_img = f"{counter}.png"
        new_lbl = f"{counter}.txt"

        old_img_path = os.path.join(img_dir, img_file)
        new_img_path = os.path.join(img_dir, new_img)

        old_lbl_path = os.path.join(lbl_dir, lbl_file)
        new_lbl_path = os.path.join(lbl_dir, new_lbl)

        os.rename(old_img_path, new_img_path)
        os.rename(old_lbl_path, new_lbl_path)

        print(f"[OK] {img_file} + {lbl_file} → {new_img}, {new_lbl}")
        counter += 1

# usuń obrazy bez labeli
valid_imgs = {f"{i}.png" for i in range(1, counter)}
for f in os.listdir(img_dir):
    if f not in valid_imgs:
        os.remove(os.path.join(img_dir, f))
        print(f"[DEL] usunięto {f}")
