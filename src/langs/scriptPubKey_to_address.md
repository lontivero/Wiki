---
title: Convert a script to its address
...


## Convert a script to its address (Main network)

```bash
$ echo 001499b73fb792d603bc6ef16664c9f39a8f850fd189 | dotnet fsi script2addr.fsx
```

```{.sh cmd="sh -c \"echo 001499b73fb792d603bc6ef16664c9f39a8f850fd189\" | dotnet fsi {{SOURCE_DIRECTORY}}/src/scripts/script2addr.fsx"}

```

## Convert all scripts in a file

```bash
$ cat CoinJoinScriptStore.txt | dotnet fsi script2addr.fsx > CoinJoinAddresses.txt
```

# Code

```{.fsx include="{{SOURCE_DIRECTORY}}/src/scripts/script2addr.fsx"}
```

