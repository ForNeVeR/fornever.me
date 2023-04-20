Maintainership Instructions
===========================

Releasing a New Version
-----------------------

1. Update the copyright year in the following places, if required:
    - `LICENSE.md`,
    - the `_Layout.cshtml` files in the `ForneverMind` project (there are two of them),
    - the `index.html` files in the `EvilPlanner.Frontend` project (there are two of them as well).
2. Choose the new version according to [Semantic Versioning][semver]. It should consist of three numbers (i.e. `1.0.0`).
3. Update the `<Version>` element contents in the `ForneverMind/ForneverMind.fsproj` file.
4. Update the `version` property in the `EvilPlanner/EvilPlanner.Frontend/package.json` file.
5. Update the `version` property in the `ForneverMind.Frontend/package.json` file.
6. Make sure there's a properly formed version entry in the `CHANGELOG.md` file.
7. Merge the changes via a pull request.
8. Push a tag named `v<VERSION>` to GitHub.

[semver]: https://semver.org/spec/v2.0.0.html
