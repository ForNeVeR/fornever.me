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

Frontend project configuration is stored inside of the `config.ts` file. It
should point to an API endpoint and it will be applied on TypeScript
compilation.

Build and Deploy
----------------

You may compile and publish the code from Visual Studio, or using `msbuild`
(assuming that you have and `msbuild` in your `PATH` environment variable):

```console
$ msbuild EvilPlanner.sln /p:Platform="Any CPU" /p:Configuration=Release /p:ProductionDeploy=true /p:PublishProfile=Production
```
