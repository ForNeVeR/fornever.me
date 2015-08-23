fornever.me: Engineer, Programmer, Gentleman [![Build Status](https://travis-ci.org/ForNeVeR/fornever.me.svg?branch=master)](https://travis-ci.org/ForNeVeR/fornever.me)
============================================

This is the [fornever.me](http://fornever.me) site source code. It is mainly
written using [Hakyll](http://jaspervdj.be/hakyll/) — a Haskell tool for static
site development.

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

*Note for Windows users:* sometimes Hakyll have problems with Unicode on this
platform. This is a known issue and it has been documented in the
[Hakyll FAQ][hakyll-faq]. I'll repeat it here: if you have any
problems mentioning `commitBuffer: invalid argument (invalid character)`, just
enable `chcp 65001` in console before running the `./forneverme` executable.

There's a `Build.ps1` file usable for Windows users that will build the whole
site.

Other components
----------------

[fornever.me](http://fornever.me) uses a great and easy-to-install [Disqus](https://disqus.com/) comment system.

[hakyll-faq]: http://jaspervdj.be/hakyll/tutorials/faq.html
[less]: http://lesscss.org/
[stack]: https://github.com/commercialhaskell/stack
