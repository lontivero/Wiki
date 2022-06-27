---
title: Effective F#
...

## Architecture

* Use Onion architecture
  * Dependencies go inwards. That is, the Core domain doesn't know about outside layers
 
* Use pipeline model to implement workflows/use-cases/stories
  * Business logic makes decisions
  * IO does storage with minimal logic
  * Keep Business logic and IO separate
  * Keep IO at edges

* Testing
  * Only need to unit test the business logic
  * IO is tested as part of integration tests

## High-level design

It's good to switch between top-down (decomposition) and bottom up coding. 

* Top down: Decide on the inputs and outputs first, then implement based on that.
  Then break that down into smaller functions than can be composed in a pipeline.
* Bottom up: write functions the way they *should* be, as independent things,
  then fit them together using "adapters" such as `bind`, `map` etc
* To parameterise behavior, pass in a function!

Grouping code:
* Types that change together should live together (e.g. in same module)
* Functions that change together should live together (e.g. in same module)

Make illegal states unrepresentable:
* use state machines to avoid using `option` for data which is not used in a particular state
* avoid bools
* when values change together, group them together (e.g `a option and b option` becomes `(a*b) option`)
  

## Setting up a project

* Compiler settings: warn unused "-warnon:1182"

## Modules and namespaces

* A namespace is just like C#
* A "module" is like a C# class with only static methods

If defining types only (eg domain), use a namespace.
If defining functions, they *MUST* be in a module (otherwise you get a compiler error)

For functions closely associated with a type (eg `create`/`value`), use a module with the same name as the type.

NOTES:
* You can't use namespaces in F# interactive
* You can use "rec" for recursive modules so that the definitions don't have to be in order.


TIP: Open few modules:
* Use qualified names so that readers know what you are referencing. Don't force people to remember things!
  Eg  `List.map` rather than `open List` and then using unqualified `map` later in the file.
* You can force this with the `[<RequireQualifiedAccess>]`  attribute on a module!
* Also, open modules as close as possible to where they are used (e.g. in a submodule rather than the top of the file)
  

## Signature files

* have `fsi` file extension.
* must be above the corresponding `fs` file.

```
|             | FSI              | FS          |
| top of file | namespace/module | same as FSI |
| types       | public           | public types must be defined the same |
|             |                  | private types OK |
| functions   | use "val"        | use "let"        |
```

Example signature file

```fsharp
/// Signature file
module MyDomain.Dto.Converter

open MyDomain

type DtoError =
    | BadName

module MyEntity =
    val fromDomain : MyEntity -> MyEntityDto
    val toDomain : MyEntityDto -> Result<MyEntity,DtoError>
```

Example implementation file

```fsharp
module MyDomain.Dto.Converter

open MyDomain

type DtoError =
    | BadName

type MyPrivateType = string

module MyEntity =
    let fromDomain (x:MyEntity) :MyEntityDto =
        failwith "not implemented"

    let toDomain (x:MyEntityDto) :Result<MyEntity,DtoError> =
        failwith "not implemented"

```

## Organising modules - general

F# is order sensitive -- modules only know about modules above them.
This maps nicely to the Onion architecture 
* core domain is at top
* IO near bottom
* Main app (composition root/Startup) is always last 

Example module structure:

1. Common libraries
1. Domain types
1. Workflows/use-cases
1. DTOs
1. DB/Infrastructure
1. API interfaces
1. Composition Root that combines everything and exposes the API. 

You can use dots in the filename to make the namespace clear:
*  `Common.PrimitiveTypes`
*  `Common.Types`
*  `MyWorkflow.Types`
*  `MyWorkflow.Impl`
  
For example: https://github.com/swlaschin/DomainModelingMadeFunctional/blob/master/src/OrderTaking/OrderTaking.fsproj

### Working with multiple assemblies 

If you want to swap out or hide infrastructure, you can use multiple assembles. For example:
  
`MyDomain.Core` assembly
* Common lib
* Domain types
* Workflows (with infrastructure dependencies injected in later)
* API

`MyDomain.Azure` assembly 
* DB/Infrastructure for Azure

`MyDomain.AWS` assembly 
* DB/Infrastructure for AWS

`MyDomain.Web` assembly (top-level assembly)
* Composition Root that combines everything and exposes the API 
  

## Code formatting

**Reading** happens much more than writing, so make it readble first. Make the INTENT understandable
During a code review if reader and writer disagree, reader is always right!

Make blocks obvious style - have only **one** indent per scope change.

```
// good
let xxxx xx xx =
  xxxx xx xx
  xxx xx 
  xxxx xxx
  xxxx xxx
  xxxx xx xxxx

// badly indented blocks
let xxxx xx xx =
  xxxx xx xx
    xxx xx 
   xxxx xxx
        xxxx xxx
     xxxx xx xxxx
```

### Function style

Two styles that I use:

* "stanza" style. Code is divided into related groups like stanzas in poetry.
* "recipe" style. Helper functions are defined (the "ingredients") and then combined in a pipeline.

Here's a "stanza" style function:

```fsharp
// "stanza" style
let myFunction xxx = 
  // part 1
  do this 
  andthen this 
  andthen something else 
  
  // part 2
  start a new thing 
  and some more

  // part 3
  ...
```

I believe a long function is perfectly fine if everything belongs together -- don't split it up for the sake of it

Here's a "recipe" style function:

```fsharp
// "recipe" style
let myFunction xxx = 
  // assemble ingredients
  let helperA = ....
  let helperB = ...
  let helperC = ...
  
  // then combine the ingredients
  start
  |> helperA
  |> helperB
  |> helperC
```

If you have a "big recipe" when the "ingredients" are more than one liners, then consider creating a private helper module to put the "ingredients" in.

```fsharp
module private Ingredients =
  // define ingredients
  let helperA = 
    ...
  let helperB = 
    ...
  let helperC = 
    ...

// bring module into scope
open Ingredients

// public  
let myFunction xxx xxx = 
  // combine the ingredients defined above
  start
  |> helperA
  |> helperB
  |> helperC
```

If you're using signature files and only one workflow per file, the "private" module is the whole `fs` file, so this approach is not needed.

### Format to make diffs more readable
  
Diffs are an important part of reading so make diffs easy to understand. Changing one thing should cause only one line to change!

```fsharp
// 1 or 2 fields
type MyRecord = { fieldA:string; fieldB:int }

// 3 or more fields
type MyRecord = {  
   fieldA : string
   fieldB : string
   fieldC : string
   fieldD : string
   }
```

Use same style for constructing records

```fsharp
let myRecord = {  
   fieldA = "..."
   fieldB = "..."
   fieldC = "..."
   fieldD = "..."
   }
```   

```fsharp
// enum-style choices
type Colour = Red | Blue | Green

// single case choices
type CustomerId = CustomerId of int

// complex choices   
type Payment = 
   | Cash
   | Check of CheckNumber
   | Card of CardInfo
```
  
### Formatting match expressions

Line up the vertical bars under the `match`:

```fsharp
let xxx p = 
  match p with
  | CaseA z -> handler...
  | CaseB z -> handler...
  | CaseC z -> handler...
```

When the "handlers" for each case become longer than a one-liner, put each handler in a new block underneath:

```fsharp
let xxx p = 
  match p with
  | CaseA z -> 
	handler...
  | CaseB z -> 
	handler...
  | CaseD z -> 
	handler...
```

### Formatting inline lambdas

Short ones can go on one line

```fsharp
aCollection
|> List.map (fun x -> x + 1)
```

Longer ones may need to start a new block

```fsharp
aCollection
|> List.map (fun x -> 
   xxxxx xxxx
   xxxx xxx
   )
```

Do *NOT* have "hanging" lambdas. If the top line changes, the indentation of the entire block will
change, breaking the "diff" rule above.

```fsharp
// example of BAD indenting. 
// If List.map is changed to List.choose, say, the entire block changes which messes up the diff
aCollection
|> List.map (fun x -> 
             xxxxx xxxx
             xxxx xxx
             )
```

### Formatting Generic types

F# allows generic types to be used as a suffix. Eg `'a list` or `List<'a>`.

Which one to use? Answer: Use suffix form for built-in types such as  `int list`, `int seq`, `int option`, `int[]`.
For all others, use C# style form (e.g. `Result<T,U>`).

## Coding tips

### Use exhaustive pattern matching
* Always be explicit and match every pattern
* Avoid using `_` as a wildcard pattern. Exception needed for matching strings and ints, where you often have to.
* If you do this, the compiler will warn you when new cases are added. 
  If you don't do this, you will never know!


### Avoid boilerplate
  
Often there are two similar bits of code but slightly different. In this case, it's easy to ignore subtle differences. 
  
FIX: parameterize with a function parameter! This forces differences to be made explicit:
  
* Example 1: iterating (aka fold)   https://youtu.be/E8I19uA-wGY?t=1100
* Example 2: continuations   https://youtu.be/E8I19uA-wGY?t=1966

### Make common errors obvious

Return a `Result` or `Option` rather than an exception.

But don't use `Result` for everything!	   
* There is a difference between "domain errors" and "panics". Panics do not need to be exposed in the domain.
  Throw an exception and catch at a top-level function. 
* Exceptions can be better than `Result` if the scope is clear and very local (e.g. drilling down in a tree and throwing to exit in the middle of iteration)

If you DO throw exceptions as part of the API, make it clear in the name of the function:

```fsharp
List.tryHead // ok
List.headExn // ok
List.head    // bad (sadly this is how the standard library does it
```

Similarly

```fsharp
List.find      // bad unless you KNOW that the item exists
List.tryFind   // good
```

### Complex type hacks 

Don't do it!

F# has Statically Resolved Type Parameters (SRTP) which can be used to do polymorphism, monads, etc. Don't!

KISS - simple is better than complex!

### Mutability

Mutable values are OK if they are local and no one sees them. But don't go overboard!
Purity is one goal but it's not the only goal.

### Operators

Avoid importing operators from other modules. It will be hard for a reader to know where they came from.
Instead define operators at the top of module they're used in (or even just in the function they are used in).

```fsharp
module xxxxx 

// define at top
let ( <!> ) = Validation.map
let ( <*> ) = Validation.apply

// use
ctor <!> firstParam <*> secondParam
```


## OO code vs FP code

It's OK to use OO style code if *behavior* is the most important thing -- that is, you want polymorphism.

e.g. in `x.ToString()` we don't care what x is.
To define methods see https://fsharpforfunandprofit.com/posts/type-extensions/

But this can mess with type inference :(  You might well need to use type annotations more often.
See https://fsharpforfunandprofit.com/posts/type-extensions/#methods-dont-play-well-with-type-inference

Similarly, it's OK to use interfaces to define groups of functions that can have multiple implementations.
See https://fsharpforfunandprofit.com/posts/interfaces/

Documentation on doing OO in F# is here: https://fsharpforfunandprofit.com/series/object-oriented-programming-in-fsharp.html

## Working with Dictionaries

```
module DictionaryExample =
    // needed to reference IDictionary
    open System.Collections.Generic

    // create a dictionary from a list of pairs
    let myDict = [ (1,"a"); (2,"b") ] |> dict

    /// get a value from a dictionary, with dictionary as LAST parameter
    let tryGetValue key (dict:IDictionary<_,_>) =
        match dict.TryGetValue(key) with
        | true, value -> Some value
        | false,_ -> None

    // this style is used when piping the dictionary into a key check
    myDict |> tryGetValue 1
    myDict |> tryGetValue 42


    /// get a value from a dictionary, with dictionary as FIRST parameter
    let tryGetValue2 (dict:IDictionary<_,_>) key =
        match dict.TryGetValue(key) with
        | true, value -> Some value
        | false,_ -> None

    // this style is used when you want to bake in the dictionary
    // in a helper function and then pass the keys in later
    let lookup = tryGetValue2 myDict
    lookup 1
    lookup 42
```

## Working with the Result type

If using `Result`, include this [Result.fs](https://github.com/swlaschin/DomainModelingMadeFunctional/blob/master/src/OrderTaking/Result.fs) file at the top of your project. 

I am fine with duplicating this file in many projects to avoid dependencies on nuget or library DLLs.

### To chain results in series ("monads")

If the first value is not a `Result`:

```fsharp
myValue
|> pointsFunctionA
|> Result.bind pointsFunctionB
```

If the first value *is* a `Result`:

```fsharp
myResult
|> Result.bind pointsFunctionA
|> Result.bind pointsFunctionB
```

## Result computation expressions instead of bind

You can use `result` computation expressions instead of `Result.bind`:

```fsharp
let finalResult = result {
    let! x = myResult
    let! y = pointsFunctionA x
    let! z = pointsFunctionB y
    return z
    }
```

### To combine results in parallel ("applicatives")

Use the `Validation` type in `Result.fs`. It's the same type as `Result` but with a *list* of errors.

If need to do validation (multiple errors), the validation code has the same pattern
  * Define a ctor for the type
  * Create all the values (which return `Results`)
  * Option 1: Use the applicative style to construct the object. 
    * First param has `<!>` in front
    * Subsequent params have `<*>` in front
  * Option 2: Lift the `ctor` using `lift2`, `lift3`, lift4` etc., depending on how many parameters the ctor has. 
  * When validation is complete and you are returning to normal code, consider mapping the list of validation errors to an error case so you have a normal `Result` again.
  
```fsharp
// Option 1: Applicative style

// define at top of file
let ( <!> ) = Validation.map
let ( <*> ) = Validation.apply

type MyError = 
   | ValidationError of string list
   | DbError of string 
   | etc

let createMyObject param1 param2 param3 = 
   { Field1 = param1; etc ... }
let param1OrError =  ...validate and return a Result
let param2OrError =  ...validate and return a Result
let param3OrError =  ...validate and return a Result
let myObjectOrError = createMyObject <!> param1OrError <*> param2OrError <*> param3OrError

// convert to a Result<_,MyError> without a list of errors
myObjectOrError |> Result.mapError ValidationError
```

```fsharp
// Option 2: Using lift

type MyError =  ...

let createMyObject param1 param2 param3 = 
   { Field1 = param1; etc ... }
let param1OrError =  ...validate and return a Result
let param2OrError =  ...validate and return a Result
let param3OrError =  ...validate and return a Result
let myObjectOrError = (Validation.lift3 createMyObject) param1OrError param2OrError param3OrError

// convert to a Result<_,MyError> without a list of errors
myObjectOrError |> Result.mapError ValidationError
  
```


See also:  
* https://github.com/swlaschin/Railway-Oriented-Programming-Example/blob/master/src/FsRopExample/Dtos.fs#L75

### Validating lists of items

To validate a list, validate each item using `List.map` and then use `Result.sequence` to put the `Result` type on the outside.

```fsharp
let itemsOrError =
    itemDtos
    // convert to F# list
    |> List.ofArray
    // convert each DTO item to a domain object
    |> List.map (ItemDto.toDomain)
    // flip from List<Result<>> to Result<List<>>
    |> Result.sequence
```


## Constrained types

Define a type with a private constructor and then a helper module (in same scope) with same name. Helper module should have `create` and `value` functions

```fsharp
type EmailAddress = private EmailAddress of string

module EmailAddress =
  let create str :Result<EmailAddress,_> =  // or Option
    if String.IsNullOrEmpty(str) then
      Error ...
    else if (* check validity *) then
       Ok (EmailAddress str)
    else
       Error ...

  let value (EmailAddress str) = str
```


## DTOs and Validation

Define them in their own module.
For each DTO, define an associated module with functions "toDomain" and "fromDomain"

```fsharp
namespace MyDomain.Dto

type MyDto = {
    Something: string
    }

/// could be in different module if you want to hide the implementation
module MyDto = 

   // may fail if DTO has bad data
   let toDomain (dto:MyDto) :Validation<MyDomain,ValidationError) =

   // always succeeds   
   let fromDomain (domainObj:MyDomain) :MyDto =
```

If using validation (multiple errors), the `toDomain` code has the "applicative" pattern described above.
  * Define a ctor for the DTO
  * Create all the values (which return Results)
  * Use the applicative style to construct the DTO
  * Map the list of validation errors to an error case
  
```fsharp
let toDomainObj dto = 
  let ctor = ...
  let firstParam = ... construct from dto.First 
  let secondParam = ... construct from dto.Second
  let thirdParam = ...
  let domainObjR = ctor <!> firstParam <*> secondParam <*> thirdParam
  domainObjR |> Result.mapError ValidationError

```
See also:  
* https://github.com/swlaschin/Railway-Oriented-Programming-Example/blob/master/src/FsRopExample/Dtos.fs#L75


If using validation WITHOUT multiple errors, the `toDomain` code can be simpler:

```fsharp
let toDomainObj dto = 
  result {
    let! firstParam = ... dto.First
    let! secondParam = ...
    let! thirdParam = ...
    domainObj = normal ctor 
    return domainObj
    }

```

See 
* https://github.com/swlaschin/DomainModelingMadeFunctional/blob/master/src/OrderTaking/PlaceOrder.Dto.fs#L120


## Interacting with C#

* Avoid exposing F# types if possible
* NOTE: If `Fsharp.Core.dll` is missing, use nuget to add package

### Exposing an API to C#

* Expose API in a .NET friendly manner. Add a module called either `Api` or `Api.Csharp`
* Understand tuple-style vs curried functions. Expose tuple-style only.

```fsharp
module Api =
  // correct
  let DoSomething(x,y,x) = 

  // incorrect
  let DoSomething x y x = 
```

* Use C#-compatible collections not F# `list`
  * F# `list` -- not available in C#
  * F# `int seq` -- same as `IEnumerable<int>` in C#
  * F# `ResizeArray<int>` -- same as `List<int>` in C#
  * F# `int[]` -- same as `Array<int>` in C#


* Have a CSharpHelper module with useful functions 

```fsharp
module List =
    // IEnumerable to F# list
    let EnumToList enum = enum |> List.ofSeq
    // IEnumerable from F# list
    let EnumFromList list = list |> List.toSeq
```

* Use Func<> instead of F# functions
* NOTE: F# Async needs to be converted to C# Task
* NOTE: F# `float` == C# `double`
* NOTE: C# Enums are not the same as choice/union types
* Don't expose tuples in Api

### Exposing choice types to C#

Create a `Match` function for each choice type you expose, with a `Func` for each case. Here's the one for `Result`:

```fsharp
module Result =
    let Match(result, onOk:Func<_,_>, onError:Func<_,_>) =
        match result with
        | Ok x -> onOk.Invoke(x)
        | Error e -> onError.Invoke(e)
```


## Common compiler errors

* Inconsistent type. Fix: add type annotations to find where the error is.
* Value restriction. Fix: add a parameter.
* See also https://fsharpforfunandprofit.com/troubleshooting-fsharp/


## Common errors in F# Interactive

* Can't use namespaces in interactive
* Need to explitly refer to other files using #load or #r
* Type mismatch even though you know its the same! 
  * caused by recompiling one bit without recompiling the other bit
  * FIX: recompile the whole thing



