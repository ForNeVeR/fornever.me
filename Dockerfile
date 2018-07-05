FROM microsoft/powershell:6.0.2-ubuntu-16.04 AS talks-env
RUN apt-get update && apt-get install -y nodejs-legacy npm
RUN npm install -g yarn

WORKDIR /talks

COPY ./ForneverMind/wwwroot/talks ./ForneverMind/wwwroot/talks/
COPY ./Scripts ./Scripts/
RUN pwsh ./Scripts/Prepare-Talks.ps1

FROM microsoft/dotnet:2.1-sdk AS build-env
RUN apt-get update && apt-get install -y nodejs-legacy

WORKDIR /app

COPY ./ForneverMind/ForneverMind.fsproj ./ForneverMind/
COPY ./ForneverMind.Frontend/ForneverMind.Frontend.csproj ./ForneverMind.Frontend/
COPY ./ForneverMind.Tests/ForneverMind.Tests.fsproj ./ForneverMind.Tests/
COPY ./ForneverMind.sln ./.
RUN dotnet restore

COPY ./ForneverMind ./ForneverMind/
COPY ./ForneverMind.Frontend ./ForneverMind.Frontend/
COPY ./ForneverMind.Tests ./ForneverMind.Tests/
COPY --from=talks-env /talks/ForneverMind/wwwroot/talks/ ./ForneverMind/wwwroot/talks/
RUN dotnet publish ./ForneverMind -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime
RUN apt-get update && apt-get install -y nodejs-legacy

WORKDIR /app

COPY --from=build-env /app/ForneverMind/out .
COPY ./deploy/appsettings.json ./

ENTRYPOINT ["dotnet", "ForneverMind.dll"]
