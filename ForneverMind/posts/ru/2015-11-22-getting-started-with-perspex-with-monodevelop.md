    title: Начало работы с Perspex с использованием MonoDevelop
    description: Недостающая инструкция по созданию проекта, использующего GUI Framework Perspex, в среде разработки MonoDevelop.
---

Не так давно на Хабрахабре [анонсировали][habrahabr-perspex] новую
кроссплатформенную библиотеку, которая позволяет разрабатывать UI с
использованием языка разметки XAML (и, соответственно, стека CLI, основными
реализациями которого являются .NET и Mono), и при этом работает под всеми
основными операционными системами: [Perspex][perspex]. Само собой, я сразу
заинтересовался этим фактом, и решил проверить, насколько хороша поддержка
используемых мной операционных систем Windows и Linux.

С поддержкой Windows никаких проблем не возникло: есть работающее и
развивающееся [расширение для Visual Studio][perspex-visual-studio-plugin],
интеграция и установка из Nuget проходит без особенных проблем.

Для известных мне кроссплатформенных IDE SharpDevelop, MonoDevelop и Xamarin
Studio на текущий момент подобных расширений не разработано (хотя на [канале
библиотеки в gitter][perspex-gitter] и поговаривали, что собираются его
разработать или даже уже начали). Есть [официальная
документация][perspex-xamarin-studio] по работе с Xamarin Studio, однако, к
моему сожалению, в репозитории пакетов используемой мной операционной системы
NixOS этой среды не оказалось. Зато есть MonoDevelop, который, в общем-то, тоже
должен поддерживаться, учитывая особенности Perspex. Ну вот и попробуем!

Я проводил данные эксперименты на MonoDevelop версии 5.9.4.

В первую очередь следует убедиться, что установлен модуль MonoDevelop для работы
с NuGet. Проверить это можно в окошке Tools > Add-In Manager. Если данный модуль
не установлен — его следует установить, он должен быть доступен в стандартном
репозитории MonoDevelop.

![Снимок окна Add-In Manager](../images/2015-11-22-monodevelop-add-in-manager.png)

Теперь создадим новый пустой проект, и добавить в него пакет при помощи
соответствующего пункта в дереве проекта (не забудьте поставить галочку "Show
pre-release packages", т.к. пакет Perspex пока что не является стабильным).

![Снимок окна Add Packages](../images/2015-11-22-monodevelop-add-packages.png)

Через некоторое время пакет будет установлен. Достаточно приятно и неожиданно,
что MonoDevelop, в отличие от Visual Studio, умеет устанавливать пакеты в
фоновом режиме и совершенно не тормозить при этом.

Вот и настало время написать немного кода! Здесь я не буду отступать от
стандартного шаблона проекта, который включён в состав плагина Perspex для
Visual Studio: речь в данной статье идёт не о правилах разработки приложений, а
скорее о настройке среды для этой разработки.

Итак, создадим в проекте файл `Program.cs`:

```csharp
namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App();
            var window = new MainWindow();
            window.Show();
            app.Run(window);
        }
    }
}
```

Код самого Perspex-приложения разместим в файле `App.cs`:

```csharp
using System;

using Perspex;
using Perspex.Controls;
using Perspex.Themes.Default;

namespace TestProject
{
    class App : Application
    {
        public App()
        {
            RegisterServices();
            InitializeSubsystems((int)Environment.OSVersion.Platform);
            Styles = new DefaultTheme();
        }
    }
}
```

А также напишем разметку и код для главного окна приложения. `MainWindow.paml`:

```xml
<Window xmlns="https://github.com/perspex">
    <TextBlock>Hello world!</TextBlock>
</Window>
```

И `MainWindow.paml.cs`:

```csharp
using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace TestProject
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
```

Важно не забыть поставить файлу `MainWindow.paml` тип сборочного действия
`EmbeddedResource` и указать корректный Resource ID (который должен совпадать с
названием и пространством имён соответствующего окну класса).

![Снимок панели свойств файла
MainWindow.xaml](../images/2015-11-22-monodevelop-resource-properties.png)

Также не забудьте добавить ссылку на стандартную сборку `System`: почему-то моя
версия MonoDevelop не делает этого самостоятельно, хотя это и было бы довольно
логично.

Казалось бы — всё, можно запускать приложение, теперь всё заработает! Но нет,
это ещё не всё. Следует вручную скопировать сборки `Perspex.Cairo` и
`Perspex.Gtk` в выходной каталог (можете это как-то автоматизировать), без этого
проект не запустится. Надеюсь, в следующих версиях Perspex эту проблему учтут и
что-нибудь придумают. Для этого я выполнил следующую команду:

    $ cp packages/Perspex.0.1.1-alpha2/build/net45/Perspex.Cairo.dll TestProject/bin/Debug/
    $ cp packages/Perspex.0.1.1-alpha2/build/net45/Perspex.Gtk.dll TestProject/bin/Debug/

Теперь уже можно нажать в MonoDevelop кнопочку Debug и насладиться, э,
прекрасным видом нашего приложения.

![Снимок окна запущенного приложения](../images/2015-11-22-perspex-application.png)

На этом пока что всё, наслаждайтесь кроссплатформенным пользовательским
интерфейсом нового поколения :)

[habrahabr-perspex]: http://habrahabr.ru/post/267425/
[perspex]: https://github.com/Perspex/Perspex
[perspex-gitter]: https://gitter.im/Perspex/Perspex
[perspex-visual-studio-plugin]: https://visualstudiogallery.msdn.microsoft.com/a4542e8a-b56c-4295-8df1-7e220178b873
[perspex-xamarin-studio]: https://github.com/Perspex/Perspex/blob/master/docs/gettingstarted.md#osx--linux
