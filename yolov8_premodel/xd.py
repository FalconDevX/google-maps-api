import os

labels_root = r"C:\PythonDev\yolov8_premodel\dataset\labels"

for subset in ["train", "val"]:
    lbl_dir = os.path.join(labels_root, subset)

    for file in os.listdir(lbl_dir):
        if file.endswith(".txt"):
            path = os.path.join(lbl_dir, file)

            with open(path, "r") as f:
                lines = f.readlines()

            new_lines = []
            for line in lines:
                parts = line.strip().split()
                if len(parts) > 0:
                    parts[0] = "0"  # wymuś jedną klasę
                    new_lines.append(" ".join(parts) + "\n")

            with open(path, "w") as f:
                f.writelines(new_lines)

            print(f"[FIXED] {file}")
