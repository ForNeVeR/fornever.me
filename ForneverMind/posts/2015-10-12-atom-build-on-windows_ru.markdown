---
title: Компиляция Atom под Windows
description: Небольшой сборник советов по компиляции редактора Atom.
---

Сегодня я решил скомпилировать редактор [atom][] под Windows. Поскольку пришлось
внести в процесс, [прекрасно описанный в документации][build-documentation],
некоторые изменения, решил написать вот этот небольшой пост.

Итак, после установки зависимостей ([Node.js][node-js], [Python][python],
[Visual Studio 2015][visual-studio-2015]) --- впрочем, они у меня в системе
обычно уже установлены, так что мне ничего делать не приходится --- следует
склонировать репозиторий:

    git clone git@github.com:atom/atom.git

Теперь один немаловажный момент: для компиляции в Visual Studio 2015 следует
установить соответствующую переменную окружения, без неё `node-gyp` может
ругнуться:

    $env:GYP_MSVS_VERSION = 2015

И теперь уже можно переходить непосредственно к процессу сборки (на всякий
случай я переместил каталоги сборки и инсталляции в отдельно указываемое место):

    md T:\Temp\atom-build
    md T:\Temp\atom
    .\script\build.cmd --build-dir T:\Temp\atom-build --install-dir T:\Temp\atom

На этом всё! Через пару десятков минут Atom скомпилируется и вы станете
обладателем собственноручно собранной копии расширяемого текстового редактора :)

[atom]: https://github.com/atom/atom
[build-documentation]: https://github.com/atom/atom/blob/master/docs/build-instructions/windows.md
[node-js]: https://nodejs.org/
[python]: https://www.python.org/
[visual-studio-2015]: https://www.visualstudio.com/vs-2015-product-editions
