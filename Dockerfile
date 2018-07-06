FROM microsoft/dotnet:2.1-sdk AS build-env
RUN apt-get update && apt-get install -y nodejs-legacy

WORKDIR /app

COPY ./EvilPlanner.Backend/EvilPlanner.Backend.fsproj ./EvilPlanner.Backend/
COPY ./EvilPlanner.Core/EvilPlanner.Core.fsproj ./EvilPlanner.Core/
COPY ./EvilPlanner.Frontend/EvilPlanner.Frontend.csproj ./EvilPlanner.Frontend/
COPY ./EvilPlanner.Logic/EvilPlanner.Logic.fsproj ./EvilPlanner.Logic/
COPY ./EvilPlanner.Tests/EvilPlanner.Tests.fsproj ./EvilPlanner.Tests/
COPY ./EvilPlanner.sln ./
RUN dotnet restore

COPY . ./
RUN dotnet publish ./EvilPlanner.Backend -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime

WORKDIR /app

COPY --from=build-env /app/EvilPlanner.Backend/out .
COPY ./deploy/appsettings.json ./

ENTRYPOINT ["dotnet", "EvilPlanner.Backend.dll"]
