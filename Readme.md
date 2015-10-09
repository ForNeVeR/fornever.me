EvilPlanner [![Status Ventis](https://img.shields.io/badge/status-ventis-yellow.svg)](https://github.com/ForNeVeR/andivionian-status-classifier)
===========
EvilPlanner is a nice statistical application.

Features
--------
Currently it will only output the random quote from Evil Overlord List (see
quotation source for every quote near the quote itself).

Database Setup
--------------
EvilPlanner uses MS SQL for data storage. It will migrate its databases
automatically if necessary.

### Development
[LocalDB instance of Microsoft SQL Server 2014][mssql-localdb] is recommended
for the development purposes (and it even may be already installed on your
machine with Visual Studio).

By default EvilPlanner will use the default shared instance named
`MSSQLLocalDB`. It will create `EvilPlannerStaging` database there for
debug and development purposes.

### Production
It's recommended to use [MS SQL 2014 Express][mssql-express] for production. By
default EvilPlanner will use `EvilPlanner` database on the `.\SQLEXPRESS`
server.

Connect to the target MS SQL Server instance with any compatible tool and
execute the following script to create the database:

    create database EvilPlanner
    go

If you're going to deploy application on IIS (as recommended), you could grant
IIS user the access to the database with the following script:

    use EvilPlanner
    create login [IIS APPPOOL\EvilPlanner] from windows
    exec sp_addsrvrolemember N'IIS APPPOOL\EvilPlanner', sysadmin

Configuration
-------------
EvilPlanner uses [Web.config transformations][web-config-transform], so there
is a basic `Web.config` file used for debug and `Web.Release.config` used for
publishing. Be sure to make changes to an appropriate file!

Most likely, you'll want to change `connectionStrings` section to match your
local database installation, or `BaseUrl` key of the `appSettings`section to
change the application base URL (you may omit `BaseUrl` entirely, then
EvilPlanner will not make assumptions about its deploy location and will use
ASP.NET defaults).

By default production version of EvilPlanner assumes that it will be deployed
to https://fornever.me/plans/.

Build and Deploy
------------
You may compile and publish the code from Visual Studio, or using `msbuild`
(assuming that you have both `nuget` and `msbuild` in your `PATH` environment
variable):

    nuget restore
    msbuild EvilPlanner.sln /p:Platform="Any CPU" /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile="Production"

Creating Database Migration
---------------------------
When developing, you'll often want to migrate your staging database.
Unfortunately, there's a bug in Visual Studio that could prevent you from doing
that in the standard way. So, if you want to create new migration, follow this
procedure:

1. Set current project in Visual Studio to `EvilPlanner.Data`.
2. Open Package Manager Console.
3. Select `EvilPlanner.Data` as current project in Package Manager Console.
4. As usual: `Add-Migration ...`, `Update-Database` etc.

Note that it will use the connection string from an `App.config` file from the
`EvilPlanner.Data`, not from the `Web.config` file.

[mssql-express]: https://www.microsoft.com/en-US/download/details.aspx?id=42299
[mssql-localdb]: https://msdn.microsoft.com/ru-ru/library/hh510202(v=sql.120).aspx
[web-config-transform]: http://www.asp.net/mvc/overview/deployment/visual-studio-web-deployment/web-config-transformations
