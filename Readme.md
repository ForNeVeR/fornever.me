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
    msbuild EvilPlanner.sln /p:Platform="Any CPU" /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile="Production"

By default, EvilPlanner uses two databases on the LocalDB server:
- `EvilPlanner` for production
- `EvilPlannerStaging` for migration preparations and debug

Both of them will be created and migrated automatically if necessary.

EvilPlanner assumes that it will be deployed to https://fornever.me/plans/ in
production.

Change any settings through `Web.config` and `Web.Release.config` configuration
files.

### Database setup
It is recommended to use shared LocalDB instance as a database. To create the
instance and share it with an IIS user, first create the instance and make it
shared (from the administrative shell session):

    sqllocaldb create EvilPlanner
    sqllocaldb share EvilPlanner EvilPlanner

After that connect to the `(localdb)\.\EvilPlanner` SQL Server instance with
any compatible tool and execute the following script that will grant IIS user
the full access to the database:

    create login [IIS APPPOOL\EvilPlanner] from windows
    exec sp_addsrvrolemember N'IIS APPPOOL\EvilPlanner', sysadmin

### IIS Tuning
If you want to use LocalDB on IIS, you may need to have its configuration tuned
as [Krzysztof Kozielczyk explains in SQL Server Express WebLog][iis-localdb].

[iis-localdb]: http://blogs.msdn.com/b/sqlexpress/archive/2011/12/09/using-localdb-with-full-iis-part-1-user-profile.aspx
[mssql-localdb]: https://msdn.microsoft.com/ru-ru/library/hh510202(v=sql.120).aspx
