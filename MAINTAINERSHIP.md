<!--
SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>

SPDX-License-Identifier: MIT
-->

Maintainership Instructions
===========================

Releasing a New Version
-----------------------

1. Update the project's status in the `README.md` file, if required.
2. Update the copyright year in the following places, if required:
    - `LICENSE.txt`,
    - the `_Layout.cshtml` files in the `ForneverMind` project (there are two of them).
3. Choose the new version according to [Semantic Versioning][semver]. It should consist of three numbers (i.e. `1.0.0`).
4. Update the `<Version>` element contents in the `ForneverMind/ForneverMind.fsproj` file.
5. Update the `version` property in the `ForneverMind.Frontend/package.json` file.
6. Build the solution: it is supposed to update the versions in `**/*.package-lock.json` files.
7. Make sure there's a properly formed version entry in the `CHANGELOG.md` file.
8. Merge the changes via a pull request.
9. Push a tag named `v<VERSION>` to GitHub.

[semver]: https://semver.org/spec/v2.0.0.html
