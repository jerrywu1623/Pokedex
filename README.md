# Welcome to Pokedex!

This is a RESTful API application which contains 2 endpoints:

1. /api/pokemons/{name}
2. /api/pokemons/translated/{name}

# Getting Started

1. Install the latest [Docker](https://docs.docker.com/get-docker/)
2. In the root of solution, open the terminal and run `docker-compose up -d`
3. Open the browser and navigate to http://localhost/, and append the endpoint
4. You should see the response like: 

````json
{
    "name": "mewtwo",
    "description": "It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.",
    "habitat": "rare",
    "isLegendary": true
}
````

# Production Deployment
1. Open `appsettings.Production.json` in `Pokedex.Web`
2. Change the Uri of Elasticsearch
3. Open `docker-compose.override.yml` in root solution
4. Change `ASPNETCORE_ENVIRONMENT` to `Production`
5. Open the terminal and run `docker-compose up -d`
