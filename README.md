<!--
SPDX-FileCopyrightText: 2024-2025 Friedrich von Never <friedrich@fornever.me>

SPDX-License-Identifier: MIT
-->

fornever.me: Engineer, Programmer, Gentleman [![Status Aquana][status-aquana]][andivionian-status-classifier] [![Docker Image][badge.docker]][docker-hub]
============================================

This is the [fornever.me][] site source code. It uses ForneverMind — a simple
homemade blog engine mainly written in F# programming language.

Features
--------

- ASP.NET Core web engine.
- Main pages layout in Razor.
- Blog posts are written in Markdown.
- Source code highlighting is provided by [highlight.js][] on the server side.
- [Disqus][disqus] comment system.

Contributing
------------

To know how to develop the application locally, read [the contributor guide][docs.contributor-guide].

Deployment
----------
Consider using the following [Ansible][ansible] task for deployment:
```yaml
- name: Deploy fornever.me website
  hosts: fornever_me
  vars:
    fornever_me_version: v5.0.0
    fornever_me_port: 5001

  tasks:
    - name: Install fornever.me
      community.docker.docker_container:
        name: fornevermind
        image_name_mismatch: recreate
        image: revenrof/fornever.me:{{ fornever_me_version }}
        published_ports:
          - {{ fornever_me_port }}:80
        restart_policy: unless-stopped
        default_host_ip: ''
```

This will deploy the Docker container version `v5.0.0` and make it to listen port `5001` on the host.

Documentation
-------------

- [The Contributor Guide][docs.contributor-guide]
- [Changelog][docs.changelog]
- [Maintainership][docs.maintainership]

License
-------
The project is distributed under the terms of [the MIT license][docs.license].

The license indication in the project's sources is compliant with the [REUSE specification v3.3][reuse.spec].

[andivionian-status-classifier]: https://github.com/ForNeVeR/andivionian-status-classifier#status-aquana-
[ansible]: https://docs.ansible.com/
[badge.docker]: https://img.shields.io/docker/v/revenrof/fornever.me?label=docker&sort=semver
[disqus]: https://disqus.com/
[docker-hub]: https://hub.docker.com/r/revenrof/fornever.me
[docs.changelog]: CHANGELOG.md
[docs.contributor-guide]: CONTRIBUTING.md
[docs.license]: LICENSE.txt
[docs.maintainership]: ./MAINTAINERSHIP.md
[fornever.me]: https://fornever.me/
[highlight.js]: https://highlightjs.org/
[reuse.spec]: https://reuse.software/spec-3.3/
[status-aquana]: https://img.shields.io/badge/status-aquana-yellowgreen.svg
