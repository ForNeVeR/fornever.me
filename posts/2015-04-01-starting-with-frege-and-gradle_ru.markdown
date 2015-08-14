---
title: Начало работы с Frege с использованием Gradle
description: Описание настройки frege-gradle-plugin и основных задач по сборке проектов на Frege с использованием Gradle.
---

Сегодня мы поговорим о сборке проектов, написанных на функциональном языкк программирования
[Frege](http://www.frege-lang.org). Про особенности языка --- как-нибудь в другой раз, а сегодня побольше конкретных
примеров.

Дело в том, что этот язык является относительно молодым. Да, конечно, его разработка началась ещё в 2011 году, но
экосистема до сих пор является не очень продвинутой (хотя в ней есть всё необходимое для комфортной работы, о чём я и
хочу рассказать в этом посте). Приятно, что язык базируется на экосистеме JVM и, следовательно, может заимствовать особо
удачные решения этой экосистемы --- в частности, репозитории [Maven](https://maven.apache.org/) и популярные системы
сборки Maven, [Gradle](https://gradle.org/) или [Leiningen](http://leiningen.org/) (для всех перечисленных систем сборки
есть соответствующие плагины для работы с Frege).

В частности, сегодня мы рассмотрим интеграцию с Gradle. Для компиляции проекта мы будем использовать
[frege-gradle-plugin](https://github.com/Frege/frege-gradle-plugin).

## Компиляция

Любой проект на Gradle начинается с написания файла `build.gradle`. Всё, что от нас требуется по сути --- это добавление
плагина `frege-gradle-plugin` к определению проекта, а также настройка компиляции получившегося кода на Java (т.к. Frege
транслируется в Java). Вот шаблон `build.gradle`:

```
buildscript {
  repositories {
    mavenCentral()
  }
  dependencies {
    classpath group: 'org.frege-lang', name: 'frege-gradle-plugin', version: '0.2'
  }
}

apply plugin: 'java'
apply plugin: 'frege'
apply plugin: 'application'

repositories {
  mavenCentral()
}

dependencies {
  compile group: 'org.frege-lang', name: 'frege', version: '3.22.367-g2737683'
}

mainClassName = 'me.fornever.example.Application'
```

Этот код всего лишь подключает нужные плагины и настраивает компиляцию. Ожидается, что код на Frege будет расположен в
каталоге `src/main/frege`. Соответствие имён каталогов именам Java-пакетов во Frege необязательно, поэтому первый
простой модуль можно расположить просто в файле `src/main/frege/Application.fr`. Для полноты изложения приведу
содержимое этого файла (эта простая программа взята непосредственно из документации Frege):

```haskell
module me.fornever.example.Application where

greeting friend = "Hello, " ++ friend ++ "!"

main args = do
    println (greeting "World")
```

Чтобы выполнить этот код, нужно запустить в терминале следующую команду:

    $ gradle run

## Тестирование

Для тестирования разработчики Frege и сопутствующих инструментов рекомендуют использовать местную реализацию библиотеки
[QuickCheck](https://wiki.haskell.org/Introduction_to_QuickCheck1). Далее мы рассмотрим её применение к чистым функциям
и к "грязным" вычислениям.

Для работы с QuickCheck достаточно оставить в модуле публично доступные описания т.н. "свойств". Это можно сделать
довольно просто:

```haskell
module me.fornever.example.Application where

greeting friend = "Hello, " ++ friend ++ "!"

main args = do
    println (greeting "World")

import frege.test.QuickCheck

greetingTest :: Property
greetingTest = property $ \f -> greeting f == "Hello, " ++ f ++ "!"
```

Этот тест, конечно, довольно очевидный, но нам для примера сгодится. Чтобы его выполнить, достаточно запустить в
терминале команду

    $ gradle quickcheck

(Команду `quickcheck` тоже предоставляет `frege-gradle-plugin`.)

Поскольку я люблю CI вообще и сервис [Travis](https://travis-ci.org/) в частности, то сразу приведу пример `.travis.yml`
для автоматизации запуска таких тестов на сервере интеграции:

```yaml
language: java
jdk: oraclejdk8
script: gradle quickcheck
```

Если такого рода тестами вы можете покрыть существенную и важную часть приложения --- то вам существенно повезло. В
реальности так сделать получается не всегда --- частенько тестируемая функциональность завёрнута глубоко в монады `ST` и
`IO`, и вытащить её оттуда не так-то просто.

При этом задача `quickcheck` не умеет самостоятельно разворачивать монады типа `IO`, так что, даже если и разместить
значения типа`IO Property`, то это ничем делу не поможет --- они не будут протестированы.

Для того, чтобы выполнить "грязные" тесты, придётся реализовать свою исполняющую среду вместо `gradle quickcheck` (не
пугайтесь, это на самом деле совсем просто делается). Начнём с того, что реализуем простой "грязный" тест:

```haskell
greetingTestIO :: IO Property
greetingTestIO = return greetingTest
```

Понятно, что в этом тесте на самом деле никакого IO не происходит, но система типов об этом уже ничего не
знает. Следующее, что мы сделаем --- это дадим возможность передавать `mainClassName` из командной строки (чтобы можно
было запускать не только приложение, но и исполняющую среду для тестов), заменив последнюю строку в `build.gradle`:

```
mainClassName = System.getProperty("mainClass") ?: 'me.fornever.example.Application'
```

Теперь при запуске через `gradle run -DmainClass=some.another.Class` будет выполнен код другого главного класса. Нам
остаётся такой класс написать, и в этом нам поможет
[исходный код](https://github.com/Frege/frege/blob/03886e4c46d1e2e35288f277ed670fa1a1cb8d1a/frege/test/QuickCheckTest.fr)
модуля Test.QuickCheckTest.

Видно, что в нём объявлен ряд вспомогательных функций типа `quickCheck, quickCheckWith, quickCheckResult`, которые
принимают `Property` с разными опциями, а возвращают различные варианты значений, обёрнутых в монаду `IO`. Получается,
что мы можем написать простое приложение, которое использует эти функции для получения результата тестирования, а затем
просто возвращает операционной системе соответствующий код (стандартно --- `0`, если всё выполнилось хорошо, и `1`, если
какие-то тесты провалились). Сразу приведу всё приложение:

```haskell
module me.fornever.example.TestApplication where

import frege.test.QuickCheck
import me.fornever.example.Application (greetingTestIO)

checkResult :: Result -> Int
checkResult Success {} = 0
checkResult _ = 1

main :: [String] -> IO Int
main _ = do
    greetingProperty <- greetingTestIO
    result <- quickCheckResult greetingProperty
    return $ checkResult result
```

Как видно, мне пришлось объявить вспомогательную функцию, которая вычисляет код возврата по результату теста. В случае,
если нужно выполнять несколько тестов, можно их результаты любым удобным образом комбинировать в рамках данной
функции. Решение не претендует на абсолютную полноту, но в качестве первого шага к тестированию "грязного" кода оно
годится. Запускаются тесты следующим образом:

    $ gradle run -DmainClass=me.fornever.example.TestApplication

Это же можно легко закодировать в `.travis.yml` (и тогда Travis будет корректно отслеживать состояние тестов):

```yaml
language: java
jdk: oraclejdk8
script: gradle run -DmainClass=me.fornever.example.TestApplication
```

Вот таким нехитрым способом можно организовать модульное тестирование для любых сценариев.

## Приложение

Код приложения можно найти в проекте [frege-example-project](https://github.com/ForNeVeR/frege-example-project).
