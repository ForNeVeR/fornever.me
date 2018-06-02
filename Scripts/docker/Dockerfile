FROM microsoft/dotnet:2.1-aspnetcore-runtime
RUN apt-get update && apt-get install -y nodejs-legacy

WORKDIR /app
COPY out .
COPY appsettings.json /app

ENTRYPOINT ["dotnet", "ForneverMind.dll"]
