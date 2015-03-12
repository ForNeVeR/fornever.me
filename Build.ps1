chcp 65001
ghc --make -threaded site
if ($?) {
    ./site build
}
