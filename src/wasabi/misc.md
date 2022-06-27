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
