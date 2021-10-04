FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . .
RUN apt-get update -yq 
RUN apt-get install curl gnupg -yq 
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs
RUN dotnet publish BattleShip.csproj -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /src
COPY --from=build-env /src/publish .

CMD ASPNETCORE_URLS=http://*:$PORT dotnet BattleShip.dll