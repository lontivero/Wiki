---
title: F# fragments
...


Read all input from console

```f#
open System;

let inputs = 
  fun _ -> Console.ReadLine()
  |> Seq.initInfinite
  |> Seq.takeWhile ((<>) null)
  |> List.ofSeq

Console.Write(inputs)
```

