import os
import json
import random
import shutil
from PIL import Image

# wejściowe katalogi
free_dir = "dataset_free"
infested_dir = "dataset_infested"
labels_file = "labels.txt"

# wyjściowe katalogi
out_root = "dataset"
out_dirs = {
    "train_images": os.path.join(out_root, "images/train"),
    "val_images": os.path.join(out_root, "images/val"),
    "train_labels": os.path.join(out_root, "labels/train"),
    "val_labels": os.path.join(out_root, "labels/val")
}
for d in out_dirs.values():
    os.makedirs(d, exist_ok=True)

# wczytaj etykiety do mapy
ann_map = {}
if os.path.exists(labels_file):
    with open(labels_file, "r") as f:
        for line in f:
            line = line.strip()
            if not line:
                continue
            try:
                data = json.loads(line)
            except json.JSONDecodeError:
                print("⚠️ Błąd JSON, pomijam:", line[:50])
                continue

            # np. "frame_4" -> "1_00953.MTS_frame4.png"
            frame_num = data["id"].replace("frame_", "")
            video_name = data["video"].split("/")[-1].replace(".MTS", "")
            img_name = f"{video_name}_frame{frame_num}.png"

            ann_map[img_name] = data

# zbierz wszystkie obrazki z obu folderów
all_images = []
for folder in [free_dir, infested_dir]:
    for file in os.listdir(folder):
        if file.endswith(".png"):
            all_images.append((os.path.join(folder, file), file))

# podział train/val
random.shuffle(all_images)
split = int(0.8 * len(all_images))
train_set = all_images[:split]
val_set = all_images[split:]

def save_label(img_path, file_name, out_dir):
    """Zapisuje etykietę YOLO dla obrazka"""
    label_str = ""
    if file_name in ann_map:
        ann = ann_map[file_name]
        if ann["varroa_visible"] == "yes":
            img = Image.open(img_path)
            w, h = img.size
            x1, y1 = ann["coord_1"]
            x2, y2 = ann["coord_2"]

            # YOLO format (normalized)
            x_center = (x1 + x2) / 2 / w
            y_center = (y1 + y2) / 2 / h
            bw = (x2 - x1) / w
            bh = (y2 - y1) / h
            label_str = f"0 {x_center:.6f} {y_center:.6f} {bw:.6f} {bh:.6f}\n"

    # zapisz plik .txt (pusty lub z boxem)
    out_label = os.path.join(out_dir, file_name.replace(".png", ".txt"))
    with open(out_label, "w") as f:
        f.write(label_str)

# kopiowanie obrazków i generowanie etykiet
for img_path, file_name in train_set:
    shutil.copy(img_path, os.path.join(out_dirs["train_images"], file_name))
    save_label(img_path, file_name, out_dirs["train_labels"])

for img_path, file_name in val_set:
    shutil.copy(img_path, os.path.join(out_dirs["val_images"], file_name))
    save_label(img_path, file_name, out_dirs["val_labels"])

# stwórz data.yaml
with open(os.path.join(out_root, "data.yaml"), "w") as f:
    f.write(
        "train: dataset/images/train\n"
        "val: dataset/images/val\n\n"
        "nc: 1\n"
        "names: ['varroa']\n"
    )

print("✅ Dataset przygotowany! Możesz teraz trenować YOLOv8.")
