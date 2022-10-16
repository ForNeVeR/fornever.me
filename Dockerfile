FROM mcr.microsoft.com/powershell:7.1.3-debian-10 AS talks-env

# First, install curl to be able to install Node.js, and then install Node.js itself:
RUN apt-get update \
    && apt-get install -y curl \
    && curl -fsSL https://deb.nodesource.com/setup_16.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

RUN npm install -g yarn@1.22.0

WORKDIR /talks

COPY ./ForneverMind/wwwroot/talks ./ForneverMind/wwwroot/talks/
COPY ./Scripts ./Scripts/
RUN pwsh ./Scripts/Prepare-Talks.ps1

FROM mcr.microsoft.com/dotnet/sdk:5.0.300 AS build-env

# Install Node.js:
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY ./ForneverMind/ForneverMind.fsproj ./ForneverMind/
COPY ./ForneverMind.Frontend/ForneverMind.Frontend.csproj ./ForneverMind.Frontend/
COPY ./ForneverMind.Tests/ForneverMind.Tests.fsproj ./ForneverMind.Tests/
COPY ./EvilPlanner/EvilPlanner.Backend/EvilPlanner.Backend.fsproj ./EvilPlanner/EvilPlanner.Backend/
COPY ./EvilPlanner/EvilPlanner.Core/EvilPlanner.Core.fsproj ./EvilPlanner/EvilPlanner.Core/
COPY ./EvilPlanner/EvilPlanner.Frontend/EvilPlanner.Frontend.csproj ./EvilPlanner/EvilPlanner.Frontend/
COPY ./EvilPlanner/EvilPlanner.Logic/EvilPlanner.Logic.fsproj ./EvilPlanner/EvilPlanner.Logic/
COPY ./EvilPlanner/EvilPlanner.Tests/EvilPlanner.Tests.fsproj ./EvilPlanner/EvilPlanner.Tests/
COPY ./ForneverMind.sln ./.
RUN dotnet restore

COPY ./ForneverMind ./ForneverMind/
COPY ./ForneverMind.Frontend ./ForneverMind.Frontend/
COPY ./ForneverMind.Tests ./ForneverMind.Tests/
COPY --from=talks-env /talks/ForneverMind/wwwroot/talks/ ./ForneverMind/wwwroot/talks/
RUN dotnet publish ./ForneverMind -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:5.0.5

# First, install curl to be able to install Node.js, and then install Node.js itself:
RUN apt-get update \
    && apt-get install -y curl \
    && curl -fsSL https://deb.nodesource.com/setup_16.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build-env /app/out .
COPY ./deploy/appsettings.json ./

ENTRYPOINT ["dotnet", "ForneverMind.dll"]
