import requests
import json

nearby_places_url = "http://34.56.66.163/api/GoogleMaps/getNearbyPlacesByName"
text_places_url = "http://34.56.66.163/api/GoogleMaps/getTextPlacesByName"

access_token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE3NTg5NzYxNjgsImlzcyI6Ik15QXBwIiwiYXVkIjoiQXBwVXNlcnMiLCJzdWIiOjExLCJuYW1lIjoidGVzdDUifQ.G3dkLstTB-ssMTLT3kGyW_jxWo1vyH3tKxLhqkjJsqI"

headers = {
    "Authorization": f"Bearer {access_token}"
}

data_results = []
unique_place_ids = {}

with open("places.json", "r", encoding="utf-8") as f:
    places = json.load(f)

radius = 500

for place in places:
    for rank in ["POPULARITY", "DISTANCE"]:
        response = requests.get(
            nearby_places_url,
            params={"query": place, "radius": radius, "rankPreference": rank},
            headers=headers,
            verify=False
        )

        if response.status_code == 200:
            results = response.json()
            for result in results:
                key = result.get("id")
                if key and key not in unique_place_ids:
                    unique_place_ids[key] = result
        else:
            print(f"Błąd ({response.status_code}) dla {place} [{rank}]")

with open("output.json", "w", encoding="utf-8") as file:
    json.dump(list(unique_place_ids.values()), file, indent=4, ensure_ascii=False)

print(f"✅ Zapisano {len(unique_place_ids)} miejsc do output.json")
