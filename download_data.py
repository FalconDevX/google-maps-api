import requests
import json

nearby_places_url= "http://34.56.66.163/api/GoogleMaps/getNearbyPlacesByName"

text_places_url = "http://34.56.66.163/api/GoogleMaps/getTextPlacesByName"

access_token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE3NTg5NzIyODksImlzcyI6Ik15QXBwIiwiYXVkIjoiQXBwVXNlcnMiLCJzdWIiOjExLCJuYW1lIjoidGVzdDUifQ.FYobDc89KJZNpplbiJXAF4r_UusN6hZ3xf7tj4LWzAQ"

headers = {
    "Authorization": f"Bearer {access_token}"
}

data_results = []

unique_place_ids = {}

place = "Kraków Kościuł Mariacki"
radius = 500

response = requests.get(
    nearby_places_url,
    params={"query": place, "radius": radius, "rankPreference": "POPULARITY"},
    headers=headers,
    verify=False
)
if response.status_code == 200:
    data_results.extend(response.json())
else:
    print(f"Error: {response.status_code}")

for result in data_results:
    key=result.get("id")
    if key not in unique_place_ids:
        unique_place_ids[key] = result
    
response = requests.get(
    nearby_places_url,
    params={"query": place, "radius": radius, "rankPreference": "DISTANCE"},
    headers=headers,
    verify=False
)
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