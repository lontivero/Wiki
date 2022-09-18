---
title: Mini Bitcoin Wallet in F# using NBitcoin Output Descriptors
...


```fsharp
#r "nuget:NBitcoin"

open NBitcoin
open NBitcoin.Scripting
open System.IO

type TWallet = {
    Mnemonic: Mnemonic
    [Network](Network.md): Network
    }

let recoverWallet (mnemonic: Mnemonic) (network:Network) =
    { Mnemonic = mnemonic; Network = network }

let createWallet (network:Network) =
    recoverWallet (Mnemonic (Wordlist.English, WordCount.Twelve)) network

let getExtKey (wallet: TWallet) =
    wallet.Mnemonic.DeriveExtKey()

let getExtPubKey (wallet: TWallet) =
    wallet
    |> getExtKey
    |> fun x -> x.Neuter();

let getKeyProvider (wallet: TWallet) =
    let masterKey = wallet |> getExtKey
    let masterPubKey = masterKey.Neuter()
    let keyProvider = FlatSigningRepository()
    keyProvider.SetSecret (masterPubKey.PubKey.Hash, masterKey)
    keyProvider

let getOutputDescriptors (wallet: TWallet): OutputDescriptor list =
    let parseDescriptor desc = OutputDescriptor.Parse (desc = desc, network = wallet.Network, repo = FlatSigningRepository())
    let coin = if wallet.Network = Network.TestNet then 1 else 0
    let xpub = wallet |> getExtPubKey |> fun x -> x.ToString(wallet.Network)
    [
        $"pkh({xpub}/44'/{coin}'/0'/0/*)"
        $"pkh({xpub}/44'/{coin}'/0'/1/*)"
        $"sh(wpkh({xpub}/49'/{coin}'/0'/0/*))"
        $"sh(wpkh({xpub}/49'/{coin}'/0'/1/*))"
        $"wpkh({xpub}/84'/{coin}'/0'/0/*)"
        $"wpkh({xpub}/84'/{coin}'/0'/1/*)"
    ] |> List.map parseDescriptor

let derivateScript (index: uint32) (keyProvider: FlatSigningRepository) (descriptor: OutputDescriptor) =
    let _, scripts = descriptor.TryExpand(index, keyProvider, keyProvider, cache = null)
    scripts

let saveWallet (walletName:string) (wallet:TWallet) = async {
    File.WriteAllTextAsync (walletName, wallet.Mnemonic.ToString()) |> Async.AwaitTask |> ignore
    return wallet
    }

let loadWallet (walletName:string) network = async {
    let! mnemonicPhrase = File.ReadAllTextAsync walletName |> Async.AwaitTask
    let mnemonic = Mnemonic(mnemonicPhrase, wordlist = null)
    return recoverWallet mnemonic network
    }

let saveNew walletName =
    createWallet >> (saveWallet walletName)
```

