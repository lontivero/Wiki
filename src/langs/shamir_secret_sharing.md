---
title: Shamir Secret Sharing
...


Most of the concepts are clarly explained in [this](https://en.wikipedia.org/wiki/Shamir%27s_Secret_Sharing) Wikipedia.

## Code

The code is mostly inspired from here: https://github.com/sinahab/shamir-secret-sharing


```fsharp
open System

let inline (mod) n d=
  ((n % d) + d) % d

let order = 39367 
let finiteField x = x mod order
                                        
let rec pow b e =
  match e with
  | 0 -> 1
  | _ -> finiteField (b * pow b (e - 1))
   
let rec extendedEuclid a b =
  match a with
  | 0 -> b, 0, 1
  | _ ->
    let gcd, x, y = extendedEuclid (b mod a) a
    gcd, y - (b / a) * x, x

let modMultInverse a p =
  let _, x, _ = extendedEuclid a p
  x

let createPolynomial cs x =
  cs
  |> List.mapi (fun i c -> c * (pow x i))
  |> List.sum

let mult xs = List.reduce (fun a b -> finiteField( a * b)) xs

let createShares secret k n =
  let f = createPolynomial (secret :: [for _ in [1..k-1] do finiteField (Random.Shared.Next())])
  [ for x in [ 1..n ] do (x, finiteField (f x)) ]

let recoverSecret shares =
  [ for (xj, yj) in shares do
      yield yj * mult [ for (xm, _) in shares do
                          if xj <> xm then
                            yield xm * (modMultInverse (finiteField (xm - xj)) order) |> finiteField ] ]
  |> List.sum
  |> finiteField

```

## Test

```fsharp
let shares = createShares 12344 3 7

let secrets =
  shares
  |> List.windowed 4
  |> List.map recoverSecret

```
