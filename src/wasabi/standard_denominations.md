---
title: Wabisabi standard denominations 
...


# Code

```{.fsx include="{{SOURCE_DIRECTORY}}/src/scripts/stddenoms.fsx"}
```


```{.fsx icmd="dotnet fsi --exec %s.fsx"}
#load "{{SOURCE_DIRECTORY}}/src/scripts/stddenoms.fsx"
open System
open Stddenoms

Console.WriteLine("<pre>")
for d in stdd do
  Console.WriteLine ($"{d}")
Console.WriteLine("</pre>")
```

