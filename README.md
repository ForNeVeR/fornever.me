<!--
SPDX-FileCopyrightText: 2024-2025 Friedrich von Never <friedrich@fornever.me>

SPDX-License-Identifier: MIT
-->

fornever.me: Engineer, Programmer, Gentleman [![Status Aquana][status-aquana]][andivionian-status-classifier] [![Docker Image][badge.docker]][docker-hub]
============================================

This is the [fornever.me][] site source code. It uses ForneverMind — a simple
homemade blog engine mainly written in F# programming language.

Features
--------

- ASP.NET Core web engine.
- Main pages layout in Razor.
- Blog posts are written in Markdown.
- Source code highlighting is provided by [highlight.js][] on the server side.
- There's a Plans page that provides a daily quote from [the Evil Overlord List][evil-overlord-list].
  - A quote for each day is stored in a LiteDB database.
- [Disqus][disqus] comment system.

Contributing
------------

To know how to develop the application locally, read [the contributor guide][docs.contributor-guide].

Deployment
----------

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

Documentation
-------------

- [The Contributor Guide][docs.contributor-guide]
- [Changelog][docs.changelog]
- [Maintainership][docs.maintainership]

License
-------
The project is distributed under the terms of [the MIT license][docs.license].

The license indication in the project's sources is compliant with the [REUSE specification v3.3][reuse.spec].

[andivionian-status-classifier]: https://github.com/ForNeVeR/andivionian-status-classifier#status-aquana-
[badge.docker]: https://img.shields.io/docker/v/revenrof/fornever.me?label=docker&sort=semver
[disqus]: https://disqus.com/
[docker-hub]: https://hub.docker.com/r/revenrof/fornever.me
[docs.changelog]: CHANGELOG.md
[docs.contributor-guide]: CONTRIBUTING.md
[docs.license]: LICENSE.txt
[docs.maintainership]: ./MAINTAINERSHIP.md
[evil-overlord-list]: https://legendspbem.angelfire.com/eviloverlordlist.html
[fornever.me]: https://fornever.me/
[highlight.js]: https://highlightjs.org/
[reuse.spec]: https://reuse.software/spec-3.3/
[status-aquana]: https://img.shields.io/badge/status-aquana-yellowgreen.svg
