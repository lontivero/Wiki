open System
open System.IO

let splitBySpace (l:string) = l.Split [| ' ' |]
let takeDateTimeParts (a:string array) = a[5] + " " + a[6] 
let releaseDate = DateTime.Parse "2022-10-27 00:00:00"
let isTooOld dt = dt < releaseDate
let timeDifferenceInMinutes (a:DateTime) (b:DateTime) = (b - a).TotalMinutes
let uncurry f (a,b) = f a b

let cjs = File.ReadAllLines("broadcasted.txt")

let elapsed = 
    cjs 
    |> Array.map splitBySpace 
    |> Array.map takeDateTimeParts
    |> Array.map DateTime.Parse
    |> Array.skipWhile isTooOld
    |> Array.pairwise 
    |> Array.map (uncurry timeDifferenceInMinutes)


let stdDev arr =
    let avg = Array.average arr
    sqrt (Array.fold (fun acc elem -> acc + (float elem - avg) ** 2.0 ) 0.0 arr / float arr.Length)

let avg = elapsed |> Array.average
let sigma3 = 3.0 * (stdDev elapsed);

Console.WriteLine avg
Console.WriteLine sigma3
