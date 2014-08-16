$target = 'E:\Programs\nginx-1.7.4\html\'
Remove-Item -Recurse $target\*
Copy-Item -Recurse _site\* $target