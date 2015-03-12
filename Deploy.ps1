$target = 'e:\Sites\fornever.me\'
Remove-Item -Recurse $target\*
Copy-Item -Recurse _site\* $target
