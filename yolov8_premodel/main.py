from ultralytics import YOLO

def main():
    model = YOLO("yolov8s.pt")

    model.train(
        data="dataset.yaml",
        epochs=150,
        imgsz=640,
        batch=8,
        name="varroa_experiment",
        patience=50,
        augment=True
    )

if __name__ == "__main__":
    main()
