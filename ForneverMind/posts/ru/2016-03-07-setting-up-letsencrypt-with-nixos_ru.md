    title: Настройка letsencrypt на Nixos
    description: Краткая инструкция по получению сертификатов letsencrypt с использованием Nixos.
---

Сегодня мы поговорим про получение сертификатов для HTTPS. Как известно,
инфраструктура шифрования HTTPS требует наличия доверенного сертификата на
стороне сервера. К сожалению, до сих пор по большому счёту эти сертификаты
раздавались за деньги. Можно было, конечно, иногда получить несколько бесплатных
сертификатов на какой-нибудь промоакции, но для этого нужно было где-то
регистрироваться, а мультидоменных сертификатов на этих акциях могли и вовсе не
раздавать.

К счастью, несколько лет назад появилась, а не так давно перешла в стадию
открытого бета-тестирования инициатива [Let's Encrypt][letsencrypt], в рамках
которой распростряняются абсолютно бесплатные, удобные в получении сертификаты,
которые можно использовать для HTTPS. Вполне вероятно, что, если вы читаете эти
строки на моём сайте — то сертификат был выписан именно с помощью letsencrypt.

Использование letsencrypt сводится к двум простым этапам: получение сертификата
и настройка веб-сервера, чтобы он использовал этот сертификат.

Для получения сертификата используется специальная программа letsencrypt,
которая должна автоматически проверить ваши права на упраление доменом (что по
сути означает, что вы можете установить на домен HTTP-сервер, правильно
отвечающий на вопросы letsencrypt; судя по всему, ожидается, что злоумышленник
этого сделать не сможет, и потому сертификатов от имени вашего домена он не
получит).

[Официальная инструкция][letsencrypt-getting-started] предлагает искать пакет
`letsencrypt` в списке пакетов дистрибутива или же пользоваться последней
версией из [репозитория на GitHub][letsencrypt-github]. Я не нашёл инструментов
letsencrypt в стабильном канале Nixos, и потому решил испытать счастья с версией
из исходников. К сожалению, не вышло: она сообщала, что не знает ничего про мою
ОС, и потому работать не будет.

К счастью, обнаружилось, что добрые люди уже добавили соответствующий пакет в
нестабильный канал Nixos, и потому мы можем смело установить его оттуда:

```
$ nix-channel --add https://nixos.org/channels/nixos-unstable-small unstable
$ nix-channel --update
$ nix-env -iA unstable.letsencrypt
```

Теперь инструмент `letsencrypt` установлен в систему, и есть несколько вариантов
по получению сертификата. Мне самым простым показался вариант с однократным
запуском встроенного сервера letsencrypt. Для этого нужно сначала отключить
действующий на машине веб-сервер (если он у вас запущен), а затем выполнить
команду с перечислением списка желаемых доменов:

```
$ sudo systemctl stop nginx.service
$ letsencrypt cartonly --standalone -d fornever.me --test-cert
```

Здесь я указал ключ `--test-cert`, чтобы система выдала мне заведомо
недоверенный сертификат, потому что мы просто проверяем её работу. Для получения
настоящего сертификата этот ключ нужно убрать. Отмечу, что можно привести
несколько доменов в списке вот так: `-d fornever.me -d zomg.fornever.me`. На все
домены будет выписан один сертификат.

Эта команда задаст несколько простых вопросов и сгенерирует всё нужное в
каталоге `/etc/letsencrypt/live/fornever.me/`.

Теперь настроим веб-сервер. В моём случае настройка `nginx.conf` выглядела
следующим образом:

```
http {
    ssl_session_cache shared:SSL:10m;
    ssl_session_timeout 5m;
    ssl_prefer_server_ciphers on;
    ssl_stapling on;
    resolver 8.8.8.8;

    limit_req_zone $binary_remote_addr zone=one:10m rate=1r/s;

    server {
        listen 80;
        server_name fornever.me;
        location / {
            rewrite ^(.*)$ https://fornever.me$1 permanent;
        }
    }

    server {
        listen 443 ssl;
        server_name fornever.me;
        keepalive_timeout 60;
        ssl_certificate /etc/letsencrypt/live/fornever.me/fullchain.pem;
        ssl_certificate_key /etc/letsencrypt/live/fornever.me/privkey.pem;
        ssl_protocols TLSv1 TLSv1.1 TLSv1.2;
        ssl_ciphers  "HIGH:!aNULL:!MD5:!kEDH";
        add_header Strict-Transport-Security 'max-age=15552000';

        location / {
            # Тут уже персональные настройки для вашего сервера
            limit_req zone=one burst=10 nodelay;
            proxy_read_timeout 600;
            proxy_set_header Host $host;
            proxy_set_header X-Forwarded-Host $host;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
            proxy_http_version 1.1;
            proxy_pass http://192.168.12.233/;
        }
    }
}
```

На этом всё. Достаточно перезапустить nginx с новой конфигурацией, и сертификат
будет задействован. Удачного шифрования!

[letsencrypt]: https://letsencrypt.org/
[letsencrypt-getting-started]: https://letsencrypt.org/getting-started/
[letsencrypt-github]: https://github.com/letsencrypt/letsencrypt
