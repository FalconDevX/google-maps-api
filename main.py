import random
import json
import matplotlib.pyplot as plt
import os

point = 0

stats = {}

votes = []

category_points = {}

if os.path.exists("votes.json") and os.path.getsize("votes.json") > 0:
    with open("votes.json", "r", encoding="utf-8") as f:
        old_category_points = json.load(f)
else:
    old_category_points = {} 

if os.path.exists("output.json") and os.path.getsize("output.json") > 0:
    with open("output.json", "r", encoding="utf-8") as f:
        data = json.load(f)
else:
    data = []

remaining_places = list(range(len(data)))

sampled_places = random.sample(remaining_places, min(20, len(remaining_places)))

for i in sampled_places:
    item = data[i]['displayName']['text']
    types = list(data[i]['types'])

    print(f"Do you like this place: {item}?")
    choice = input("Enter 'y' for yes, 'n' for no, 's' for skip, or 'q' to quit: ").strip().lower()

    if choice == 'y':
        point = 1
    elif choice == 'n':
        point = -1
    elif choice == 's':
        point = 0
    elif choice == 'q':
        break
    else:
        print("Invalid input. Please enter 'y', 'n', 's', or 'q'.")
        continue

    votes.append((item, choice))

    for t in types:
        category_points[t] = category_points.get(t, 0) + point

for cat, pts in category_points.items():
    old_category_points[cat] = old_category_points.get(cat, 0) + pts

category_points = old_category_points

sorted_data = dict(sorted(category_points.items(), key=lambda x: x[1], reverse=True))

top_n = 15
categories = list(sorted_data.keys())[:top_n]
values = list(sorted_data.values())[:top_n]

with open("votes.json", "w", encoding="utf-8") as f:
    json.dump(category_points, f, indent=4, ensure_ascii=False)

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