---
title: Tracing Wasabi
---


# dotnet-trace

First, install the trace tool. It is common to install it as a `global` tool but it is not really necessary. 

```bash
$ dotnet tool install --global dotnet-trace
```

Next, find the `pid` of the process you want to trace with:

```bash
$ dotnet-trace ps
 3342266  dotnet  /usr/lib/dotnet/dotnet/dotnet  /WalletWasabi.Backend/bin/Release/net6.0/publish/WalletWasabi.Backend.dll
```

Finally collect the trace with:

```bash
dotnet trace collect -p <process id> --format speedscope
```

We are going to use `speedscope` format for Linux and `nettrace` (the one by default) for Windows. Anyway, the rest of this article uses `speedscope` format exclusively.

::: Warning
In production servers this command can create several gigabytes trace so, don't let it run for too long. For example, in Wasabi backend server it creates about 1 GiB in 5 minutes!
::::

# speedscope

We can use [https://www.speedscope.app/](https://www.speedscope.app/) to explore interactively the performance profile of our process. It runs totally in-browser, 
and does not send any profiling data to any servers, what is cool. 

![speedscope](../../src/images/speedscope.png)

# Advanced exporing

`Speedscope` is super super cool but when you have hundreads and hundreads of frames (in the screenshot above we can see 309 frames!) going one by one is not an option. 
The best approach is to parse `speedscope` file and analize it with a program, this is specially good if your language has some kind of built-in query language like LINQ and
if you can get a statically-typed representation of the file content.

## F\# type providers at rescue

`F#` allows us to get an statically-type representation just by providing an example of the json. Here is one that is enough for F# to "learn" how to parse the real file:

```json
{
  "exporter": "Microsoft.Diagnostics.Tracing.TraceEvent@2.0.64.0",
  "name": "trace.speedscope",
  "activeProfileIndex": 0,
  "$schema": "https://www.speedscope.app/file-format-schema.json",
  "shared": {
    "frames": [
      {
        "name": "Process64 Process(3342266) (3342266) Args: "
      }
    ]
  },
  "profiles": [
    {
      "type": "evented",
      "name": "Thread (3342401)",
      "unit": "milliseconds",
      "startValue": "3425.02085",
      "endValue": "397049.89585",
      "events": [
        {
          "type": "O",
          "frame": 0,
          "at": 3425.02085
        }
      ]
    }
  ]
}
```

And the script for analizying the frames looks like the following:

```fsharp
#r "nuget:FSharp.Data"
open FSharp.Data

[<Literal>]
let ResolutionFolder = __SOURCE_DIRECTORY__

type Trace = JsonProvider<"./trace.speedscope.json", ResolutionFolder=ResolutionFolder>

let trace = Trace.Load $"{ResolutionFolder}/trace.speedscope.json"
//let most_freq_events = trace.Profiles |> Array.collect (fun p -> p.Events |> Array.countBy (fun x-> x.Frame)) |> Array.sortByDescending snd;
//most_freq_events |> Array.take 1000 |> Array.map (fun (f,cnt) -> trace.Shared.Frames[f].Name) |> Array.distinct |> Array.filter (fun x -> x.Contains "Wasabi");


trace.Profiles 
|> Array.collect (
    fun p -> p.Events 
            |> Array.groupBy (fun e -> e.Frame ) 
            |> Array.map (fun (f,e) -> trace.Shared.Frames[f].Name, (e[0], e.[1])) 
            |> Array.filter (fun (f,es)->f.Contains "Wasabi"  ) 
            |> Array.map (fun (f,es)-> (p.Name, f), es |> fun (o, c) ->  c.At - o.At)) 
    |> Array.sortByDescending snd 
    |> Array.take 100;;


```

After run it you will get an outputs like this one:

```output
  [|(("Thread (3342410)",
      "WalletWasabi!WalletWasabi.BitcoinP2p.P2pBehavior.AttachedNode_MessageReceivedAsync(class NBitcoin.Protocol.Node,class NBitcoin.Protocol.IncomingMessage)"),
     392832.066404815474M);
    (("Thread (3342410)",
      "WalletWasabi!WalletWasabi.BitcoinP2p.P2pBehavior+<AttachedNode_MessageReceivedAsync>d__7.MoveNext()"),
     392832.066404815474M);
    (("Thread (3342410)",
      "WalletWasabi!WalletWasabi.Blockchain.Mempool.MempoolService.Process(class NBitcoin.Transaction)"),
     392807.276163122949M);
    (("Thread (3342410)",
      "WalletWasabi!WalletWasabi.BitcoinCore.Mempool.MempoolMirror.MempoolService_TransactionReceived(class System.Object,class WalletWasabi.Blockchain.Transactions.SmartTransaction)"),
     392738.303377355611M);
    (("Thread (3342410)",
      "WalletWasabi!WalletWasabi.BitcoinCore.Mempool.MempoolMirror.AddTransactions(class System.Collections.Generic.IEnumerable`1<class NBitcoin.Transaction>)"),
     392735.522252746241M);
    (("Thread (3342410)",
      "WalletWasabi!WalletWasabi.BitcoinCore.Mempool.MempoolMirror.EvictSpendersNoLock(class System.Collections.Generic.IEnumerable`1<class NBitcoin.OutPoint>)"),
     392733.110997790321M);
    ....
```

Using a similar code we can also count the frames to get the top methods and see the most invoked ones:

```output
  [|"WalletWasabi!WalletWasabi.Crypto.Groups.GroupElement.GetHashCode()";
    "WalletWasabi!WalletWasabi.Crypto.GroupElementVector.GetHashCode()";
    "WalletWasabi!WalletWasabi.Models.ImmutableValueSequence`1[System.__Canon].GetHashCode()";
    "WalletWasabi!WalletWasabi.WabiSabi.Crypto.CredentialRequesting.RealCredentialsRequest.GetHashCode()";
    "WalletWasabi!WalletWasabi.WabiSabi.Models.ReissueCredentialRequest.GetHashCode()";
    "WalletWasabi.Backend!WalletWasabi.Backend.Controllers.WabiSabi.IdempotencyRequestCache+<GetCachedResponseAsync>d__14`2[System.__Canon,System.__Canon].MoveNext()";
    "WalletWasabi.Backend!WalletWasabi.Backend.Controllers.WabiSabiController+<ReissuanceAsync>d__14.MoveNext()";
    "WalletWasabi.Backend!WalletWasabi.Backend.Controllers.WabiSabiController.ReissuanceAsync(class WalletWasabi.WabiSabi.Models.ReissueCredentialRequest,value class System.Threading.CancellationToken)";
    "WalletWasabi!WalletWasabi.WabiSabi.Crypto.IssuanceRequest.GetHashCode()";
    "WalletWasabi.Backend!WalletWasabi.Backend.Middlewares.HeadMethodMiddleware+<InvokeAsync>d__2.MoveNext()";
    "WalletWasabi.Backend!WalletWasabi.Backend.Middlewares.HeadMethodMiddleware.InvokeAsync(class Microsoft.AspNetCore.Http.HttpContext)";
    "WalletWasabi.Backend!WalletWasabi.Backend.Controllers.WabiSabiController+<RegisterInputAsync>d__12.MoveNext()";
    "WalletWasabi.Backend!WalletWasabi.Backend.Controllers.WabiSabiController.RegisterInputAsync(class WalletWasabi.WabiSabi.Models.InputRegistrationRequest,value class System.Threading.CancellationToken)";
    "WalletWasabi!WalletWasabi.WabiSabi.Models.Serialization.WitScriptJsonConverter.WriteJson(class Newtonsoft.Json.JsonWriter,class NBitcoin.WitScript,class Newtonsoft.Json.JsonSerializer)";
    "WalletWasabi!System.Threading.Tasks.TaskExtensions+<WithAwaitCancellationAsync>d__0`1[System.__Canon].MoveNext()";
    "WalletWasabi!System.Threading.Tasks.TaskExtensions.WithAwaitCancellationAsync(class System.Threading.Tasks.Task`1<!!0>,value class System.Threading.CancellationToken)"|]

```

## Checking results

After getting the sum of all the times with the previous script we can check in `speedscope` whether our result makes sense or not. In this case for example we have that the code that takes more time 
in Wasabi is the one that process incoming transactions received via the Bitcoin P2P network. The thread is `334210' and the method is `EvictSpendersNoLock`. Let's see how accurate that is:

![EvictDoubleSpend](../../src/images/speedscope-evictdoublespend.png)

Well, it takes more than 7 seconds to evict one single transaction and this method is culprit of `> 99%` of the execution time so, yes, this is the method we were looking for.

## Checking logs

Lets see the Wasabi logs for the last ten days:

```
sudo journalctl -u walletwasabi.service --since "10 days ago"
```

The last time the `MempoolMirror` worked as expected was `Oct 17`. After that date there were no more messages like this:

```
Oct 17 17:33:04 WalletWasabi walletwasabi-backend[3342266]: 2022-10-17 17:33:04.509 [53] INFO        MempoolMirror.ExecuteAsync (39)        6607 transactions were copied from the full node to the 
```

# dotnet-dump

First, install the trace tool. It is common to install it as a `global` tool but it is not really necessary. 

```bash
$ dotnet tool install --global dotnet-dump
```

Next, find the `pid` of the process you want to trace with:

```bash
$ dotnet-dump ps
 3342266  dotnet  /usr/lib/dotnet/dotnet/dotnet  /WalletWasabi.Backend/bin/Release/net6.0/publish/WalletWasabi.Backend.dll
```

Finally make a full dump with:

```bash
dotnet dump collect -p <process id>
```

## dumpheap

Do a `dumphead` and see:

```
00007fdc08554fe0      359      4279016 NBitcoin.Secp256k1.GE[]
00007fdc06bfdd40       39      5008296 WalletWasabi.Backend.Models.FilterModel[]
00007fdc0855cba0      392      6178048 NBitcoin.Secp256k1.GEJ[]
00007fdc0855d900      211      6522504 NBitcoin.Secp256k1.StraussPointState[]
00007fdc057154e0    65455      7854600 NBitcoin.Secp256k1.GE
00007fdc051d1ae0   328318     10506176 WalletWasabi.Backend.Models.FilterModel
00007fdc072cbed0    82693     11592736 WalletWasabi.Crypto.Groups.GroupElement[]
00007fdc06c02f48   328318     13132720 System.Lazy`1[[NBitcoin.GolombRiceFilter, NBitcoin]]
00007fdc02368080     4208     13325944 System.Int32[]
00007fdc06c02eb8   278843     15615208 NBitcoin.GolombRiceFilter
00007fdc051ecd28   328323     15759504 WalletWasabi.Blockchain.Blocks.SmartHeader
00007fdc07234c58        1     22324232 System.Collections.Generic.HashSet`1+Entry[[NBitcoin.Script, NBitcoin]][]
00007fdc06bf9548        3     22348632 System.Collections.Generic.HashSet`1+Entry[[NBitcoin.uint256, NBitcoin]][]
00007fdc023b1028     1844     34442018 System.Char[]
00007fdc072cb858    69801     36790040 NBitcoin.Secp256k1.Scalar[]
00007fdc051d8a68  1041446     49989408 NBitcoin.Script
00007fdc0236d2e0    60741     62876916 System.String
00007fdc04edfe58  2154463    103414224 NBitcoin.uint256
00007fdc072cd1a0  1169323    159027928 System.Lazy`1[[NBitcoin.Secp256k1.GE, NBitcoin.Secp256k1]]
00007fdc050c4740  1169323    205800848 WalletWasabi.Crypto.Groups.GroupElement
00007fdc02b7d740  1417841   1094670666 System.Byte[]
000055c87b444260   209717   1631828040      Free
Total 10254084 objects
```

# Other things

```
Oct 19 12:54:16 WalletWasabi walletwasabi-backend[3342266]:       System.ArgumentException: An item with the same key has already been added. Key: d33ac9c36fe99540b46a02c991aaed16a06a1681b6c1518760979687>
Oct 19 12:54:16 WalletWasabi walletwasabi-backend[3342266]:          at System.Collections.Generic.Dictionary`2.TryInsert(TKey key, TValue value, InsertionBehavior behavior)
Oct 19 12:54:16 WalletWasabi walletwasabi-backend[3342266]:          at WalletWasabi.BitcoinCore.Mempool.MempoolMirror.GetSpenderTransactions(IEnumerable`1 txOuts)
Oct 19 12:54:16 WalletWasabi walletwasabi-backend[3342266]:          at WalletWasabi.Backend.Controllers.BlockchainController.BroadcastAsync(String hex)
```
