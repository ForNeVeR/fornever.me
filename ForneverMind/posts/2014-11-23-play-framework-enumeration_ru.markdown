    title: Обработка перечислений в роутинге Play Framework
    description: Обработка перечислений Scala (Enumeration) с помощью стандартного роутинга Play Framework.
---

Уже не один раз за последний месяц возвращался к вопросу, как обрабатывать перечисления с помощью роутинга Play Framework. Задача такова: пусть имеется Scala-перечисление MyEnumeration, имеющее такое определение:

```scala
object MyEnumeration extends Enumeration {
  val MyValue1, MyValue2 = Value
}
```

(Подробнее про класс Enumeration читайте [в официальной документации](http://www.scala-lang.org/api/current/index.html#scala.Enumeration); если описать коротко — то это аналог перечислений, которые задаются в Java ключевым словом `enum`.)

Стандартный механизм роутинга Play Framework, который занимается маппированием параметров запросов на методы контроллеров, про наши перечисления ничего не знает и, если его им не научить, будет при попытке их использовать справедливо ругаться на отсутствие определений классов `QueryStringBindable` или `PathBindable` для наших перечислений.

Опишу требования к используемому решению. Итак, у нас есть вот такой вот файл роутинга `routes`:

```
GET / controllers.MyController.myAction(myValue: models.MyEnumeration.Value ?= models.MyEnumeration.MyValue1)
```

(Обратите внимание — пакеты приходится явно указывать для всех нестандартных классов, не забудьте этого при реализации.)

По идее, такой файл роутинга приведёт к тому, что при обращении по URL `http://localhost:9000/` будет вызвано действие `myAction` контроллера `controllers.MyController`, и в качестве единственного аргумента ему будет передано значение `MyValue1` перечисления `models.MyEnumeration`. Также предполагается, что при отправке `GET`-запроса по URL `http://localhost:9000/?myValue=myvalue1` действию контроллера будет передан параметр `MyEnumeration.MyValue1`.

На деле же такой подход не работает — как я уже говорил, Play Framework ругается на отсутствие биндера.

В интернете предлагается несколько решений, которые подразумевают включение соответствующего неявного определения (через `implicit def`) в наш класс перечисления. Например, [вот этот пост](http://danieldietrich.net/play-with-scala-url-path-binding/). Мне в этих решениях не понравилась узость их применения — предлагается в каждый класс-перечисление включать вот такую вот копипасту.

Немного подумал и выработал другое решение: абстрактный класс-наследник `Enumeration`, который сразу включает в себя это неявное определение биндера. Всем классам перечислений, которые используются в роутинге, предлагается наследоваться не от базового `Enumeration`, а от наследного `BindableEnumeration` (лёгким движением руки можно переделать его из абстрактного класса в трейт и использовать в качестве примеси, если это по каким-то причинам удобнее). Привожу реализацию этого класса для включения в наследников неявного определения `QueryStringBindable`. Это позволит использовать перечисление в качестве параметра URL. Для биндинга в качестве части пути (то есть для обработки запросов наподобие `http://localhost:9000/value/myvalue1`) нужно похожим образом реализовать другой, более простой биндер `PathBindable`, реализацию которого я, пожалуй, оставлю в качестве упражнения читателю. Ниже приведён полный код класса `BindableEnumeration`:

```scala
import play.api.mvc.QueryStringBindable

abstract class BindableEnumeration extends Enumeration { Self =>
  implicit def bindable(implicit stringBinder: QueryStringBindable[String]): QueryStringBindable[Self.Value] =
    new QueryStringBindable[Self.Value] {
      def bind(key: String, params: Map[String, Seq[String]]) =
        for {
          valueBind <- stringBinder.bind(key, params)
        } yield {
          valueBind match {
            case Right(value) =>
              Self.values.find(_.toString.toLowerCase == value.toLowerCase) match {
                case Some(v) => Right(v)
                case None => Left(s"Unknown parameter type '$value'")
              }
            case other => Left(s"Not found string value for key '$key'")
          }
        }

      def unbind(key: String, value: Self.Value) = stringBinder.unbind(key, value.toString.toLowerCase)
    }
}
```

Легко увидеть, что он производит матчинг приведением всех участвующих строк в нижний регистр; если для вашего случая нужен другой подход, то легко поменять это поведение.

А вот как наш класс-перечисление будет выглядеть после наследования от `BindableEnumeration`:

```scala
object MyEnumeration extends BindableEnumeration {
  val MyValue1, MyValue2 = Value
}
```

Поменяв в нём всего одну строку (имя базового класса), мы включили в него функциональность биндинга значений из параметров URL.
