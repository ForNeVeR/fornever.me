EvilPlanner [![Status Ventis](https://img.shields.io/badge/status-ventis-yellow.svg)](https://github.com/ForNeVeR/andivionian-status-classifier) [![Build status](https://ci.appveyor.com/api/projects/status/7h5slaytywuhshp6/branch/develop?svg=true)](https://ci.appveyor.com/project/ForNeVeR/evilplanner/branch/develop)
===========
EvilPlanner is a nice statistical application.

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
EvilPlanner uses [Web.config transformations][web-config-transform], so there
is a basic `Web.config` file used for debug and `Web.Release.config` used for
publishing. Be sure to make changes to an appropriate file!

You'll probably want to change `databasePath` parameter and point it to a
preferred location for the database.

By default production version of EvilPlanner assumes that it will be deployed
to https://fornever.me/plans/.

Frontend project configuration is stored inside of the `config.ts` file. It
should point to an API endpoint and it will be applied on TypeScript
compilation.

Build and Deploy
----------------
EvilPlanner consists of independent backend and frontend parts. They should be
compiled and deployed separately.

### Backend
You may compile and publish the code from Visual Studio, or using `msbuild`
(assuming that you have both `nuget` and `msbuild` in your `PATH` environment
variable):

```console
$ nuget restore
$ msbuild EvilPlanner.sln /p:Platform="Any CPU" /p:Configuration=Release /p:ProductionDeploy=true /p:PublishProfile=Production
```

### Frontend
To compile frontend, you'll need a local npm installation. First of all,
install used packages:

```console
cd EvilPlanner.Frontend
npm install
```

After that, you can compile code with Visual Studio (it will use the prodived
`Gulpfile.js`) or from the command line:

```console
npm run build
```

#### Watch
The `watch` task has been provided for the development purposes. Visual Studio
will start it automatically; you also have an option to start it from the
terminal:

```console
npm run watch
```

For production, you should use the following commands:

```console
rm ./dist/*
npm run deploy
cp -r -force ./dist/* $TargetPath
```

Frontend project may be deployed to any server.

[web-config-transform]: http://www.asp.net/mvc/overview/deployment/visual-studio-web-deployment/web-config-transformations
