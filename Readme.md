EvilPlanner [![Status Ventis](https://img.shields.io/badge/status-ventis-yellow.svg)](https://github.com/ForNeVeR/andivionian-status-classifier) [![Build status](https://ci.appveyor.com/api/projects/status/7h5slaytywuhshp6/branch/develop?svg=true)](https://ci.appveyor.com/project/ForNeVeR/evilplanner/branch/develop)
===========

EvilPlanner is a small lab application.

Features
--------

Currently it will only output the random quote from Evil Overlord List (see
quotation source for every quote near the quote itself).

Database Setup
--------------

EvilPlanner uses LiteDB for data storage. It will migrate its databases
automatically if necessary.

Configuration
-------------

Change `EvilPlanner.Backend/appsettings.json` file to change the application
settings.

You'll probably want to change `databasePath` parameter and point it to a
preferred location for the database.

Build
-----

You may compile the code using .NET Core SDK:

```console
$ dotnet build -c Release
```

Deployment
----------

To deploy the project, check the `docker-compose.yml` and run the following
script:

```
$ pwsh ./docker-compose.ps1
```
