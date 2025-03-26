<!--
SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>

SPDX-License-Identifier: MIT
-->

# Changelog

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic
Versioning](https://semver.org/spec/v2.0.0.html).

This file only documents changes in the site engine, not any changes in the
content or the hosting infrastructure. In the past, some versions were created
due to content-only changes (this is one of the reasons a version may be listed
here as having "no notable changes"); currently, no version increment is
performed in such a case.

## [Unreleased] (5.0.0)
### Removed
- EvilPlanner functionality, including everything (server API, pages) related to daily quotes.

### Upgrade Notes
When upgrading from previous versions,
- drop `databasePath` from the configuration file,
- drop the data by the path, it's no longer used by the application.

## [4.6.3] - 2024-10-21
### Changed
- Used library updates.

## [4.6.2] - 2024-09-25
### Changed
- Used library updates.

## [4.6.1] - 2024-07-14
### Fixed
- [#278: RSS feed reports invalid encoding: UTF-16](https://github.com/ForNeVeR/fornever.me/issues/278).

### Changed
- Reduce the background memory footprint (was caused by the constant presence of the code highlighting module).
- Disable server GC.
- Minor dependency update on the frontend and on the backend.

## [4.6.0] - 2024-07-01
### Changed
- Upgrade some frontend and backend dependencies.

## [4.5.1] - 2024-02-18
### Changed
- Upgrade some frontend dependencies.

## [4.5.0] - 2023-10-21
### Added
- Pages are now cached in memory.

## [4.4.0] - 2023-10-21
### Changed
- Translation tags no longer shown for the posts where there's no translations.

## [4.3.2] - 2023-09-10
### Changed
- Minor frontend library version updates.

## [4.3.1] - 2023-04-20
### Changed
- Docker images now pack less dependencies, thanks to @grosa1.

## [4.3.0] - 2023-02-26
### Changed
- Update to LiteDB v5.

  The old database will be updated automatically on the first run.

## [4.2.2] - 2022-12-25
### Changed
- Update several frontend libraries.

## [4.2.1] - 2022-10-31
### Fixed
- [#167: 404 on the Plans page](https://github.com/ForNeVeR/fornever.me/issues/167).

## [4.2.0] - 2022-10-30
### Changed
- The project runtime has been upgraded to .NET 6.
- The **Plans** page has migrated from the Freya web framework to the ASP.NET Core MVC framework.
- The **Plans** page's footer is synchronized with the rest of the site.

## [4.1.0] - 2022-10-22
### Changed
- Instead of Node.js, the application now uses [Jint](https://github.com/sebastienros/jint) embedded JavaScript engine. This removes a requirement of having Node.js installed in runtime.

## [4.0.0] - 2022-10-19
EvilPlanner project, which was a separate application (but always was a part of the fornever.me site structure), was merged into fornever.me. It used to have [a separate changelog](docs/CHANGELOG.EvilPlanner.md).

The main changes link for this version will show the changes relative to the previous version of fornever.me, and [here's the link](https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/1.0...v4.0.0) to see the changes relative to the previous version of EvilPlanner.

The changelog below will list all the changes since the previous releases of both applications.

### Added
- Localization for the **Plans** page.

### Fixed
- Links from the **Plans** page back to the main site were leading to nonexistent pages.

## [3.5.0] - 2021-05-02
### Changed
- Project updated to .NET 5.
- Project now has a runtime dependency on Node.js 16+.

## [3.4.0] - 2021-05-01
### Changed
- Node.js used in the container is updated to v8.10.0.

## [3.3.0] - 2021-04-10
### Changed
- Migrated to Markdig library for Markdown parsing.

### Added
- Table support for Markdown posts.

## [3.2.0] - 2021-02-12
### Changed
- Minor site layout changes: links to localized versions are now less confusing.

## [3.1.0] - 2018-06-02
### Changed
- Update to .NET Core 2.1

## [3.0.1] - 2017-11-18
No notable changes.

## [3.0.0] - 2017-11-09
### Added
- English localization.

## [2.1.7] - 2017-10-30
No notable changes.

## [2.1.6] - 2017-10-29
No notable changes.

## [2.1.5] - 2017-10-16
### Changed
- Migrate to .NET Core 2.0 SDK.

## [2.1.4] - 2017-09-23
No notable changes.

## [2.1.3] - 2017-09-21
No notable changes.

## [2.1.2] - 2017-09-20
### Changed
- RssSyndication library update after [a bug
  fix](https://github.com/shawnwildermuth/RssSyndication/issues/2).

## [2.1.1] - 2017-06-08
No notable changes.

## [2.1.0] - 2017-05-29
### Changed
- Source highlighting engine now uses only server-side JavaScript
  ([#58](https://github.com/ForNeVeR/fornever.me/issues/58)).

## [2.0.3] - 2017-05-15
No notable changes.

## [2.0.2] - 2017-05-10
### Security
- Fix for a vulnerability [Microsoft Security Advisory
  4021279](https://github.com/dotnet/announcements/issues/12).

## [2.0.1] - 2017-05-07
No notable changes.

## [2.0.0] - 2017-05-07
### Changed
- Migrate the application to .NET Core 1.1.

## [1.6.2] - 2017-05-01
No notable changes.

## [1.6.1] - 2017-04-21
No notable changes.

## [1.6.0] - 2017-03-30
### Changed
- Migrate to highlight.js instead of microlight.js.

## [1.5.5] - 2017-03-18
No notable changes.

## [1.5.4] - 2017-02-12
No notable changes.

## [1.5.3] - 2017-02-12
No notable changes.

## [1.5.2] - 2017-02-12
No notable changes.

## [1.5.1] - 2016-12-03
### Added
- `HEAD` HTTP method support
  ([#37](https://github.com/ForNeVeR/fornever.me/issues/37)).

## [1.5.0] - 2016-11-21
### Added
- The **Talks** page (projected into the repository as Git submodules).

## [1.4.7] - 2016-10-03
No notable changes.

## [1.4.6] - 2016-10-02 (13 minutes later)
No notable changes.

## [1.4.5] - 2016-10-02 (4 minutes later!)
No notable changes.

## [1.4.4] - 2016-10-02
No notable changes.

## [1.4.3] - 2016-09-14
No notable changes.

## [1.4.2] - 2016-09-12
No notable changes.

## [1.4.1] - 2016-08-20
No notable changes.

## [1.4.0] - 2016-08-13
### Added
- Custom site error pages.

## [1.3.2] - 2016-08-08
### Changed
- HTML5 is now used instead of XHTML 1.0 Strict.

## [1.3.1] - 2016-07-09
No notable changes.

## [1.3.0] - 2016-06-11
### Added
- Source code highlighting using the microlight.js library.

## [1.2.1] - 2016-06-11
No notable changes.

## [1.2.0] - 2016-06-04
No notable changes.

## [1.1.0] - 2015-12-27
No notable changes.

## [1.0.2] - 2015-12-26
### Added
- `Last-Modified` header support.

## [1.0.1] - 2015-11-09
### Removed
- The compatibility mode with old Disqus comments.

## [1.0.0] - 2015-11-08
### Changed
- Completely rewritten the blog in F#, using Freya web framework.

## [unversioned-4] - 2015-11-01
Last known released version using Hakyll. Contains only minor changes.

## [unversioned-3] - circa 2014-08-17
### Changed
- Completely rewritten the site in Haskell, using Hakyll static site generator.
- It is now a blog.

## [unversioned-2] - 2010-03-02
Last known released version using Django. Contains only minor changes. Was shut
down somewhere between this date and the next release.

## [unversioned-1] - circa 2010-01-04
Initial version of the site, written in Python using Django framework.

[unversioned-1]: https://github.com/ForNeVeR/fornever.me/commit/ed4797da69402f572794544a94202f5bc9b640d8
[unversioned-2]: https://github.com/ForNeVeR/fornever.me/compare/ed4797da69402f572794544a94202f5bc9b640d8...legacy
[unversioned-3]: https://github.com/ForNeVeR/fornever.me/tree/021a5f0d922a7218f9f10d2c2cb7e039a346a0a0
[unversioned-4]: https://github.com/ForNeVeR/fornever.me/compare/021a5f0d922a7218f9f10d2c2cb7e039a346a0a0...32a11d6c5fb5838a36b4c9b5824bf3eca262d0ed
[1.0.0]: https://github.com/ForNeVeR/fornever.me/compare/32a11d6c5fb5838a36b4c9b5824bf3eca262d0ed...1.0
[1.0.1]: https://github.com/ForNeVeR/fornever.me/compare/1.0...1.0.1
[1.0.2]: https://github.com/ForNeVeR/fornever.me/compare/1.0.1...1.0.2
[1.1.0]: https://github.com/ForNeVeR/fornever.me/compare/1.0.2...1.1
[1.2.0]: https://github.com/ForNeVeR/fornever.me/compare/1.1...1.2
[1.2.1]: https://github.com/ForNeVeR/fornever.me/compare/1.2...1.2.1
[1.3.0]: https://github.com/ForNeVeR/fornever.me/compare/1.2.1...1.3
[1.3.1]: https://github.com/ForNeVeR/fornever.me/compare/1.3...1.3.1
[1.3.2]: https://github.com/ForNeVeR/fornever.me/compare/1.3.1...1.3.2
[1.4.0]: https://github.com/ForNeVeR/fornever.me/compare/1.3.2...1.4.0
[1.4.1]: https://github.com/ForNeVeR/fornever.me/compare/1.4.0...1.4.1
[1.4.2]: https://github.com/ForNeVeR/fornever.me/compare/1.4.1...1.4.2
[1.4.3]: https://github.com/ForNeVeR/fornever.me/compare/1.4.2...1.4.3
[1.4.4]: https://github.com/ForNeVeR/fornever.me/compare/1.4.3...1.4.4
[1.4.5]: https://github.com/ForNeVeR/fornever.me/compare/1.4.4...1.4.5
[1.4.6]: https://github.com/ForNeVeR/fornever.me/compare/1.4.5...1.4.6
[1.4.7]: https://github.com/ForNeVeR/fornever.me/compare/1.4.6...1.4.7
[1.5.0]: https://github.com/ForNeVeR/fornever.me/compare/1.4.7...1.5.0
[1.5.1]: https://github.com/ForNeVeR/fornever.me/compare/1.5.0...1.5.1
[1.5.2]: https://github.com/ForNeVeR/fornever.me/compare/1.5.1...1.5.2
[1.5.3]: https://github.com/ForNeVeR/fornever.me/compare/1.5.2...1.5.3
[1.5.4]: https://github.com/ForNeVeR/fornever.me/compare/1.5.3...1.5.4
[1.5.5]: https://github.com/ForNeVeR/fornever.me/compare/1.5.4...1.5.5
[1.6.0]: https://github.com/ForNeVeR/fornever.me/compare/1.5.5...1.6.0
[1.6.1]: https://github.com/ForNeVeR/fornever.me/compare/1.6.0...1.6.1
[1.6.2]: https://github.com/ForNeVeR/fornever.me/compare/1.6.1...v1.6.2
[2.0.0]: https://github.com/ForNeVeR/fornever.me/compare/v1.6.2...2.0.0
[2.0.1]: https://github.com/ForNeVeR/fornever.me/compare/2.0.0...2.0.1
[2.0.2]: https://github.com/ForNeVeR/fornever.me/compare/2.0.1...2.0.2
[2.0.3]: https://github.com/ForNeVeR/fornever.me/compare/2.0.2...2.0.3
[2.1.0]: https://github.com/ForNeVeR/fornever.me/compare/2.0.3...2.1.0
[2.1.1]: https://github.com/ForNeVeR/fornever.me/compare/2.1.0...2.1.1
[2.1.2]: https://github.com/ForNeVeR/fornever.me/compare/2.1.1...2.1.2
[2.1.3]: https://github.com/ForNeVeR/fornever.me/compare/2.1.2...2.1.3
[2.1.4]: https://github.com/ForNeVeR/fornever.me/compare/2.1.3...2.1.4
[2.1.5]: https://github.com/ForNeVeR/fornever.me/compare/2.1.4...2.1.5
[2.1.6]: https://github.com/ForNeVeR/fornever.me/compare/2.1.5...2.1.6
[2.1.7]: https://github.com/ForNeVeR/fornever.me/compare/2.1.6...2.1.7
[3.0.0]: https://github.com/ForNeVeR/fornever.me/compare/2.1.7...3.0.0
[3.0.1]: https://github.com/ForNeVeR/fornever.me/compare/3.0.0...3.0.1
[3.1.0]: https://github.com/ForNeVeR/fornever.me/compare/3.0.1...3.1.0
[3.2.0]: https://github.com/ForNeVeR/fornever.me/compare/3.1.0...v3.2.0
[3.3.0]: https://github.com/ForNeVeR/fornever.me/compare/v3.2.0...v3.3.0
[3.4.0]: https://github.com/ForNeVeR/fornever.me/compare/v3.3.0...v3.4.0
[3.5.0]: https://github.com/ForNeVeR/fornever.me/compare/v3.4.0...v3.5.0
[4.0.0]: https://github.com/ForNeVeR/fornever.me/compare/v3.5.0...v4.0.0
[4.1.0]: https://github.com/ForNeVeR/fornever.me/compare/v4.0.0...v4.1.0
[4.2.0]: https://github.com/ForNeVeR/fornever.me/compare/v4.1.0...v4.2.0
[4.2.1]: https://github.com/ForNeVeR/fornever.me/compare/v4.2.0...v4.2.1
[4.2.2]: https://github.com/ForNeVeR/fornever.me/compare/v4.2.1...v4.2.2
[4.3.0]: https://github.com/ForNeVeR/fornever.me/compare/v4.2.2...v4.3.0
[4.3.1]: https://github.com/ForNeVeR/fornever.me/compare/v4.3.0...v4.3.1
[4.3.2]: https://github.com/ForNeVeR/fornever.me/compare/v4.3.1...v4.3.2
[4.4.0]: https://github.com/ForNeVeR/fornever.me/compare/v4.3.2...v4.4.0
[4.5.0]: https://github.com/ForNeVeR/fornever.me/compare/v4.4.0...v4.5.0
[4.5.1]: https://github.com/ForNeVeR/fornever.me/compare/v4.5.0...v4.5.1
[4.6.0]: https://github.com/ForNeVeR/fornever.me/compare/v4.5.1...v4.6.0
[4.6.1]: https://github.com/ForNeVeR/fornever.me/compare/v4.6.0...v4.6.1
[4.6.2]: https://github.com/ForNeVeR/fornever.me/compare/v4.6.1...v4.6.2
[4.6.3]: https://github.com/ForNeVeR/fornever.me/compare/v4.6.2...v4.6.3
[Unreleased]: https://github.com/ForNeVeR/fornever.me/compare/v4.6.3...HEAD
