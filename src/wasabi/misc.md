---
title: Some useful things
toc: false
...

## Get production logs

```bash
alias getprodlogs='ssh zk-production -t "tail -1000 ~/.walletwasabi/backend/Logs.txt"'
```

## Watch production logs

```bash
alias watchprodlogs='ssh zk-production -t "tail -F ~/.walletwasabi/backend/Logs.txt"'
```

### Monitor input registration

```bash

while true
do
  ROUNDSTATE=$(curl -s https://wasabiwallet.io/WabiSabi/human-monitor | jq '.roundStates[0]')
  INPUT_COUNT=$(echo $ROUNDSTATE | jq '.inputCount')
  INPUT_REGISTRATION_REMAINING=$(echo $ROUNDSTATE | jq -r '.inputRegistrationRemaining')
  IS_BLAME=$(echo $ROUNDSTATE | jq '.isBlameRound')
  echo "inputCount:" $INPUT_COUNT  "Remaining:" ${INPUT_REGISTRATION_REMAINING:6}  "Blame:" $IS_BLAME 
  sleep 5
done
```

### Open recent cpoinjoins in browser

Parse the wasabi log file to find transactions and open each of those in a firefox tab.

```bash
ssh zk-production -t "tail ~/.walletwasabi/backend/WabiSabi/CoinJoinIdStore.txt \
| sed 's/\(.*\)/https:\/\/mempool.space\/tx\/\1/'" \
| xargs firefox
```

## Convert filters to binay

```c#
using System;
using System.IO;
using System.Linq;
using NBitcoin.DataEncoders;

namespace BinaryFilters
{
    class Program
    {
        private static byte[] EmptyByteArray = new byte[0];

        static void Main(string[] args)
        {
            using var binaryFile = File.Open("/home/lontivero/.walletwasabi/client/IndexMain.bin", FileMode.Truncate);
            using var writer = new BinaryWriter(binaryFile);
            using var textFile = File.OpenRead("/home/lontivero/.walletwasabi/client/IndexMain.dat");
            using var reader = new StreamReader(textFile);
            
            string line;
            while((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(":", StringSplitOptions.RemoveEmptyEntries);
                var blockId = Encoders.Hex.DecodeData(parts[0]).Reverse().ToArray();
                var filter = parts.Length == 2 ? Encoders.Hex.DecodeData(parts[1]) : EmptyByteArray;

                writer.Write(blockId);
                writer.Write(BitConverter.GetBytes(filter.Length));
                writer.Write(filter);
            }
        }
    }
}
```

```f#
open System
open System.IO

let bin xs = Convert.FromHexString xs
let binBE xs = xs |> bin |> Array.rev

let binFile = File.Create "IndexMain.bin"
let writer = new BinaryWriter (binFile)
let textFile = File.OpenRead "MatureIndex.dat"
let reader = new StreamReader(textFile)

(fun _ -> reader.ReadLine())
|> Seq.initInfinite
|> Seq.takeWhile (fun x -> x <> null)
|> Seq.map (fun x -> x.Split(":", StringSplitOptions.RemoveEmptyEntries))
|> Seq.map (fun [|bheight; bid; bfilter; bph; btime |] -> bheight, binGE bid, binBE bph, btime, bin bfilter )
|> Seq.iter (fun (height, bid, bph, btime, bfilter) ->
       writer.Write height
       writer.Write bid
       writer.Write bph
       writer.Write btime
       writer.Write (bfilter.Length)
       writer.Write bfilter)


```
[Tor reliability proposal](tor_reliability_proposal)

