fornever.me: Engineer, Programmer, Gentleman
============================================

This is the [fornever.me](http://fornever.me) site source code. It is mainly written using [Hakyll](http://jaspervdj.be/hakyll/) - a Haskell tool for static site development.

Prerequisites
-------------

For building this source code you'll need the following components:

1. [Glasgow Haskell Compiler](http://www.haskell.org/ghc/) with the [Cabal packaging system](http://www.haskell.org/cabal/). I'd recommend you to use [Haskell Platform](https://www.haskell.org/platform/) that contains both.
2. Valid [Hakyll](http://jaspervdj.be/hakyll/) installation.
3. [Less CSS pre-processor](http://lesscss.org/).

Building
--------

First, build the Hakyll site:

    $ ghc --make -threaded site

Then use the Hakyll-compiled binary to build rest of the site:

    $ ./site build

It will generate a static HTML in the `_site` directory. Feel free to deploy this code, but please don't forget to notify me if you're using my post materials!

Other components
----------------

[fornever.me](http://fornever.me) uses a great and easy-to-install [Disqus](https://disqus.com/) comment system.
