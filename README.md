# Welcome to Pokedex!

This is a RESTful API application which contains 2 endpoints:

1. /api/pokemons/{name}
2. /api/pokemons/translated/{name}

# Getting Started

1. Install the latest [Docker](https://docs.docker.com/get-docker/)
2. In the root of solution, run `docker-compose up -d`
3. Open the browser and navigate to http://localhost/, and append the endpoint
4. You should see the response like: 

````json
{
    "name": "mewtwo",
    "description": "It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.",
    "habitat": "rare",
    "isLegendary": true
}
