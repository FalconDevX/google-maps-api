import random
import json
import matplotlib.pyplot as plt

point = 0

stats = {}

votes = []

category_points = {}

with open("output.json", "r", encoding="utf-8") as f:
    data = json.load(f)

remaining_places = list(range(len(data)))

for i in range(len(data)):
    item = data[i]['displayName']['text']
    types = list(data[i]['types'])

    print(f"Do you like this place: {item}?")
    choice = random.choice(['y', 'n', 's'])

    if choice == 'y':
        point = 1
    elif choice == 'n':
        point = -1
    elif choice == 's':
        point = 0

    for type in types:
        category_points[type] = category_points.get(type, 0) + point

sorted_data = dict(sorted(category_points.items(), key=lambda x: x[1], reverse=True))

top_n = 15
categories = list(sorted_data.keys())[:top_n]
values = list(sorted_data.values())[:top_n]

plt.figure(figsize=(12,6))
plt.bar(categories, values, color="skyblue")
plt.xticks(rotation=45, ha="right")
plt.ylabel("Liczba wystąpień")
plt.title(f"Top {top_n} kategorii miejsc")
plt.tight_layout()
plt.show()

values = list(category_points.values())

plt.hist(values, bins=20, color="skyblue", edgecolor="black")
plt.title("Histogram punktów kategorii")
plt.xlabel("Wartość punktowa")
plt.ylabel("Liczba kategorii")
plt.show()