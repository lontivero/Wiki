---
title: Wasabito CoinJoin reliability
tags: [plan, list]
summary: |
    Here I propose a mechanism for simplifying the communication of the clients with the centralized coordinator
    and set the initial step to remove the centrilized coordinator one day.

    You can  [read more about wasabito](wasabito_vision):
...


# Introduction

[Wasabi Wallet](http://github.com/zkSNACKs/WalletWasabi) guaratees the central coordinator cannot learn the input-to-input, output-to-output nor input-to-output links.
That is achieved by using different Tor circuits for different identities. I mean, if your wallet selects two coins for
participating in a coinjoin, each of those coins are registered by sending the input registration requests through two
completely isolated Tor circuits. The same is true for all other requests.

# The "problem"

This mechanism was inherited, and greatly improved, from WW1. However, it requires more requests and more isolated Tor circuits
than WW1 implementation. This can be problematic under extreme network conditions like the current DDoS attack.

Imagine our wallet selects 4 coins (3.0, 0.1, 0.1, 0.00201) to participate in the next coinjoin round. The amount decomposer 
breaks the total 3.20201BTC as follow (1.0, 1.0, 0.5, 0.5, 0.2, 0.002) what requires 5 requests for reissuing the amount
credentials. So, in the absolutely best case it requires negotiate 15 Tor circuits. The circuits negotiation is expensive and
not always reliable. 

# Rethink the solution

Rethinking the solution requires to see the problem from a different perspective, in this case the question could be: **what if the
inputs/ouputs/signatures that users register don't belong to themselves but to other users instead?** In that case would make sense to
isolate the requests? 

The answer is no, it wouldn't make sense to try to hide the link of things that are not linked, or that we don't know whether
they are linked or not.

# P2P network

Given my wallet registers other people's inputs/outputs/signatures then others have to register my inputs/outputs/signature,
what means that wallets needs to speak one each other. Moreover, wallets should send the same request to more than one peer.

```{.dot render="dot -Tsvg -o %o.svg %i"
         img="../../src/images/wasabito_p2p_graph"
         out="/home/lontivero/Documents/Wiki/src/images" }

digraph G {
	fontname="Helvetica,Arial,sans-serif"
	node [fontname="Helvetica,Arial,sans-serif"]
	edge [dir=both]

	"Wallet 1" -> "Wallet 2";
	"Wallet 1" -> Coordinator;
	"Wallet 2" -> Coordinator;
	"Wallet 3" -> Coordinator;
	"Wallet 3" -> "Wallet 4";
	"Wallet 3" -> "Wallet 2";
	"Wallet 4" -> "Wallet 2";
	Coordinator;
}
```

We can say that we have the exact same problem that before because the communication with our peers has to be also 
annonymous and that requires Tor circuits, one for each request so my peer cannot link my requests, right? well, no because
requests are broadcasted.

## Request broadcasting

When a client receives requests from its peers it rebroadcast them to random peers, in this way other peers can't know whether
we are sending our requests or someone elses' requests. For this reason we don't need multiple circuits, just one (or more) 
long-life connection(s) to each peer.  


## Response delivery

The clients need to know the responses of their requests because those contain credentials, error messages, etc. so. there
are two alternatives:

1. The client receives that information as part of round status from the coordinator or,
2. The client receives responses sent back from the sender.

## Do we need encryption?

WabiSabi communication protocol was not design to prevent MitM like 
the one proposed by the scheme described here so, we should analyze whether
encryption is required.

A priori it seems we don't need to because:

(1) `prev_out` are public info,
(2) `ownership proof` commits to the specific round, 
(3) `issued credentials` cannot be presented without proving knowledge of the serial number and
(4) `presented credentials` are _randomized_. 


# Timing analysis

This scheme allows us to send request not only for others but also to send our own request mixed with those from others. Finally,
the requests randomization is also not only unnecessary but undesirable because clients should wait until they have collected 
enough requests from others in order to make as less request to the coordinator as possible.

# Other isolation considerations

Tor circuits are the communication isolation mechanims so, if the clients don't need to isolate identities then circuits are
no mandatory and, following the same reasoning, if Tor circuits are not necessary anymore then Tor is not the only alternative.

See:  [Network anonymization agnostic](wasabito_network_agnostic)


# Preliminar conclusion

A peer-to-peer network, as mentioned in the  [wasabito vision document](wasabito_vision) is not only critical for P2EP and 
for payjoins in coinjoins, unlinkeable payments and similar schemes but also for removing one of the greater source of
unreliability: the Tor circuit management to guarantee identities isolation and request scheduling for preventing
network timing analysis.



