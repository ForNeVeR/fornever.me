FROM mcr.microsoft.com/powershell:7.2.2-debian-10 AS talks-env

# First, install curl to be able to install Node.js, and then install Node.js itself:
RUN apt-get update \
    && apt-get install --no-install-recommends -y curl \
    && curl -fsSL https://deb.nodesource.com/setup_16.x | bash - \
    && apt-get install --no-install-recommends -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

RUN npm install -g yarn@1.22.0

WORKDIR /talks

COPY ./ForneverMind/wwwroot/talks ./ForneverMind/wwwroot/talks/
COPY ./Scripts ./Scripts/
RUN pwsh ./Scripts/Prepare-Talks.ps1

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Install Node.js:
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash - \
    && apt-get install --no-install-recommends -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY ./Directory.Build.props ./
COPY ./ForneverMind.Frontend/ForneverMind.Frontend.proj ./ForneverMind.Frontend/
COPY ./ForneverMind.TestFramework/ForneverMind.TestFramework.fsproj ./ForneverMind.TestFramework/
COPY ./ForneverMind.Tests/ForneverMind.Tests.fsproj ./ForneverMind.Tests/
COPY ./ForneverMind.sln ./
COPY ./ForneverMind/ForneverMind.fsproj ./ForneverMind/
RUN dotnet restore

COPY ./ForneverMind ./ForneverMind/
COPY ./ForneverMind.Frontend ./ForneverMind.Frontend/
COPY ./ForneverMind.Tests ./ForneverMind.Tests/
COPY --from=talks-env /talks/ForneverMind/wwwroot/talks/ ./ForneverMind/wwwroot/talks/
RUN dotnet publish ./ForneverMind -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app

COPY --from=build-env /app/out .
COPY ./deploy/appsettings.json ./

ENTRYPOINT ["dotnet", "ForneverMind.dll"]
