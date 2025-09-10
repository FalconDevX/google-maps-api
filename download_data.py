import requests
import json

nearby_places_url= "https://localhost:63286/api/GoogleMaps/getNearbyPlacesByName"

text_places_url = "https://localhost:63286/api/GoogleMaps/getTextPlacesByName"

data_results = []

unique_place_ids = {}

place = "Kraków Kościuł Mariacki"
radius = 500

response = requests.get(nearby_places_url, params={"query": place, "radius": radius, "rankPreference": "POPULARITY"}, verify=False)

if response.status_code == 200:
    data_results.extend(response.json())
else:
    print(f"Error: {response.status_code}")

for result in data_results:
    key=result.get("id")
    if key not in unique_place_ids:
        unique_place_ids[key] = result
    
response = requests.get(nearby_places_url, params={"query": place, "radius": radius, "rankPreference": "DISTANCE"}, verify=False)

if response.status_code == 200:
    data_results.extend(response.json())
else:
    print(f"Error: {response.status_code}")

for result in data_results:
    key=result.get("id")
    if key not in unique_place_ids:
        unique_place_ids[key] = result

with open("output.json", "w", encoding="utf-8") as file:
    json.dump(list(unique_place_ids.values()), file, indent=4, ensure_ascii=False)