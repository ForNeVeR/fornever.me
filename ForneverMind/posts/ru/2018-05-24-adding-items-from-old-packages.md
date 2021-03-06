    title: Включение в проекты, собираемые с помощью .NET Core SDK, файлов из старых пакетов
    description: Включаем в проекты с PackageReference файлов из некоторых NuGet-пакетов, которые создавались без учёта особенностей нового SDK.
---

Не так давно мне при работе над одним из проектов с открытым кодом пришлось
решать довольно нетипичную задачу: нужно было интегрировать очень старый
NuGet-пакет ([2012 год, как-никак][activiz.net]), написанный на C++/CLI и
адаптированный, понятное дело, только для большого .NET, в сборку, реализованную
на базе .NET Core SDK. При этом я столкнулся с проблемами, которые сподвигли
меня исследовать некоторые доселе неизвестные особенности работы MSBuild с
NuGet-пакетами, и найти решение этих проблем в рамках современных технологий.

Рассматриваемый пакет включает в себя несколько нативных DLL, которые требуются
для его работы. Поскольку они нативные, ссылок на них в .NET-проекте поставить
нельзя. Но после сборки они должны быть размещены в выходном каталоге, рядом с
собранным проектом, который использует пакет (то есть в привычном нам
`bin/Debug` или подобном каталоге).

Например, в пакете есть файл `lib/net20/Cosmo.dll`, и предполагается, что этот
пакет будет размещён рядом с основными файлами после компиляции.

На старом SDK это работает, потому что MSBuild копирует всё подряд из
соответствующего каталога `lib/net20/` в выходной каталог. А вот на новом SDK
это работать перестаёт, потому что [нужно использовать
`contentFiles`][contentfiles]. Файлы в выходной каталог из этого старого пакета
больше не копируются, а новую версию пакета нам вряд ли кто-то сделает (потому
что пакет на C++/CLI из 2012 года совершенно никому не интересно портировать для
.NET Core SDK — он же вообще не совместим с .NET Core).

К счастью, MSBuild — достаточно гибкая система, и позволяет эту проблему решить,
написав немножко дополнительного XML. Мы вручную сформируем список файлов для
копирования из пакета, и заставим MSBuild их за нас копировать.

Следующий код можно вставить внутрь элемента `<Project>` в `.csproj`-файле.

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

Этот пример показывает, как можно получить путь к содержимому пакета,
распакованному на диске после его установки (через переменную
`NuGetPackageRoot`), а также добавить файлы из пакета в проект (опционально
убрав те из них, которые копировать не требуется, с помощью атрибута `Remove`
элемента `Content`).

Таким образом можно получить проект на новом SDK, который будет использовать
старые пакеты, которые полагались на обсуждаемый аспект поведения связки NuGet +
MSBuild.

[activiz.net]: https://www.nuget.org/packages/Activiz.NET.x64/
[contentfiles]: https://blog.nuget.org/20160126/nuget-contentFiles-demystified.html
