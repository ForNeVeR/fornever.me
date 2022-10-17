fornever.me: Engineer, Programmer, Gentleman [![Status Aquana][status-aquana]][andivionian-status-classifier] [![Docker Image][badge.docker]][docker-hub]
============================================

This is the [fornever.me][] site source code. It uses ForneverMind — a simple
homemade blog engine mainly written in F# programming language.

Dependencies
------------

ForneverMind requires recent [Node.js][node-js] 16 installation both for building and for running. It is recommended to use [nvm-windows][] on Windows:

```console
$ nvm install 16
Installation complete. If you want to use this version, type

nvm use 16.18.0
# nvm use 16.18.0
```

Configuration
-------------

Backend reads its settings from the standard `appsettings.json` file. The
available settings are:

- `baseUrl`: URL to listen when started
- `databasePath`: path to the database file, use `~` as an alias to the application's binary directory

Build
-----

### Frontend

To compile frontend part, you'll need a [Node.js][node-js] 16 installation. The bundled [Yarn][yarn] package manager will be automatically executed on build.

### Talks

There's an additional talks archive included as a git submodule in this
repository. To prepare tasks for build, use the `Scripts/Prepare-Talks.ps1`
script. Talks require external [Yarn][yarn] installation of v1.22.0 or higher.

### Backend

To compile the backend, you'll need a [.NET SDK][dotnet] 6 (or later) installation.

Here's a sample build script:

```console
$ dotnet build
$ cd ForneverMind
$ dotnet run
```

Test
----

```console
$ dotnet test
```

Publish
-------

Prepare the production-ready distribution in the `publish` directory:

```console
$ dotnet publish --configuration Release --output publish ./ForneverMind
```

This application uses Docker for deployment. To create a Docker image, use the
following command:

```console
$ docker build -t revenrof/fornever.me:$FORNEVER_ME_VERSION -t revenrof/fornever.me:latest .
```

(where `$FORNEVER_ME_VERSION` is the version for the image to use)

Then push the image to the Docker Hub:

```console
$ docker login
$ docker push revenrof/fornever.me:$FORNEVER_ME_VERSION
$ docker push revenrof/fornever.me:latest
```

Deploy
------

To install the application from Docker, run the following command:

```console
$ docker run -d \
    --restart unless-stopped \
    -p:$PORT:80 \
    --name $NAME \
    -v $DATA:/data \
    revenrof/fornever.me:$VERSION
```

Where
- `$PORT` is the port you want to expose the application on
- `$NAME` is the container name
- `$VERSION` is the version you want to deploy, or `latest` for the latest
  available one
- `$DATA` is the database directory

For example, a production server may use the following settings (note this
command uses the Bash syntax; adapt for your shell if necessary):

```bash
PORT=5001
NAME=fornevermind
VERSION=latest
DATA=/opt/fornever/fornever.me/data
docker pull revenrof/fornever.me:$VERSION
docker rm -f $NAME
docker run -d \
    --restart unless-stopped \
    -p $PORT:80 \
    --name $NAME \
    -v $DATA:/data \
    revenrof/fornever.me:$VERSION
```

Other components
----------------

1. [EvilPlanner][evil-planner] meant to be an integral part of the site, but it needs to be installed separately.
2. [fornever.me][] uses an easy-to-install [Disqus][disqus] comment system.

Documentation
-------------

- [Changelog][changelog]
- [Maintainership][maintainership]
- [License (MIT)][license]

[andivionian-status-classifier]: https://github.com/ForNeVeR/andivionian-status-classifier#status-aquana-
[badge.docker]: https://img.shields.io/docker/v/revenrof/fornever.me?label=docker&sort=semver
[changelog]: CHANGELOG.md
[disqus]: https://disqus.com/
[docker-hub]: https://hub.docker.com/r/revenrof/fornever.me
[dotnet]: https://dotnet.microsoft.com/
[evil-planner]: https://github.com/ForNeVeR/EvilPlanner
[fornever.me]: https://fornever.me/
[license]: LICENSE.md
[maintainership]: ./MAINTAINERSHIP.md
[node-js]: https://nodejs.org/
[nvm-windows]: https://github.com/coreybutler/nvm-windows
[status-aquana]: https://img.shields.io/badge/status-aquana-yellowgreen.svg
[yarn]: https://yarnpkg.com/
