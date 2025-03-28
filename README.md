<!--
SPDX-FileCopyrightText: 2024-2025 Friedrich von Never <friedrich@fornever.me>

SPDX-License-Identifier: MIT
-->

fornever.me: Engineer, Programmer, Gentleman [![Status Aquana][status-aquana]][andivionian-status-classifier] [![Docker Image][badge.docker]][docker-hub]
============================================

This is the [fornever.me][] site source code. It uses ForneverMind — a simple
homemade blog engine mainly written in the F# programming language.

Features
--------

- ASP.NET Core web engine.
- Main pages layout in Razor.
- Blog posts are written in Markdown.
- Source code highlighting is provided by [highlight.js][] on the server side.
- [Disqus][disqus] comment system.

Configuration
-------------
Backend reads its settings from the standard `appsettings.json` file. The available settings are:

- `baseUrl`: URL to listen when started.

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

Versioning Policy
-----------------
The project adheres to [Semantic Versioning][docs.semver].

We consider the web API (HTTP requests and responses, URLs) as the public API. Project deployment requirements are not considered as public API, and thus may change at any time.

Since new published posts create a new available URL, they are considered as part of the API, and the addition of a post will lead to incrementing the version as well.

> [!NOTE]
> This project has a long history, and versioning was evolving through the years as well. In particular, the following versioning changes have been made:
> - the old versions (in Python and Haskell) were not versioned at all (these are tagged as `unversioned-*` in the changelog);
> - the versions in range \[1.0.0; 3.1.0\] didn't technically follow the Semantic Versioning;
> - the versions in range \[3.2.0; 4.6.3\] weren't always incrementing on new content publishing; sometimes, new content was added into an already published version.

Documentation
-------------
- [Changelog][docs.changelog]
- [Contributor Guide][docs.contributor-guide]
- [Maintainership][docs.maintainership]

License
-------
The project is distributed under the terms of [the MIT license][docs.license].

The license indication in the project's sources is compliant with the [REUSE specification v3.3][reuse.spec].

[andivionian-status-classifier]: https://andivionian.fornever.me/v1/#status-aquana-
[ansible]: https://docs.ansible.com/
[badge.docker]: https://img.shields.io/docker/v/revenrof/fornever.me?label=docker&sort=semver
[disqus]: https://disqus.com/
[docker-hub]: https://hub.docker.com/r/revenrof/fornever.me
[docs.changelog]: CHANGELOG.md
[docs.contributor-guide]: CONTRIBUTING.md
[docs.license]: LICENSE.txt
[docs.maintainership]: ./MAINTAINERSHIP.md
[docs.semver]: https://semver.org/spec/v2.0.0.html
[fornever.me]: https://fornever.me/
[highlight.js]: https://highlightjs.org/
[reuse.spec]: https://reuse.software/spec-3.3/
[status-aquana]: https://img.shields.io/badge/status-aquana-yellowgreen.svg
