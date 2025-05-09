<!--
SPDX-FileCopyrightText: 2024-2025 Friedrich von Never <friedrich@fornever.me>

SPDX-License-Identifier: MIT
-->

Contributor Guide
=================

Dependencies
------------

To build ForneverMind, the following is required:
- [.NET SDK][dotnet] 6 or later,
- [Node.js][node-js] 22 or later.

To set the Node-related prerequisites up, you may use [Volta][volta]:

```console
$ volta install node@22
```

Build
-----

### Talks

There's an additional talks archive included as a git submodule in this
repository. To prepare tasks for build, use the `Scripts/Prepare-Talks.ps1`
script. Talks require external [Yarn][yarn] installation of v1.22.0 or higher. Again, you may set it up using [Volta][volta]:

```console
$ volta install yarn@1
```

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

File Encoding Changes
---------------------
If the automation asks you to update the file encoding (line endings or UTF-8 BOM) in certain files, either fix the files manually, or run [the VerifyEncoding script][verify-encoding] in [PowerShell Core][powershell] with the `-Autofix` argument:
```console
$ pwsh -Command 'Install-Module VerifyEncoding && Test-Encoding -Autofix'
```

The `-AutoFix` switch will automatically fix the encoding issues, and you'll only need to commit and push the changes.

[dotnet]: https://dotnet.microsoft.com/
[node-js]: https://nodejs.org/
[nvm-windows]: https://github.com/coreybutler/nvm-windows
[powershell]: https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell
[reuse]: https://reuse.software/
[verify-encoding]: https://github.com/ForNeVeR/VerifyEncoding
[volta]: https://volta.sh/
[yarn]: https://yarnpkg.com/
