---
title: Control coinjoin process 
...

# How to run

First, extract all the coinjoins from the journal:

```
$ sudo journalctl -u walletwasabi.service --grep "Successfully broadcast the coinjoin: " > broadcasted.txt
```

```
$ head -1 broadcasted.txt
Aug 31 15:02:20 WalletWasabi walletwasabi-backend[16945]: 2022-08-31 15:02:20.080 [247] INFO        Arena.StepTransactionSigningPhaseAsync (356)        Blame Round (2df8dd8f7d942eaa3937d1bf04d6d766ae161359074f48095fcd3d45dac1baf1): Successfully broadcast the coinjoin: 1013f27dfd14ed1affba617e67db628b4215f4ad9d67f4c467855b311349848d.
```

Then run the script:
```
$ dotnet fsi sigma3.fsx
16.867450876232194
50.748847963747274
```

# Code

```{.fsx include="{{SOURCE_DIRECTORY}}/src/scripts/sigma3.fsx"}
```


