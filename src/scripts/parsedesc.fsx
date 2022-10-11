open System
open System.IO


type Router =
  {
    nickname: string
    fingerprint: string
    ip: string 
    flags: string list
    meta: Map<string, string>
  }

let emptyRouter = { nickname=""; fingerprint=""; ip=""; flags=[]; meta=Map.empty }

let (|RouterData|_|) (s: string) = 
  match s.Split(' ') |> Array.toList with 
  | "r"::rest -> Some { nickname=rest[0]; fingerprint=rest[1]; ip=rest[5]; flags=[]; meta=Map.empty}
  | _ -> None

let (|Flags|_|) (s: string) = 
  match s.Split(' ') |> Array.toList with 
  | "s"::rest -> Some rest
  | _ -> None

let (|Meta|_|) (s: string) = 
  match s.Split(' ') |> Array.toList with 
  | "w"::rest -> 
    Some (rest 
      |> List.map (fun kv -> kv.Split("=")) 
      |> List.map (fun kv -> kv[0], kv[1])
      |> Map.ofList )
  | _ -> None


let parse (lines: string list) : Router list =
  let rec parseDetails (lines: string list) (router: Router): Router =
    match lines with
    | (Flags f)::t -> parseDetails t { router with flags = f }
    | (Meta m)::t -> parseDetails t { router with meta = m }
    | _ -> router

  let rec parseRouters (lines: string list) (routers: Router list): Router list =
    match lines with
    | (RouterData r)::t -> parseRouters t ((parseDetails t r)::routers)
    | (h::t) -> parseRouters t routers 
    | [] -> routers 

  parseRouters lines []

let contains l2 l1 =
  Set.ofList l1 |> Set.isSubset (Set.ofList ["Exit"; "Fast"; "Stable"; "Running"; "Stable"; "Valid"])

let isCandidate flags = 
  flags |> contains ["Exit"; "Fast"; "Stable"; "Running"; "Stable"; "Valid"]

let isBadCandidate flags =
  flags |> contains ["BadExit"]

let toRow r =
  $"| {r.nickname} | {r.fingerprint} | {r.ip} | {String.Join(',', r.flags)} | {String.Join(',', r.meta)} |"


fun _ -> Console.ReadLine()
|> Seq.initInfinite
|> Seq.takeWhile ((<>) null)
|> Seq.toList
|> parse
|> Seq.filter (fun r -> isCandidate r.flags)
|> Seq.filter (fun r -> r.flags |> List.contains "BadExit" |> not )
|> Seq.sortByDescending (fun x -> x.meta |> Map.tryFind "Bandwidth" |> Option.defaultValue "0" |> int ) 
|> Seq.map toRow
|> Seq.iter (Console.WriteLine)