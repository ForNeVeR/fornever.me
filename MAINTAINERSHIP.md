Maintainership Instructions
===========================

Releasing a New Version
-----------------------

1. Update the copyright year in [the license][license], and in the `_Layout.cshtml` files (currently, there are two of them), if required.
2. Choose the new version according to [Semantic Versioning][semver]. It should consist of three numbers (i.e. `1.0.0`).
3. Update the `<Version>` element contents in the `ForneverMind/ForneverMind.fsproj` file.
4. Update the `version` property in the `ForneverMind.Frontend/package.json` file.
5. Make sure there's a properly formed version entry in [the changelog][changelog].
6. Merge the changes via a pull request.
7. Push a tag named `v<VERSION>` to GitHub.

[changelog]: ./CHANGELOG.md
[license]: ./LICENSE.md
[semver]: https://semver.org/spec/v2.0.0.html
