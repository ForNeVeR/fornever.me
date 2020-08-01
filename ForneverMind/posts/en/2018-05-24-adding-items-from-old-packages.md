    title: Adding any files from old NuGet packages projects into .NET Core SDK-based MSBuild projects
    description: The post describes how to include content from old NuGet packages (created before new SDK) into new projects using PackageReference.
---

Not so long ago, while working on an open-source project, I had to solve a
problem: how to integrate an old C++/CLI NuGet package ([built in
2012][activiz.net]) that's only compatible only with the full .NET Framework,
and a project built using .NET Core SDK. While solving the issue, I've found
several peculiarities in MSBuild and NuGet integration, and was eventually able
to solve the problem.

The package in question contains several native DLLs. That means that my project
couldn't have any references to these DLLs, but I need these files in my output
directory (like `bin/Debug`) to run the project.

For example, there's a file `lib/net20/Cosmo.dll` that should be placed into the
same directory as `MyProject.exe` after the build.

The NuGet package in question works correctly with "old" .NET SDK, because in
the old days MSBuild was silently copying everything from the `lib/net20`
directory into the output directory. For the new SDK, package authors should use
[`contentFiles`][contentfiles] (and actually they should've used that for the
old SDK, too), although, obviously, it's unlikely they'll ever fix their C++/CLI
package to be compatible with the _.NET Core_ SDK: it isn't in fact compatible
with .NET Core at all, so why bother?

Fortunately, MSBuild is flexible enough to allow us to fix the issue by writing
a bit of additional XML. We'll have to manually create a package file list (note
that wildcards are supported), and make MSBuild to copy these files to the
output directory.

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

This sample shows how to get the package content path (via the
`NuGetPackageRoot` builtin variable) and how to add that package content into
your project (optionally excluding some items with `<Content Remove="…"/>`).

Using this mechanic, you can create a .NET Core SDK project that will use any
old NuGet packages that used to rely on this MSBuild + NuGet quirk.

[activiz.net]: https://www.nuget.org/packages/Activiz.NET.x64/
[contentfiles]: https://blog.nuget.org/20160126/nuget-contentFiles-demystified.html
