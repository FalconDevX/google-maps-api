import os
import shutil
import random

# Ścieżki wejściowe
images_dir = r"C:\PythonDev\yolov8_premodel\dataset\images"
labels_dir = r"C:\PythonDev\yolov8_premodel\dataset\labels"

# Ścieżki wyjściowe
output_base = r"C:\PythonDev\yolov8_premodel\dataset_split"
train_img_dir = os.path.join(output_base, "images", "train")
val_img_dir = os.path.join(output_base, "images", "val")
train_lbl_dir = os.path.join(output_base, "labels", "train")
val_lbl_dir = os.path.join(output_base, "labels", "val")

# Utwórz katalogi
for d in [train_img_dir, val_img_dir, train_lbl_dir, val_lbl_dir]:
    os.makedirs(d, exist_ok=True)

# Lista obrazów i odpowiadających im labeli
images = [f for f in os.listdir(images_dir) if f.endswith(".png")]
images.sort()

pairs = []
for img in images:
    name = os.path.splitext(img)[0]
    lbl = name + ".txt"
    lbl_path = os.path.join(labels_dir, lbl)
    if os.path.exists(lbl_path):
        pairs.append((img, lbl))

print(f"Znaleziono {len(pairs)} par obraz+label")

# Tasujemy dane
random.shuffle(pairs)

# Podział 80/20
split_idx = int(0.8 * len(pairs))
train_pairs = pairs[:split_idx]
val_pairs = pairs[split_idx:]

# Kopiowanie do folderów
def copy_pairs(pairs, img_dest, lbl_dest):
    for img, lbl in pairs:
        shutil.copy(os.path.join(images_dir, img), os.path.join(img_dest, img))
        shutil.copy(os.path.join(labels_dir, lbl), os.path.join(lbl_dest, lbl))

copy_pairs(train_pairs, train_img_dir, train_lbl_dir)
copy_pairs(val_pairs, val_img_dir, val_lbl_dir)

print(f"Train: {len(train_pairs)} par")
print(f"Val: {len(val_pairs)} par")
