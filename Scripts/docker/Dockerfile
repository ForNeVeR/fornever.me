FROM microsoft/aspnetcore:1.1.1

WORKDIR /app
COPY out .
COPY appsettings.json /app

ENTRYPOINT ["dotnet", "ForneverMind.dll"]
