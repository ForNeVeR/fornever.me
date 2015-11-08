    title: Настройка XMonad для работы с KDE 4
    description: Описание настройки XMonad для работы в среде KDE 4: управление окнами, панелями инструментов, полноэкранным режимом.
---

Существует такой замечательный tiling window manager — [XMonad](http://xmonad.org/). Он позволяет в полуавтоматическом режиме  управлять окнами в операционных системах Linux или FreeBSD. К преимуществам этого менеджера окон относят возможность максимально эффективно распределить рабочее пространство между открытыми окнами.

Мне бы хотелось поговорить про интеграцию этой программы с всем известной средой [K Desktop Environment](http://www.kde.org/), также известной как KDE. В первую очередь речь пойдёт о версии 4 (хотя процесс интеграции с KDE 3 не очень-то отличается).

Необходимость этого поста связана с тем, что официальная документация кое в чём ошибается (либо, возможно, содержит дистрибутивозависимую информацию), а часть важной информации, не относящейся к KDE напрямую, разбросана по другим её частям.

Я буду рассматривать использование XMonad под операционной системой [Fedora 20](http://fedoraproject.org/), однако получившаяся конфигурация полностью портируема на другие операционные системы с теми же версиями программного обеспечения.

## Установка XMonad в качестве менеджера окон KDE

### Установка пакета XMonad

В первую очередь XMonad нужно установить с помощью пакетного менеджера. Для этого используем консольную команду

    # yum install xmonad

(Для других дистрибутивов, конечно же, может использоваться и другая команда — если что-то не так, проконсультируйтесь с документацией своего дистрибутива; там *точно* есть раздел про установку программ из пакетов.)

### Интеграция с рабочим столом

Теперь стоит заняться подключением XMonad в качестве менеджера окон для KDE 4. Для этого нужно создать каталог `~/.kde/env`, если он ещё не существует, а затем создать там файл `set_window_manager.sh` со следующим содержимым:

```bash
export KDEWM=/usr/bin/xmonad
```

Также следует убедиться, что этот файл исполняемый. Я обычно "убеждаюсь" с помощью следующей команды:

    $ chmod +x ~/.kde/env/set_window_manager.sh

*Примечание.* [Официальная документация](http://www.haskell.org/haskellwiki/Xmonad/Using_xmonad_in_KDE) рекомендует для KDE 4 создавать файл в каталоге `~/.kde4`, но на Fedora это не работает; нужно использовать именно каталог `~/.kde`.

## Конфигурация XMonad для KDE 4

Теперь самое время написать конфигурационный файл. Я не буду описывать здесь особенности конфигурации XMonad или языка программирования [Haskell](http://www.haskell.org/), а просто приведу конкретные сниппеты и итоговый результат для тех, кто не хочет заморачиваться самостоятельным конфигурированием XMonad. Здесь отмечу лишь, что конфигурация XMonad должна храниться в файле `~/.xmonad/xmonad.hs`.

В первую очередь следует обратить внимание на то, что умолчальная дистрибьюция XMonad уже содержит небольшую конфигурацию для пользователей KDE 4. Стоит ей воспользоваться:

```haskell
import XMonad
import XMonad.Config.Kde

-- Переменную standardConfig мы будем в дальнейшем использовать для внесения модификаций
standardConfig = kde4Config

main = xmonad standardConfig
```

Сразу рекомендую настроить горячие клавиши (например, я предпочитаю использовать клавишу `Win` в качестве Meta Key, т.к. стандартный вариант с `Alt` слишком конфликтует с переключением раскладки по `Alt-Shift`):

```haskell
import XMonad

main = xmonad $ standardConfig {
                    modMask = mod4Mask
                }
```

После этих изменений можно уже, в принципе, перезапустить сесиию и логиниться в XMonad, но я рекомендую подождать до конца статьи :)

### Использование EWMH

Для взаимодействия DE и WM придуман стандарт Extended Window Manager Hints, сокращённо — EWMH. Смысл его использования — в том, что DE может передавать в WM информацию о каких-то своих панелях и прочем, а WM — отвечать ему взаимностью. Это позволяет использовать панели KDE совместно с XMonad (да-да, по умолчанию это не работает, так что даже таскбар не прорисуется).

В XMonad уже есть встроенный модуль для интеграции с EWMH. Можно включить его через конфиг:

```haskell
import XMonad.Config.Kde
import XMonad.Hooks.EwmhDesktops

standardConfig = ewmh kde4Config
```

Вот с такой конфигурацией уже не жалко переключить сессию и уже можно использовать XMonad. Остался уже, по сути, мелкий тюнинг, который можно комфортно выполнить уже будучи "внутри".

### Управление плавающими окнами

По умолчанию XMonad считает плавающие окна KDE 4 обычными окнами — разворачивает их на полэкрана и работает не очень хорошо; то же самое относится к выпадающему терминалу Yakuake, который я привык использовать. Он ломается и всё портит. Но этому легко можно противостоять! Вот набор хуков для того, чтобы сделать эти окна плавающими:

```haskell
import XMonad
import XMonad.Hooks.ManageHelpers
import XMonad.Layout.NoBorders
import XMonad.Layout.Accordion

main = xmonad $ standardConfig {
                    modMask = mod4Mask,
                    manageHook = manageHook kde4Config <+> myManageHook
                }

myManageHook = composeAll . concat $
    [ [ className   =? c --> doFloat           | c <- myFloats]
    , [ title       =? t --> doFloat           | t <- myOtherFloats]
    ]
  where myFloats      = ["MPlayer", "Gimp", "Yakuake", "krunner", "Plasma-desktop"]
        myOtherFloats = ["alsamixer"]
```

Этот фрагмент кода берёт стандартный manageHook (это хуки, используемые в XMonad для управления окнами), а затем добавляет к нему нашу функцию при помощи оператора композиции хуков `<+>`.

### Управление полноэкранными окнами

Иногда окна хотят развернуться на весь экран (например, видеоплеер или браузер в полноэкранном режиме), но из-за EWMH они не могут скрыть собой панель задач. Эту проблему можно решить при помощи специального библиотечного `fullscreenEventHook`, встроенного в XMonad. Для этого можно воспользоваться следующим сниппетом:

```haskell
import XMonad
import XMonad.Hooks.EwmhDesktops

main = xmonad $ standardConfig {
                    handleEventHook = handleEventHook kde4Config <+> fullscreenEventHook
                }
```

Существует ещё одна раздражающая поначалу проблема. XMonad рисует вокруг активного окна красную рамку толщиной в 1 пиксел. Всё бы хорошо, но зачем рисовать её и вокруг полноэкранного окна тоже?! Ведь понятно же, что на данный момент активно именно оно — а красная рамка раздражает и мешает сосредоточиться, например, на просмотре фильма.

Для того, чтобы избавиться от рамки на полноэкранных окнах, можно использовать ещё одну возможность XMonad — это layout hook. Встроенная функция `smartBorders` эффективно решает проблему. Где-то я видел разговоры о том, чтобы `smartBorders` режимом по умолчанию в новых версиях XMonad, однако же пока что это не реализовано, так что пользователь должен активировать этот режим самостоятельно. Сделать это можно следующим образом:

```haskell
import XMonad
import XMonad.Config.Kde
import XMonad.Hooks.ManageHelpers

main = xmonad $ standardConfig {
                    layoutHook = myLayoutHook
                }

myLayoutHook = smartBorders $ layoutHook kde4Config
```



## Результирующий файл

Для ленивых пользователей выкладываю весь мой файл `xmonad.hs`. Он подходит для любого окружения, работающего с KDE 4.

```haskell
import XMonad
import XMonad.Config.Desktop
import XMonad.Config.Kde
import XMonad.Hooks.EwmhDesktops
import XMonad.Hooks.ManageHelpers
import XMonad.Layout.NoBorders
import XMonad.Layout.Accordion
import qualified XMonad.StackSet as W

standardConfig = ewmh kde4Config

main = xmonad $ standardConfig {
                    modMask = mod4Mask,
                    manageHook = manageHook kde4Config <+> myManageHook,
                    handleEventHook = handleEventHook kde4Config <+> fullscreenEventHook,
                    layoutHook = myLayoutHook
                }

myManageHook = composeAll . concat $
    [ [ className   =? c --> doFloat           | c <- myFloats]
    , [ title       =? t --> doFloat           | t <- myOtherFloats]
    , [ className   =? c --> doF (W.shift "2") | c <- webApps]
    , [ className   =? c --> doF (W.shift "3") | c <- ircApps]
    ]
  where myFloats      = ["MPlayer", "Gimp", "Yakuake", "krunner", "Plasma-desktop"]
        myOtherFloats = ["alsamixer"]
        webApps       = ["Firefox-bin", "Opera"] -- open on desktop 2
        ircApps       = ["Ksirc"]                -- open on desktop 3

kdeManageHook = composeOne [
             isKDETrayWindow -?> doIgnore,
             transience,
             isFullscreen -?> doFullFloat,
             resource =? "stalonetray" -?> doIgnore
         ]

myLayoutHook = smartBorders $ layoutHook standardConfig
```
