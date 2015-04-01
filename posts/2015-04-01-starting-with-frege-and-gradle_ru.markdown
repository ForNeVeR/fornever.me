---
title: Начало работы с Frege с использованием Gradle
description: Описание настройки frege-gradle-plugin и основных задач по сборке проектов на Frege с использованием Gradle.
---

Сегодня мы поговорим о сборке проектов, написанных на функциональном языкк программирования
[Frege](http://www.frege-lang.org). Про особенности языка - как-нибудь в другой раз, а сегодня побольше конкретных
примеров.

Дело в том, что этот язык является относительно молодым. Да, конечно, его разработка началась ещё в 2011 году, но
экосистема до сих пор является не очень продвинутой (хотя в ней есть всё необходимое для комфортной работы, о чём я и
хочу рассказать в этом посте). Приятно, что язык базируется на экосистеме JVM и, следовательно, может заимствовать особо
удачные решения этой экосистемы - в частности, репозитории [Maven](https://maven.apache.org/) и популярные системы
сборки Maven, [Gradle](https://gradle.org/) или [Leiningen](http://leiningen.org/) (для всех перечисленных систем сборки
есть соответствующие плагины для работы с Frege).

В частности, сегодня мы рассмотрим интеграцию с Gradle. Для компиляции проекта мы будем использовать
[frege-gradle-plugin](https://github.com/Frege/frege-gradle-plugin).

## Компиляция

Любой проект на Gradle начинается с написания файла `build.gradle`. Всё, что от нас требуется по сути - это добавление
плагина `frege-gradle-plugin` к определению проекта, а также настройка компиляции получившегося кода на Java (т.к. Frege
транслируется в Java). Вот шаблон `build.gradle`:

```groovy
buildscript {
  repositories {
    maven {
      url = 'https://oss.sonatype.org/content/groups/public'
    }
  }
  dependencies {
    classpath group: 'org.frege-lang', name: 'frege-gradle-plugin', version: '0.2'
  }
}

apply plugin: 'java'
apply plugin: 'frege'
apply plugin: 'application'

repositories {
  maven {
    url = 'https://oss.sonatype.org/content/groups/public'
    mavenCentral()
  }
}

dependencies {
  compile group: 'com.theoryinpractise.frege', name: 'frege', version: '3.22.367-g2737683'
}

mainClassName = 'me.fornever.example.Application'
```

Этот код всего лишь подключает нужные плагины и настраивает компиляцию. Ожидается, что код на Frege будет расположен в
каталоге `src/main/frege`. Соответствие имён каталогов именам Java-пакетов во Frege необязательно, поэтому первый
простой модуль можно расположить просто в файле `src/main/frege/Application.frege`. Для полноты изложения приведу
содержимое этого файла:

```haskell
module me.fornever.jtray.Application where

import me.fornever.jtray.Config (readConfig)
import me.fornever.jtray.Data (CCProject)
import me.fornever.jtray.Network (withNetworkStream)
import me.fornever.jtray.Parser (readStream)

jenkinsUrl :: String
jenkinsUrl = "http://localhost:3500/cc.xml"

configFileName :: String
configFileName = "jtray.properties"

openStream :: String -> IO InputStream
openStream path = FileInputStream.new path

main :: [String] -> IO ()
main _ = do
    stream <- openStream configFileName
    config <- readConfig stream
    println $ show config
    projects <- withNetworkStream jenkinsUrl readStream
    println $ show projects

```

## Тестирование

## Приложение

Код приложения можно найти в проекте [frege-example-project](https://github.com/ForNeVeR/frege-example-project).
