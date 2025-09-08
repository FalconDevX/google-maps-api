import requests
import json

# url = "http://34.56.66.163/api/GoogleMaps/getNearbyPlacesByName"
url = "https://localhost:63286/api/GoogleMaps/getNearbyPlacesByName"

all_results = []  

places = []

with open("places.csv", "r", encoding="utf-8") as file:
    reader = file.readlines()
    for line in reader:
        places.append(line.strip())

for place in places:
    params = {
        "query": place,
        "radius": 500
    }

    response = requests.get(url, params=params, verify=False)

    if response.status_code == 200:
        data = response.json()  
        all_results.extend(data)  
        print(f"✅ Dodano dane dla: {place}")
    else:
        print(f"❌ Błąd dla {place}: {response.status_code}")

with open("output.json", "w", encoding="utf-8") as f:
    json.dump(all_results, f, indent=4, ensure_ascii=False)

print("📁 Wszystkie dane zapisane do output.json")
