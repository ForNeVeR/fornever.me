    title: Использование AvaloniaUI из PascalABC.NET
    description: Компиляция проекта на языке программирования PascalABC.NET с использованием UI-фреймворка AvaloniaUI.
---

Сегодня мы обсудим совершенно неожиданную вещь: реализацию пользовательского
интерфейса для программы на CLI-совместимом языке программирования
[PascalABC.NET][pascalabc-net] с использованием современного UI-фреймворка
AvaloniaUI, [который недавно обновился до версии 0.5][avaloniaui/0.5].

[AvaloniaUI][avaloniaui] — это кроссплатформенный UI-фреймворк с открытым
исходным кодом, который использует XAML для описания пользовательского
интерфейса. Чем-то похож на WPF, но с открытым кодом и немного другой.

[PascalABC.NET][pascalabc-net] — это не очень известный язык программирования
под платформу CLI, предназначенный прежде всего для обучения. Код компилятора
также открыт под свободной лицензией.

*Почему бы нам их не скрестить?*

В данной статье мы рассмотрим основные сложности, которые приходится решать
при написании программ, использующих AvaloniaUI, на языке PascalABC.NET.

Компиляция PascalABC.NET
------------------------

У PascalABC.NET есть [своя графическая IDE][pascalabc-net/screenshots], но я
предпочитаю пользоваться своими средствами разработки, не будучи привязанным к
её довольно скромным возможностям. Поэтому я пользуюсь консольным компилятором
языка. Скачать его можно с [соответствующего раздела
сайта][pascalabc-net/download]; нас интересует файл `PABCNETC.ZIP`. Распакуем
его в каталог `compiler`.

Для компиляции проекта мы не будем пользоваться никакими вспомогательными
средствами, а будем передавать ему непосредственно путь к основному модулю
программы:

```console
$ compiler/pabcnetcclear.exe Application.pas
```

Все вспомогательные модули и ресурсы компилятор будет находить на
основании этого файла, так уж тут заведено.

Скачивание пакетов из NuGet
---------------------------

При разработке для .NET программисты часто пользуются репозиторием библиотек для
этой платформы, [NuGet][nuget]. AvaloniaUI выложена именно на NuGet, так что нам
придётся также пользоваться этим репозиторием. Я использую для работы с этим
хранилищем программу [Paket][paket], написанную на F#. Её следует установить в
каталог `.paket` рядом с исходным кодом программы; вызывать её мы будем из этого
каталога.

Для работы этой программы необходимо составить список зависимостей и сохранить в
текстовый файл `paket.dependencies`. Запишем туда пакеты `Avalonia` и
`Avalonia.Desktop`:

```
source https://www.nuget.org/api/v2
nuget Avalonia ~> 0.5.0
nuget Avalonia.Desktop ~> 0.5.0
```

После этого можно установить зависимости:

```console
$ .paket/paket.exe install
```

Настройка компилятора для использования зависимостей
----------------------------------------------------

На этом этапе становится ясно, что PascalABC.NET — это всё-таки учебный язык, и
авторы не ожидали, что кто-то попытается использовать его для "серьёзной"
разработки. Из-за своей модели компиляции компилятор PascalABC.NET с очень
большим трудом использует внешние зависимости, и в частности — пакеты NuGet.

Дело в том, что для компиляции программы компилятор загружает все библиотеки в
своё основное адресное пространство, а для этого ему нужно строго разрешить все
зависимости между библиотеками. Это создаёт две проблемы:

1. Загруженные из интернета библиотеки размещены в подкаталогах каталога
   `packages`, а компилятор не ожидает их там увидеть.
2. Библиотеки в мире .NET ссылаются друг на друга по строгой версии сборки, и
   ничего не знают о _совместимых версиях_. Например, если у нас есть библиотека
   `Avalonia`, ссылающаяся на библиотеку `System.Reactive.Core` версии
   `3.0.0.0`, а у нас скачалась библиотека более новой, но совместимой версии
   `3.0.1000.0` — загрузчик компилятора просто упадёт.

   Обычно это при запуске скомпилированной программы решается файлом
   `app.config`, а при компиляции решается тем, что компиляторы управляемых
   языков не загружают библиотеки этим способом.

Однако же, компилятор PascalABC.NET написан именно так, как написан, и эти
проблемы нам придётся как-то решать. К счастью, они решаемы, если немного
поиграться с конфигурацией компилятора.

Для решения проблемы с путями мы скопируем все библиотеки в один предсказуемый
каталог `lib`, и будем ссылаться на него из кода программы. Я делаю это таким
скриптом для PowerShell, `prepare-libraries.ps1`:

```powershell
param (
    $LibraryDirectory = "$PSScriptRoot/lib"
)

Write-Output 'Preparing library packages…'

$libraries = @(
    "$PSScriptRoot/packages/Avalonia/lib/net45/*"
    "$PSScriptRoot/packages/Avalonia.Direct2D1/lib/net45/*"
    "$PSScriptRoot/packages/Avalonia.Win32/lib/net45/*"
    "$PSScriptRoot/packages/SharpDX/lib/net45/*"
    "$PSScriptRoot/packages/SharpDX.Direct2D1/lib/net45/*"
    "$PSScriptRoot/packages/SharpDX.Direct3D11/lib/net45/*"
    "$PSScriptRoot/packages/SharpDX.DXGI/lib/net45/*"
    "$PSScriptRoot/packages/Sprache/lib/net40/*"
    "$PSScriptRoot/packages/System.Reactive.Core/lib/net45/*"
    "$PSScriptRoot/packages/System.Reactive.Interfaces/lib/net45/*"
    "$PSScriptRoot/packages/System.Reactive.Linq/lib/net45/*"
)

if (-not (Test-Path $LibraryDirectory)) {
    $null = New-Item -ItemType Directory $LibraryDirectory
}

Copy-Item $libraries $LibraryDirectory
```

Теперь нам нужно пропатчить конфигурационный файл компилятора. Вот
конфигурационный файл `app.config` нашего приложения:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="System.Reactive.Interfaces" publicKeyToken="94bc3704cddfc263" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-3.0.1000.0" newVersion="3.0.1000.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Reactive.Core" publicKeyToken="94bc3704cddfc263" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-3.0.1000.0" newVersion="3.0.1000.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Reactive.Linq" publicKeyToken="94bc3704cddfc263" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-3.0.1000.0" newVersion="3.0.1000.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="SharpDX" publicKeyToken="b4dcf0f35e5521f1" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-3.1.1.0" newVersion="3.1.1.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="SharpDX.Direct2D1" publicKeyToken="b4dcf0f35e5521f1" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-3.1.1.0" newVersion="3.1.1.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>
```

Просто скопируем его по пути `compiler/pabcnetcclear.exe.config`, чтобы он же
использовался и компилятором при поиске сборок.

Главный модуль приложения
-------------------------

Главный модуль приложения, `Application.pas`, будет выполнять следующие задачи:

- добавит ссылки на библиотеки
- добавит основной файл ресурсов
- определит главный класс
- инициализирует AvaloniaUI

Вот как это выглядит:

```pascal
{$reference libs/Avalonia.Controls.dll}
{$reference libs/Avalonia.Direct2D1.dll}
{$reference libs/Avalonia.DotNetFrameworkRuntime.dll}
{$reference libs/Avalonia.Markup.Xaml.dll}
{$reference libs/Avalonia.Win32.dll}

{$resource Application.App.xaml}

uses
    Avalonia,
    Avalonia.Controls,
    Avalonia.Markup.Xaml,

    MainWindowUnit;

type
    App = class(Application)
    public
        procedure Initialize; override;
        begin;
            AvaloniaXamlLoader.Load(Self);
        end;
    end;
begin
    AppBuilder.Configure&<App>()
        .UseWin32()
        .UseDirect2D1()
        .Start&<MainWindow>(nil);
end.

```

Здесь важно не запутаться с названиями ресурсов (AvaloniaUI по умолчанию ожидает
определённого именования XAML-файлов для того, чтобы их правильно находить).

Вот так выглядит стандартный файл `Application.App.xaml`, который должен быть
использован в качестве ресурса:

```xml
<Application xmlns="https://github.com/avaloniaui">
  <Application.Styles>
    <StyleInclude Source="resm:Avalonia.Themes.Default.DefaultTheme.xaml?assembly=Avalonia.Themes.Default"/>
    <StyleInclude Source="resm:Avalonia.Themes.Default.Accents.BaseLight.xaml?assembly=Avalonia.Themes.Default"/>
  </Application.Styles>
</Application>
```

Модуль главного окна приложения
-------------------------------

В качестве главного окна используем стандартную заглушку с сообщением "Hello,
world!". Вот модуль `MainWindowUnit.pas` на языке PascalABC.NET:

```pascal
unit MainWindowUnit;
{$resource MainWindowUnit.MainWindow.xaml}

uses
    Avalonia.Controls,
    Avalonia.Markup.Xaml;

type
    MainWindow = class(Window)
    public
        constructor;
        begin;
            AvaloniaXamlLoader.Load(Self);
        end;
    end;
end.
```

А вот соответствующий файл ресурса `MainWindowUnit.MainWindow.xaml`:

```xml
<Window xmlns="https://github.com/avaloniaui" MinWidth="500" MinHeight="300">
    <TextBlock>Hello world!</TextBlock>
</Window>
```

Сборка приложения
-----------------

Помимо, собственно, компиляции, при сборке нам ещё потребуется скопировать
приложение и библиотеки в выходной каталог. Я использую для этого следующий
скрипт, `build.ps1`:

```powershell
compiler/pabcnetcclear.exe Application.pas
if ($?) {
    if (-not (Test-Path bin)) {
        New-Item -Type Directory bin
    }

    Copy-Item Application.exe bin
    Copy-Item 'libs/*' bin
    Copy-Item app.config bin/Application.exe.config
}
```

Теперь можно запустить приложение и увидеть окно программы:

```console
$ bin/Application.exe
```

![Окно приложения AvaloniaUI на PascalABC.NET][screenshot]

Для удобства читателя исходный код тестового приложения со всеми скриптами, но
без бинарников, [опубликован на GitHub][github].

[screenshot]: ../images/2017-05-15-avaloniaui-pascalabc-net.png

[avaloniaui]: https://github.com/AvaloniaUI/Avalonia
[avaloniaui/0.5]: https://habrahabr.ru/post/328684/
[github]: https://github.com/ForNeVeR/avalonia-pascalabc-net-showcase
[nuget]: https://www.nuget.org/
[paket]: https://fsprojects.github.io/Paket/
[pascalabc-net]: http://pascalabc.net/
[pascalabc-net/download]: http://pascalabc.net/ssyilki-dlya-skachivaniya
[pascalabc-net/screenshots]: http://pascalabc.net/skrinshoti
