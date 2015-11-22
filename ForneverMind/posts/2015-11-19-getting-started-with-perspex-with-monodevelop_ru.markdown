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

![Снимок окна Add-In Manager](images/2015-11-22-monodevelop-add-in-manager.png)

Теперь создадим новый пустой проект, и добавить в него пакет при помощи
соответствующего пункта в дереве проекта (не забудьте поставить галочку "Show
pre-release packages", т.к. пакет Perspex пока что не является стабильным).

![Снимок окна Add Packages](images/2015-11-22-monodevelop-add-packages.png)

Через некоторое время пакет будет установлен. Достаточно приятно и неожиданно,
что MonoDevelop, в отличие от Visual Studio, умеет устанавливать пакеты в
фоновом режиме и совершенно не тормозить при этом.

[habrahabr-perspex]: http://habrahabr.ru/post/267425/
[perspex]: https://github.com/Perspex/Perspex
[perspex-gitter]: https://gitter.im/Perspex/Perspex
[perspex-visual-studio-plugin]: https://visualstudiogallery.msdn.microsoft.com/a4542e8a-b56c-4295-8df1-7e220178b873
[perspex-xamarin-studio]: https://github.com/Perspex/Perspex/blob/master/docs/gettingstarted.md#osx--linux
