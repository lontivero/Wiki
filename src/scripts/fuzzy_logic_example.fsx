open System

module FuzzyLogic =
  type FunctionShape =
    | Triangular of float * float * float
    | Trapezoidal of float * float * float * float

  let (|LessThanOrEqualTo|_|) k value = if value <= k then Some() else None
  let (|GreaterThanOrEqualTo|_|) k value = if value >= k then Some() else None
  let (|Between|_|) b e value =  if value >= b && value <= e then Some() else None

  let triangular a b c x =
    match x with 
    | LessThanOrEqualTo a -> 0.0
    | Between a b -> (x - a) / (b - a)
    | Between b c -> (c - x) / (c - b)
    | GreaterThanOrEqualTo c -> 0.0

  let trapezoidal a b c d x =
    match x with 
    | LessThanOrEqualTo a -> 0.0
    | Between a b -> (x - a) / (b - a)
    | Between b c -> 1.0
    | Between c d -> (d - x) / (d - c)
    | GreaterThanOrEqualTo d -> 0.0

  let eval (f: FunctionShape) =
    match f with
    | Triangular (a, b, c) -> triangular a b c  
    | Trapezoidal (a, b, c, d) -> trapezoidal a b c d

  let area (f: FunctionShape) h =
    match f with
    | Trapezoidal (a, b, c, d) -> (b - a) * h / 2.0 + (c - b) * h + (d - c) * h / 2.0
    | Triangular (a, b, c) -> (c - a) * h / 2.0 

  type Centroid = float * float

  let centroid (f: FunctionShape) h: Centroid =
    match f with
    | Trapezoidal (a, b, c, d) -> a + (0.25 - a) / 2.0, h / 2.0 
    | Triangular (a, b, c) -> a + (c - a) * h / 2.0, h / 3.0 

  type CategoryName = string
  type Category = CategoryName * FunctionShape
  type FuzzyValue = float list

  let fuzzyfy (categories: Category list) score : FuzzyValue =
    categories 
    |> Seq.map (fun (_, f) -> eval f score) 
    |> Seq.toList

  let defuzzyfy (categories: Category list) (fuzzy_value: FuzzyValue) : float =
    let data = 
        fuzzy_value
        |> List.zip categories
        |> List.map (fun ((_, f), v) -> area f v, centroid f v)

    let s1 = data |> List.sumBy (fun (a, (x, _)) -> x * a)
    let sum_areas = data |> List.sumBy (fun (a, _) -> a)
    s1 / sum_areas

let credit_according_to_banckers : FuzzyLogic.Category list =
  [ "bad", FuzzyLogic.Trapezoidal (0, 0, 550, 650)
    "neutral", FuzzyLogic.Triangular (550, 650, 750)
    "good", FuzzyLogic.Trapezoidal(650, 750, 1000, 1000)]

let risk_according_to_bankers : FuzzyLogic.Category list =
  [ "low risk", FuzzyLogic.Trapezoidal (0, 0, 0.25, 0.5)
    "mid risk", FuzzyLogic.Triangular (0.25, 0.50, 0.75)
    "high risk", FuzzyLogic.Trapezoidal (0.50, 0.75, 1, 1)
  ]

Console.Write ("Enter credit score: ") 
let crips_credit_score = Console.ReadLine() |> float

let fuzzy_value_for_credit = FuzzyLogic.fuzzyfy credit_according_to_banckers crips_credit_score

fuzzy_value_for_credit 
|> List.zip credit_according_to_banckers
|> List.iter (fun (n, v) -> Console.WriteLine ($"{n} => {v}") )

fuzzy_value_for_credit
|> FuzzyLogic.defuzzyfy risk_according_to_bankers
|> fun v -> Console.WriteLine($"defuzzified value: {v}")