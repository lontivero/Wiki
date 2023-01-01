let pow b (e:int)  = float b ** e |> int64
let filter_upper_bound xs = xs |> Seq.takeWhile (fun x -> x <= 137438953472L) |> Seq.toList
let in_range xs = xs |> Seq.skipWhile (fun x -> x < 5000L) |> filter_upper_bound
let powers_of b = Seq.initInfinite (pow b) |> filter_upper_bound
let multiple (cs:int64 list) = List.collect (fun v -> [for c in cs -> c * v]) 

let powers_of_2 = powers_of 2 |> multiple [1]
let powers_of_3 = powers_of 3 |> multiple [1; 2]
let powers_of_10 = powers_of 10 |> multiple [1; 2; 5]

let stdd = powers_of_2 @ powers_of_3 @ powers_of_10 |> List.sort |> in_range

