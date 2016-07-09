    title: Вызов Microsoft Word из Windows-сервиса
    description: Решение проблем, связанных с задействованием COM-объектов Microsoft Word из контекста Windows-сервиса
---

Сегодня мы будем решать неожиданную задачу неожиданным способом. Мне было нужно
сконвертировать PDF в DOCX на билд-сервере Jenkins, который выполняется как
Windows-сервис.

Опытным способом было установлено, что встроенный в Microsoft Word 2013
конвертер из PDF в DOCX работает неплохо, и осталось "всего лишь" найти способ
его задействовать.

Взаимодействовать с Word мы будем при помощи COM. Вот простой PowerShell-скрипт,
который сохраняет переданный ему PDF-файл в DOCX с использованием COM API,
поставляемого с Word.

```powershell
param (
    [string]$InputPath
)

$ErrorActionPreference = 'Stop'

$wdFormatDocumentDefault = 16
$InputPath = Resolve-Path $InputPath
$outputPath = [IO.Path]::ChangeExtension($InputPath, 'docx')

Write-Output "Saving $InputPath to $outputPath..."
$word = New-Object -ComObject 'Word.Application'
try {
    $word.Visible = $false
    $document = $word.Documents.Open($InputPath)
    $document.SaveAs($outputPath, $wdFormatDocumentDefault)
    $document.Close()
} finally {
    $word.Quit()
}
```

Однако, как всегда, есть небольшая проблемка. Если мы запускаем Jenkins как
Windows-сервис под обычным сервисным аккаунтом, то работать с Word он не сможет.
Судя по дереву процессов, во время вызова этого кода Word запускается и начинает
уверенно пожирать системную память. Однако же, вызов
`$word.Documents.Open($InputPath)` возвращает `$null`, а `$word.Quit()` вообще
даже не может выполниться — процесс так и остаётся висеть, пока его не прибить
вручную.

Я пытался исследовать эту проблему и находил рекомендации создать каталог
`C:\Windows\System32\config\systemprofile\Desktop` (с возможной заменой
`System32` на `SysWOW64` в зависимости от битности системы и установленного
Microsoft Office). Эти рекомендации хоть и решили проблему запуска Word, но не
решили проблемы конвертации PDF; на этапе вызова конвертера Word просто зависал
(уже без пожирания системных ресурсов).

Оказалось, что можно подойти к проблеме по-другому: модель выполнения COM
позволяет нам настроить объект так, чтобы он создавался в процессе,
принадлежащем "обычному" пользователю. Таким образом, проблем с разрешениями и
недостающими каталогами можно вообще избежать.

Итак, нужно запустить старый добрый `dcomcnfg` (или `mmc comexp.msc /32` для
доступа к 32-битным объектам в 64-битном окружении), найти в нём "Microsoft Word
97 - 2003 Document" (несмотря на то, что, казалось бы, этот элемент не имеет
отношения к DOCX и PDF), и разрешить ему запуск от имени текущего пользователя.
Создавать каталог `systemprofile\Desktop` при использовании этого способа нет
необходимости.

На правильное решение меня натолкнули эти вопросы и ответы на Stack Overflow:

1. [Automating MS Word in Server 2012 R2][so-0].
2. [Task Scheduler doesn't execute batch file properly][so-1].
3. [Automating MS Word in Server 2012 R2][so-2].

[so-0]: http://stackoverflow.com/a/34720474
[so-1]: http://stackoverflow.com/a/25210194
[so-2]: http://stackoverflow.com/a/34720474
