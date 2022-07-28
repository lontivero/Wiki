---
title: Parse and count Ngnix requests
...


```fsharp
open System
open System.IO
open System.Text.RegularExpressions

let pattern = "^(?<remote>[^ ]*) - - \[(?<time>[^\]]*)\] \"(?<method>\S+)(?: +(?<path>[^ ]*) +\S*)?\" (?<code>[^ ]*) (?<size>[^ ]*)"

let parseLine pattern input =
  let m = Regex.Match(input, pattern)
  if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
  else None

let log = File.ReadAllLines("access.log")


log 
|> Seq.map (parseLine pattern) 
|> Seq.choose id
|> Seq.map (fun [_; _; method; url; _; _] -> method, url.Split '?' |> Array.head  )
|> Seq.countBy (snd)
|> Seq.sortBy (snd)
|> Seq.iter (fun (url, cnt) -> printfn "%d %s" cnt url)

```
