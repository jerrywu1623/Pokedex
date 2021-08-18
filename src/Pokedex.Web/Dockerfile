#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Pokedex.Web/Pokedex.Web.csproj", "src/Pokedex.Web/"]
COPY ["src/Pokedex.Infrastructure/Pokedex.Infrastructure.csproj", "src/Pokedex.Infrastructure/"]
COPY ["src/Pokedex.Application/Pokedex.Application.csproj", "src/Pokedex.Application/"]
COPY ["src/Pokedex.Domain/Pokedex.Domain.csproj", "src/Pokedex.Domain/"]
RUN dotnet restore "src/Pokedex.Web/Pokedex.Web.csproj"
COPY . .
WORKDIR "/src/src/Pokedex.Web"
RUN dotnet build "Pokedex.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pokedex.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pokedex.Web.dll"]