EvilPlanner Changelog
=====================

EvilPlanner project was deployed alongside the main fornever.me site, but initially was a separate application. Starting from v4.0.0, it is an integral part of fornever.me.

This file documents the changes made to the project before that merge. For further changes, see [the main changelog file][docs.fornever.me.changelog].

The format of this file is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

This file only documents changes in the site engine, not any changes in the content or the hosting infrastructure. In the past, some versions were created due to content-only or infrastructure-only changes. This is one of the reasons a version may be listed here as having "no notable changes".

## [1.0] - 2018-07-07
### Changed
- [#39: Migrate to .NET Core 2.1](https://github.com/ForNeVeR/EvilPlanner/issues/39).
- [#41: Migrated to the embedded LiteDB](https://github.com/ForNeVeR/EvilPlanner/issues/41).
- The application is now configured via an `appsettings.json` file.

## [0.1.13] - 2016-11-30
No notable changes.

## [0.1.12] - 2016-11-21
### Changed
- Site layout and CSS updated according to the changes on fornever.me.

## [0.1.11] - 2016-10-10
### Fixed
- The **Plans** link wasn't opening the main page properly.

## [0.1.10] - 2016-08-14
### Added
- Custom application-provided error pages are enabled.

## [0.1.9] - 2016-06-26
### Fixed
- [#43: Some quotes are duplicated in the database](https://github.com/ForNeVeR/EvilPlanner/issues/43).

## [0.1.8] - 2016-06-25
### Changed
- Update the CSS from fornever.me.

## [0.1.7] - 2016-06-25
No notable changes.

## [0.1.6] - 2015-11-08
### Fixed
- Links to the quote sources were broken.

## [0.1.5] - 2015-11-02
### Fixed
- [#35: Errors when fetching the quote for single-digit date](https://github.com/ForNeVeR/EvilPlanner/issues/35).

## [0.1.4] - 2015-10-15
### Added
- Automatic database migration on application startup.

## [0.1.3] - 2015-10-13
### Fixed
- A better fix for a race condition when several users request a new today's quote simultaneously.

## [0.1.2] - 2015-10-10
No notable changes.

## [0.1.1] - 2015-10-10
No notable changes.

## [0.1] - 2015-10-10
### Changed
- The backend now uses the Freya stack.
- The frontend is now written in TypeScript (using React) and loads the quote dynamically.

## [0.0.1] - 2015-10-09

The first release of the project. It is able to show a random quote from [the Evil Overlord List](https://legendspbem.angelfire.com/eviloverlordlist.html).

The application is written in F# using ASP.NET MVC 5, runs on IIS with .NET Framework 4.6, and uses Microsoft SQL Server 2014.

The site layout has been taken from the fornever.me version in Hakyll (see [the main changelog][docs.fornever.me.changelog] for details).

[docs.fornever.me.changelog]: ../CHANGELOG.md

[0.0.1]: https://github.com/ForNeVeR/fornever.me/tree/EvilPlanner/0.0.1
[0.1]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.0.1...EvilPlanner/0.1
[0.1.1]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.1...EvilPlanner/0.1.1
[0.1.2]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.1.1...EvilPlanner/0.1.2
[0.1.3]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.1.2...EvilPlanner/0.1.3
[0.1.4]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.1.3...EvilPlanner/v0.1.4
[0.1.5]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/v0.1.4...EvilPlanner/0.1.5
[0.1.6]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.1.5...EvilPlanner/0.1.6
[0.1.7]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.1.6...EvilPlanner/0.1.7
[0.1.8]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.1.7...EvilPlanner/0.1.8
[0.1.9]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.1.8...EvilPlanner/0.1.9
[0.1.10]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.1.9...EvilPlanner/0.1.10
[0.1.11]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.1.10...EvilPlanner/0.1.11
[0.1.12]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.1.11...EvilPlanner/0.1.12
[0.1.13]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.1.12...EvilPlanner/0.1.13
[1.0]: https://github.com/ForNeVeR/fornever.me/compare/EvilPlanner/0.1.13...EvilPlanner/1.0
