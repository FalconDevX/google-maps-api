from ultralytics import YOLO

def train():
    model = YOLO("yolov8n.pt")  # najmniejszy model
    model.train(
        data="C:/PythonDev/bees/project/dataset/data.yaml",
        epochs=50,
        imgsz=640,
        batch=16,
        device=0   # GPU
    )

if __name__ == "__main__":
    train()
