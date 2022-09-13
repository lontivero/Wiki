---
title: Wasabito
tags: [plan, list]
...

:::::::: Warning ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
Note: not too long ago I shared a harsher version of this same content with the zkSNACKs internal staff.
That document was for the team and for that reason a harsh language was okay but sadly for that same reason it is not publishable so,
here I want to share the core ideas expressed at that time.
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


I've commented many times to different people inside as well as outside zkSNACKs how my vision about what Wasabi should be is quite
different than the official one but I think I have never explained what that vision is and what that implies in practical/technical
terms so, here I go.

# WabiSabi

WabiSabi is the innovation, what makes Wasabi unique and provides the best chances of providing privacy at low cost for more people
who need it most. Taking advantage of this technology is one of the most important things we should focus on. This means that payments
in coinjoins, P2EP (anonymous payments using the credentials system), open lightning channels in coinjoins, batching payments (related
to all the previous ones) is the important part.

# To Wallet or not to wallet

Wasabi wallet client should have being the first bridge to consume the coinjoin-as-a-service service but it became the Only way to 
access to the service, it is basically a gate keeper that doesn't let WabiSabi to reach all the potential that it can achieve. 

The Wasabi client (the wallet software) should be just a user-friendly way to participate in coinjoins but not the only way to do it.
In my vision many developers should be allowed to build on top of our solution and create useful things that we cannot even imagine.
This is something developers want to do and have been trying to do for a while with scripts in bash and python mainly, while others
tried to integrate the monolithic wallet using the rpc interface or porting to other platforms or rewriting part of the code, etc.

Sadly, maintainers don't make it easy for them because the official vision is a wallet for Peter McCormack.

Note: if well this is true, fortunately there are other devs from other companies contributing to change that.

# Service

Wasabi should run as a service/daemon all the time and not as a desktop wallet that we hide when the user tries
to close it. In fact, it should be a tiny, low resource (low cpu, low memory, low bandwidth) service that 
receives a compact filter every ~10 minutes, some information about the existing rounds and nothing else, except
when it is mixing of course.

The development of the daemon and the client should be 100% separated. The daemon would provide an API using 
gRPC (protobuf) so, the UI clients could consume it. The Avalonia UI could be adapted in a more or less 
transparent way while other clients could be developed by others (WebUI, CLI, Slack bot, whatever)
 
This implies that things like the sleep-prevention and the single-instance checker, among other things, 
should be removed. The lifetime management of the GUI and the daemon should be effectively different.

# P2EP

With Wasabi running as a daemon the P2EP idea is somehow easier to achieve and the goal should be to introduce 
payments in coinjoins first. This is the first really awesome app for WabiSabi because we can make payments really
private. This is the post-mix killer. This is the next big thing bitcoin privacy and only we can do it because it
requires wabisabi (and tor control) that we already have.

# Reliability

See: [Reliability](wasabito_network_reliability)


# Wasabi client/service should NOT depend on a server

The Wasabi service should be able to work independently of the existence of a coordinator. This means that the 
compact filters should be taken from the bitcoin p2p network while the fee rates could be obtained from an 
oracle or a trusted node. Currently users' money is unspendable when the coordinator is unavailable, that must change. 

BTC/USD rate is a dangerous feature should not be used by the service. The GUI can have configured some rate
provider that is trusted by the user but the coordinator should NOT provide this info at all.

In other words, the only component in the coordinator should be the one that coordinates the coinjoins.

# Technological update

Wasabi should support taproot scripts. This is something that many could need to implement solutions involving 
complex scripts. 

Wasabi should support wallet descriptors for interoperability and it is something that would be necessary to go from
a multi-wallets wallet to a multi-accounts wallet.

Wasabi should implement SLIP-39 as the default backup mechanism what would allow backing up users' seeds in the cloud
and it is also safer.

Wasabi should use the SigNet network for testing and should NOT provide testnet nor signet for final users (they 
should be able to specify the network in the command line only, something like `wasabi --testnet`)

# Amount decomposition

There should not be change in the coinjoins. There should not be unequal outputs in the coinjoins. Payments in coinjoins 
can produce unequal outputs but when there are no payments embeded then there should be no unequal outputs.

# Multi-account wallet

Wasabi should be a multi account wallet 

# Re-architecture the solution

Each component should have its own place and should be able to be deployed independently of other components. 
IndexService could be an independent application running in who knows how many servers. The fee rate oracle 
should be also an independent component. 

We should not provide BTC/USD rate but in case we do it it should be also an independent components. 

The website should not be part of the wallet software at all.

The wasabi service should be one independent of the UI.  

etc.

# Remove things that failed to achieve their goals

In my vision the current RPC is completely removed as well as payjoin support because it didn't gain any traction.
The bitcoin full node integration should be also removed because it doesn't do what users expect.

Password finder is the most obvious and absurd piece of trash that should be eliminated immediately. 

Swagger makes no sense for WabiSabi protocol because nobody can create crypotographic proofs by hand.

Other candidates are: 
 * Terms and conditions
 * Legal documents
 * All WW 1 code
 * Wallet backups folder
 * Dust threshold
 * kitchen
 * Clearnet backup (should be remove completely)

--------------------------

The following was not in the original document but I think this is even more important than many of the previous points:

# Wasaito Free and Libre 

The coordinator should not take any fees for coordination because this is the way to bring coinjoins to everybody. 
Each coordinator could implement it own policy. I can imagine private coordinators running in private networks available
for members only, plain fees or no fees at all.

A coordinator should be able to run in low resources machines, even with pruned bitcoin nodes, so running a coordinator 
should be cheap.

# Packaging

Wasabi depends on three other major components: bitcoind (optional), hwi (option) and Tor. 
It is the job of the package manager to install the dependencies and Wasabito should not include
the binaries for any of those. 

# Deployments

Deployments should be done with some kind of "one-click" (or "one command line command"), ideally with something like nixops.
It could be good to consider building deterministic docker containers with the coordinator.


