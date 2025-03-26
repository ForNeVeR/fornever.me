<!--
SPDX-FileCopyrightText: 2024-2025 Friedrich von Never <friedrich@fornever.me>

SPDX-License-Identifier: MIT
-->

The Contributor Guide
=====================

Dependencies
------------

To build ForneverMind, the following is required:
- [.NET SDK][dotnet] 6 or later,
- [Node.js][node-js] 16.

It is recommended to use [nvm-windows][] on Windows:

```console
$ nvm install 16
Installation complete. If you want to use this version, type

nvm use 16.18.0
# nvm use 16.18.0
```

Configuration
-------------

Backend reads its settings from the standard `appsettings.json` file. The available settings are:

- `baseUrl`: URL to listen when started.

Build
-----

### Talks

There's an additional talks archive included as a git submodule in this
repository. To prepare tasks for build, use the `Scripts/Prepare-Talks.ps1`
script. Talks require external [Yarn][yarn] installation of v1.22.0 or higher.

### Frontend and Backend

Make sure you have all the build dependencies installed, then run the following shell command:

```console
$ dotnet build
```

To run the built application, execute the following shell command:

```console
$ dotnet run --project ForneverMind
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

License Automation
------------------
<!-- REUSE-IgnoreStart -->

If the CI asks you to update the file licenses, follow one of these:
1. Update the headers manually (look at the existing files), something like this:
   ```fsharp
   // SPDX-FileCopyrightText: %year% %your name% <%your contact info, e.g. email%>
   //
   // SPDX-License-Identifier: MIT
   ```
   (accommodate to the file's comment style if required).
2. Alternately, use the [REUSE][reuse] tool:
   ```console
   $ reuse annotate --license MIT --copyright '%your name% <%your contact info, e.g. email%>' %file names to annotate%
   ```
(Feel free to attribute the changes to "Praefectus contributors <https://github.com/ForNeVeR/Praefectus>"
instead of your name in a multi-author file,
or if you don't want your name to be mentioned in the project's source: this doesn't mean you'll lose the copyright.)

<!-- REUSE-IgnoreEnd -->

[dotnet]: https://dotnet.microsoft.com/
[node-js]: https://nodejs.org/
[nvm-windows]: https://github.com/coreybutler/nvm-windows
[reuse]: https://reuse.software/
[yarn]: https://yarnpkg.com/
