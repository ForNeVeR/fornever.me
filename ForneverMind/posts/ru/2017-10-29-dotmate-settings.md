    title: Настройка сбитых параметров модели в фотонаборном автомате ScanView DotMate 7500 SA+
    description: Детективная история о фотонаборном автомате.
---

Недавно мне пришлось решать нетипичную задачу по настройке типографского
оборудования, информацию о котором очень сложно найти в современном интернете.
Выполняя свой общественный долг, размещаю историю и инструкции в этом посте.

Передо мной была поставлена задача восстановить печатную функцию фотонаборного автомата ScanView DotMate 7500 SA+ (который был выпущен не позднее 2002 года; с тех пор сайт производителя закрылся и официальную поддержку по этим автоматам больше никто не предоставляет). С ним произошла следующая проблема: в один прекрасный день в автомате села батарейка, которая отвечает за хранение параметров в памяти автомата. Это повлекло за собой следующие последствия:

1. Настройки приходится восстанавливать после каждого старта автомата (то есть
   каждый день, если есть для него работа). Учитывая количество настроек (более
   20 числовых параметров), это весьма трудоёмкий процесс, который требует
   повышенной внимательности, и человек при настройке очень просто может
   ошибиться (что может привести к порче материалов, если автомат с такими
   настройками будет запущен в работу).
2. Помимо этого, автомат также не может запомнить критично важную информацию —
   свою собственную марку, и сбрасывает её на ScanView DotMate 7500 SA _без
   плюса_ (видимо, прошивка поддерживает несколько разных моделей автомата, и
   при сбое памяти иногда переключается на модель по умолчанию), а
   пользовательского доступа к изменению модели автомата нет. От модели автомата
   зависит работоспособность некоторых функций — в частности, работа с
   полиграфическим материалом больших габаритов. Поэтому автомат с расстроенной
   самоидентификацией не может быть использован на полную.

В связи с этим нам пришлось изрядно повозиться с автоматом, чтобы привести его в
полностью рабочее состояние. Далее я расскажу, как можно это сделать, и откуда
пришлось брать информацию.

В сопроводительном пакете вместе с автоматом поставлялась программа RipMate 4.1
(с обновлением до 4.2 на дискете), которой мы и пользовались при восстановлении.
Основная работа делается в терминале DotMonitor, доступ к которому есть из
основного окна RipMate. Этот терминал предоставляет текстовый интерфейс к
основным функциям мониторинга и настройки ScanView DotMate 7500 SA+.

Для выполнения настроек DotMonitor требует специального пользовательского
пароля. Насколько нам удалось установить, используя [информацию с
форума][rudtp], существуют следующие служебные пароли:

- `super`: Super User. Что делает — непонятно; никаких дополнительных функций
  после ввода пароля не появилось.
- `ast`: Authorized Service Technician. С использованием этого пароля можно
  провести настройку системы, но нельзя поменять модель.
- `770ast`: 770 Authorized Service Technician. Видимо, это пароль инженера,
  специализирующегося на автоматах какой-то конкретной марки (впрочем, что же
  такое 770 — история умалчивает).
- `abdast`: ABDICK Authorized Service Technician. Это инженер, который может
  поменять модель автомата, но только на какую-то свою, маркированную
  таинственным словом ABDICK.
- `svmp`: SCANVIEW MODEL. Нам не известно, какой сотрудник обозначен этим паролем, но **этот пароль позволяет поменять марку автомата на какую угодно**, что и требовалось.

Насколько нам удалось разобраться при настройке, пароль `svmp` даёт максимальные
полномочия по настройке автомата, и позволяет поменять любые параметры, включая
модель автомата. Довольно иронично, что именно этого последнего пароля в
открытом доступе на сегодняшний день нет вообще нигде.

Итак, для смены модели устройства через терминал DotMonitor, следует выбрать в
терминале следующие пункты:

1. `U: User`
2. `svmp`
3. `A: Adjust`
4. `V: Version parameters`
5. `M: Model`
6. Набрать номер модели (для ScanView DotMate 7500 SA+ это номер `12`)

Надеюсь, если кому-то ещё понадобится настраивать эти устройства почти 20-летней
давности, эта информация вам поможет.

[rudtp]: https://forum.rudtp.ru/threads/dotmate-7500p-nvram-config.68311/
