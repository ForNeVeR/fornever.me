fornever.me: Engineer, Programmer, Gentleman [![Status Ventis](https://img.shields.io/badge/status-ventis-yellow.svg)](https://github.com/ForNeVeR/andivionian-status-classifier) [![Build status](https://ci.appveyor.com/api/projects/status/dh7qx27hrjs8chp3/branch/develop?svg=true)](https://ci.appveyor.com/project/ForNeVeR/fornever-me/branch/develop)
============================================

This is the [fornever.me][] site source code. It uses ForneverMind — a simple
homemade blog engine mainly written in F# programming language.

Build
-----

### Frontend

To compile frontend part, you'll need a recent (6.9.1+) [Node.js][node-js]
installation. The bundled [Yarn][yarn] package manager will be automatically
executed on build.

### Talks

There's an additional talks archive included as git submodule in this
repository. To prepare tasks for build, use the `Scripts/Prepare-Talks.ps1`
script.

### Backend

To compile the backend, you'll need a [.NET Core][dotnet-core] installation.

Here's a sample build script:

```console
$ dotnet restore
$ dotnet build
$ cd ForneverMind
$ dotnet run
```

Publish
-------

TODO

Other components
----------------

1.  [EvilPlanner][evil-planner] meant to be an integral part of the site, but it
    need to be installed separately.
2.  [fornever.me][] uses an easy-to-install [Disqus][disqus] comment system.

[disqus]: https://disqus.com/
[dotnet-core]: https://www.microsoft.com/net/core
[evil-planner]: https://github.com/ForNeVeR/EvilPlanner
[fornever.me]: https://fornever.me/
[node-js]: https://nodejs.org/
[yarn]: https://yarnpkg.com/
