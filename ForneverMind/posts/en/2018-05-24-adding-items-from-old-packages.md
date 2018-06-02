    title: Adding any files from old NuGet packages projects into .NET Core SDK-based MSBuild projects
    description: The post describes how to include content from old NuGet packages (created before new SDK) into new projects using PackageReference.
---

Not so long ago, while working on an open-source project, I had to solve an
problem: how to integrate an old C++/CLI NuGet package ([built in
2012][activiz.net]) that's compatible only with full .NET Framework with the
project built using .NET Core SDK. While solving the issue I encountered some
interesting historical and engineering facts about MSBuild and NuGet, and
filanny I've solved the problem.

The package in question includes multiple native DLLs. That means that my
project couldn't have any references to these DLLs, but I need these files in my
output directory (like `bin/Debug`) to run the project.

For example, there's a file `lib/net20/Cosmo.dll` and it should be placed into
the same directory `MyProject.exe` is placed after the build.

This package works correctly with "old" .NET SDK, because for old SDK, MSBuild
was copying everything from the `lib/net20` directory into the output directory.
For new SDK (and actually for old SDK, too) package authors should've used
[`contentFiles`][contentfiles], although, obviously, it's unlikely they will fix
their C++/CLI package to be compoatible with _.NET Core_ SDK: it's not in fact
compatible with .NET Core at all, so why bother?

Fortunately, MSBuild is flexible enough to fix the issue from our side by
writing some additional XML. We'll create a package file list manually (note
that wildcards are supported) and make MSBuild to copy these files for us.

The following code should be included into the `<Project>` element in your
`.csproj`:

```xml
<PropertyGroup>
    <ActivizNetPackage>Activiz.NET.x64</ActivizNetPackage>
    <ActivizNetVersion>5.8.0</ActivizNetVersion>
    <ActivizNetPackagePath>$(NuGetPackageRoot)\$(ActivizNetPackage)\$(ActivizNetVersion)</ActivizNetPackagePath>
    <ActivizNetContents>$(ActivizNetPackagePath)\lib\net20\*.dll</ActivizNetContents>
    <ActivizNetExclude>$(ActivizNetPackagePath)\lib\net20\msvc?90.dll</ActivizNetExclude>
</PropertyGroup>
<ItemGroup>
<PackageReference Include="$(ActivizNetPackage)" Version="$(ActivizNetVersion)"/>
    <Content CopyToOutputDirectory="Always" Include="$(ActivizNetContents)">
        <Visible>false</Visible>
    </Content>
    <Content Remove="$(ActivizNetExclude)" />
</ItemGroup>
```

This sample shows how to get package content path (via the `NuGetPackageRoot`
builtin variable) and how to add package content into your project (optionally
excluding some items with `<Content Remove="…"/>`).

Using this mechanic, you can create a .NET Core SDK project that will use any
old NuGet packages relied on this MSBuild+NuGet quirk.

[activiz.net]: https://www.nuget.org/packages/Activiz.NET.x64/
[contentfiles]: https://blog.nuget.org/20160126/nuget-contentFiles-demystified.html
