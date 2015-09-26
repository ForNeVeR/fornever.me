fornever.me: Engineer, Programmer, Gentleman [![Build Status](https://travis-ci.org/ForNeVeR/fornever.me.svg?branch=master)](https://travis-ci.org/ForNeVeR/fornever.me)
============================================

This is the [fornever.me][] site source code. It is mainly written using
[Hakyll][hakyll] — a Haskell tool for static site development.

Prerequisites
-------------

For building this source code you'll need the following components:

1. [The Haskell Tool Stack][stack].
2. [Less CSS pre-processor][less].

Make sure all the tools are installed and placed in your `PATH` environment
variable before you begin.

Build
-----

First of all, you may need to setup stack if you haven't been done that already:

    $ stack setup

After that, build the site executable:

    $ stack build

Then use the executable to build rest of the site:

    $ stack exec forneverme build

It will generate a static HTML in the `_site` directory. Feel free to deploy
this content, but please don't forget to notify me if you're using my post
materials!

There's a `Build.ps1` file usable for Windows users that will build the whole
site.

Other components
----------------

1.  [EvilPlanner][evil-planner] meant to be an integral part of the site, but it
    need to be installed separately.
2.  [fornever.me][] uses an easy-to-install [Disqus][disqus] comment system.

[disqus]: https://disqus.com/
[evil-planner]: https://github.com/ForNeVeR/EvilPlanner
[fornever.me]: https://fornever.me/
[hakyll]: http://jaspervdj.be/hakyll/
[hakyll-faq]: http://jaspervdj.be/hakyll/tutorials/faq.html
[less]: http://lesscss.org/
[stack]: https://github.com/commercialhaskell/stack
