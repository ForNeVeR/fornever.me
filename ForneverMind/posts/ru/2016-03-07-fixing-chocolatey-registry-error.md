    title: Исправление ошибки приведения Get-ItemProperty при установке пакетов из Chocolatey
    description: Способ диагностирования и исправления ошибки Get-ItemProperty : Specified cast is not valid.
---

При установке [очередной версии pandoc][chocolatey-pandoc] из Chocolatey я
сегодня столкнулся с довольно странной ошибкой, которая выглядела так:

```
PS> choco install pandoc
pandoc v1.16.0.2
Installing pandoc...
pandoc has been installed.
Get-ItemProperty : Specified cast is not valid.
At C:\ProgramData\chocolatey\lib\pandoc\tools\chocolateyInstall.ps1:18 char:8
+ $key = Get-ItemProperty -Path @($machine_key6432, $machine_key, $local_key) -Err ...
+ ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
+ CategoryInfo : NotSpecified: (:) [Get-ItemProperty], InvalidCastException
+ FullyQualifiedErrorId : System.InvalidCastException,Microsoft.PowerShell.Commands.GetItemPropertyComma ...
The install of pandoc was NOT successful.
```

Я пошёл жаловаться в комментарии, и обнаружил, что такую проблему почти никто не
встречал, но на аналогичную ошибку жалуются и в описании других пакетов —
например, [copyq][chocolatey-copyq-error]. Пришлось исследовать её самому.

Итак, что же тут происходит? Видно, что упала вот такая строка:

```
$key = Get-ItemProperty -Path @($machine_key6432, $machine_key, $local_key)
```

В обсуждении один из мейнтейнеров [говорит][statement-about-template], что эта
строка — часть стандартного шаблона пакета Chocolatey, так что ошибка вполне
может встретиться и в других пакетах.

Эта строка всего лишь обращается к системному реестру, и перечисляет записи трёх
разделов:

```
$local_key = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*'
$machine_key = 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\*'
$machine_key6432 = 'HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*'
```

Эти разделы реестра отвечают за деинсталляцию программ в Windows, и, очевидно,
что-то в них очень сильно не нравится PowerShell. Если попытаться выполнить на
них команду `Get-ItemProperty` вручную, то она тоже в какой-то момент упадёт. В
чём же дело?

Я написал простой диагностический скрипт, который выводит все подряд названия
подразделов, которые прочитались без ошибки:

```
$local_key = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*'
$machine_key = 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\*'
$machine_key6432 = 'HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*'
$items = Get-ChildItem @($local_key, $machine_key, $machine_key6432)
$ErrorActionPreference = 'Stop'

foreach ($item in $items) {
    Write-Output $item.PSPath
    $null = Get-ItemProperty $item.PSPath
}
```

Последнее название раздела, которое выводит этот скрипт, будет содержать ошибку.
В моём случае ошибку содержал раздел
`HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\nbi-glassfish-mod-3.1.2.23.2`.
Просмотрев этот ключ с помощью `regedit`, я обнаружил, что параметр `NoModify`
содержит "некорректное значение DWORD". Удаление ключа `NoModify` исправило
проблему, и нет оснований полагать, что оно может существенно испортить процесс
деинсталляции вовлечённой программы, так что я рекомендую такой способ решения.

Как оказалось, это [известная проблема][netbeans-bugzilla] некоего
инсталляционного пакета от NetBeans, которая также затронула и дистрибутив
GlassFish — в реестре действительно создаётся некорректное значение.

После удаления этого испорченного ключа проблема была исправлена и я смог
установить пакет через Chocolatey.

[chocolatey-copyq-error]: https://chocolatey.org/packages/copyq#comment-2438623396
[chocolatey-pandoc]: https://chocolatey.org/packages/pandoc
[netbeans-bugzilla]: https://netbeans.org/bugzilla/show_bug.cgi?id=251538
[statement-about-template]: https://chocolatey.org/packages/copyq#comment-2440908951
