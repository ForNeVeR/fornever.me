Maintainership Instructions
===========================

Releasing a New Version
-----------------------

1. Update the copyright year in the following places, if required:
    - `LICENSE.md`,
    - the `_Layout.cshtml` files in the `ForneverMind` project (there are two of them),
    - the `index.html` files in the `EvilPlanner.Frontend` project (there are two of them as well).
2. Update the copyright year in [the license][license], and in the `_Layout.cshtml` files (currently, there are two of them), if required.
3. Choose the new version according to [Semantic Versioning][semver]. It should consist of three numbers (i.e. `1.0.0`).
4. Update the `<Version>` element contents in the `ForneverMind/ForneverMind.fsproj` file.
5. Update the `version` property in the `EvilPlanner/EvilPlanner.Frontend/package.json` file.
6. Update the `version` property in the `ForneverMind.Frontend/package.json` file.
7. Make sure there's a properly formed version entry in [the changelog][changelog].
8. Merge the changes via a pull request.
9. Push a tag named `v<VERSION>` to GitHub.

[changelog]: ./CHANGELOG.md
[license]: ./LICENSE.md
[semver]: https://semver.org/spec/v2.0.0.html
