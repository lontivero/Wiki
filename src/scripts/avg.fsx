open System

fun _ -> Console.ReadLine()
|> Seq.initInfinite
|> Seq.takeWhile ((<>) null)
|> Seq.takeWhile ((<>) "")
|> Seq.map float
|> Seq.windowed 4
|> Seq.map Array.average
|> Seq.iter (fun a -> Console.WriteLine($"{a}"))
