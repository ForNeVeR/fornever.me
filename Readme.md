EvilPlanner [![Status Umbra](https://img.shields.io/badge/status-umbra-red.svg)](https://github.com/ForNeVeR/andivionian-status-classifier)
===========
EvilPlanner is a nice statistical application.

Features
--------
Currently it will only output the random quote from Evil Overlord List (see
quotation source for every quote near the quote itself).

Installation
------------
Prepare [LocalDB instance of Microsoft SQL Server 2014][mssql-localdb] (may get
preinstalled with Visual Studio).

You may use publising from Visual Studio, or from msbuild (assuming that you
have both `nuget` and `msbuild` in your `PATH` environment variable):

    nuget restore
    msbuild EvilPlanner.sln /p:Platform="Any CPU" /p:DeployOnBuild=true /p:PublishProfile="Production"

By default, EvilPlanner assumes that it will have access to `EvilPlanner`
database on LocalDB instance (and it will create and migrate that if necessary)
and that it was deployed to https://fornever.me/plans/ (all these settings may
be changed in the `Web.config` file).

### IIS Tuning
If you want to use LocalDB on IIS, you may need to have its configuration tuned
as [Krzysztof Kozielczyk explains in SQL Server Express WebLog][iis-localdb].

[iis-localdb]: http://blogs.msdn.com/b/sqlexpress/archive/2011/12/09/using-localdb-with-full-iis-part-1-user-profile.aspx
[mssql-localdb]: https://msdn.microsoft.com/ru-ru/library/hh510202(v=sql.120).aspx
