FROM microsoft/aspnetcore:1.1.2
RUN apt-get update && apt-get install -y nodejs-legacy

WORKDIR /app
COPY out .
COPY appsettings.json /app

ENTRYPOINT ["dotnet", "ForneverMind.dll"]
