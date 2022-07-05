---
title: My vision for Wasabi
...


I've commented many times to different people inside as well as outside zkSNACKs about how my vision about what Wasabi should be is quite different than the official one and, even when I am aligned to The one  I think I have never explained what that vision is and what that implies in practical/technical terms so, here I go.

# WabiSabi

WabiSabi is THE thing, it is the innovation, what makes us unique and provides the best chances of providing privacy at low cost for more people. WabiSabi is the service that we provide and the only one that allows the zkSNACks team to be funded to continue working on privacy solutions. 

Taking advantage of this technology is one of the most important things we should focus on. This means that payments in coinjoins, P2EP (anonymous payments using the credentials system), open lightning channels in coinjoins, batching payments (related to all the previous ones) is the important part.

# To Wallet or not to wallet

No, not to wallet. Wasabi wallet client should have being the first bridge to consume the coinjoin-as-a-service service but it became the Only way to access to the service, it is basically a gate keeper that doesn't let WabiSabi to reach all the potential that it can achieve. 

The Wasabi client (the wallet software) should be just a user-friendly way to participate in coinjoins but not the only way to do it. In my vision many developers should be allowed to build on top of our solution and create useful things that we cannot even imagine. This is something developers want to do and have been trying to do for a while with scripts in bash and python, while other tried to integrate the monolithic wallet using the rpc interface or porting to other platforms or rewriting part of the code, etc. Maintainers don't make it easier for them because the official vision is a wallet for Peter McCormack.

# Service

Wasabi should run as a service/daemon all the time and not as a desktop wallet that we hide when the user attempt to close it. In fact, it should be a tiny, low resource (low cpu, low memory, low bandwidth) service that receives a compact filter every 10 minutes, some information about the existing rounds and nothing else, except when it is mixing of course.

The development of the daemon and the client should be 100% separated. The daemon would provide an API using gRPC (protobuf) so, the UI clients could consume it. The Avalonia UI could be adapted in a more or less transparent way while other clients could be developed by others (WebUI, CLI, Slack bot, whatever)
 
This implies that things like the sleep-prevention and the single-instance checker, among other atrocities, should be removed. The lifetime management of the UI and the daemon should be effectively different.

# P2EP

With Wasabi running as a daemon the P2EP idea is somehow easier to achieve and the idea is to introduce payments in coinjoins first. This is the first really awesome app for WabiSabi because we can make payments really private. This is the post-mix killer. This is the next big thing bitcoin privacy and only we can do it because it requires wabisabi (and tor control) that we already have.

# Service should NOT depend on a server

The Wasabi service should be able to work independently of the existence of a coordinator. This means that the compact filters should be taken from the p2p network (or from the wasabi p2p network) while the fee rates could be obtained from an oracle or a trusted node but it is unacceptable the fact that users money is unspendable when the coordinator is unavailable. As an alternative the IndexBuilder could be extracted and generate the exact same filters (and filter headers ) that Core generates and it could be run but other actors or in multiple servers of even in local servers by the users how have a pruned node (Core doesn't allow to generate compact filter for pruned node so we can provide that)

Price (BTC/USD rate) is a dangerous feature that should not be used by the service. The UI can have configured some rate provider that is trusted by the user but zkSNACKs (or the coordinator) should NOT provide this info at all.

In other words, the only component in the coordinator should be the one that coordinates the coinjoins.

# Technological update

Wasabi should work with taproot scripts. This is something that many could need to implement solutions involving complex scripts. 

Wasabi should support wallet descriptors for interoperability and it is something that would be necessary to go from a multi-wallets wallet to a multi-accounts wallet (what would solve a few problems btw)

Wasabi should implement SLIP-39 as the default backup mechanism what would allow backing up users' seeds in the cloud and it is also safer.

Wasabi should use the SigNet network for testing and should NOT provide testnet nor signet for final users (they should be able to spacify the network in the command line only)


# Amount decomposition

There should not be change in the coinjoins. There should not be unequal outputs in the coinjoins. The amount organization should be re-design.

# Multi-account wallet

Wasabi should be a multi account wallet 

# Re-architecture the solution

Each component should have its own place and should be able to be deployed independently of other components. IndexService could be an independent application running in who knows how many servers. The fee rate oracle should be also an independent component. We should not provide BTC/USD rate but in case we do it it should be also an independent components. 

The website should not be part of the wallet software at all.

The wasabi service should be one independent of the UI.  

etc.

# Lightning network

F**ck that. I mean, i think lightning is awesome but that's not what we are good at, that's something that many other wallets can do much much better than us so, no, no lightning support. 

# Remove failed things

In my vision the current RPC is completely removed as well as payjoin support because it didn't gain any traction. The bitcoin full node integration should be also removed because it doesn't do what users expect and it is a huge amount of failed code.

Password finder is the most obvious and absurd piece of trash that should be eliminated immediately. 

Other candidates are: terms and conditions. legal documents, all WW 1 code, wallet backups folder, dust threshold, kitchen, clearnet backup (should be remove completely)



