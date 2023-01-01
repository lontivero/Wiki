#r "nuget:NBitcoin"

open System
open NBitcoin

fun _ -> Console.ReadLine()
|> Seq.initInfinite
|> Seq.takeWhile ((<>) null)
|> Seq.map Script.FromHex
|> Seq.map (fun s -> s.GetDestinationAddress(Network.Main))
|> Seq.iter Console.WriteLine
