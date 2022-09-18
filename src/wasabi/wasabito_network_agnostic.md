---
title: Wasabito Network
subtitle: Peer-to-peer network agnostic
tags: [plan, list]
summary: |
    I discuss how migrating from a client/server architecture (centralized coordinator) to a more descentraliced peer-to-peer
    network, even in the case of a hibrid architecture changes the initial assumptions about the usage of Tor and how it is 
    possible to write peers without any specific anonymity network in mind.

    You can  [read more about wasabito](wasabito_vision):
...


# Introduction

Be part of a peer-to-peer network would allow wallets to implement many protocols that would be otherwise impractical as [anonymous payments](wasabito_anonymous_payments) 
and all kind of collaborative multiparty transactions but even simpler tasks like using peers as proxies to the centralized coordinator 
cam improve greatly the [reliability](wasabito_network_reliability) of the communication.

# Proxing traffic 

As discussed the  [peer-to-peer](wasabito_network_reliability#P2P network) section of the  [reliability](wasabito_network_reliability) page,
one of the main benefits would be to be able to participate in coinjoin rounds without even needing to send information directly to the centralized
coordinator. This doesn't mean that all clients can operate without sending anything to the coordinator but that even if all clients connect, it
could happen that some clients decide to only relay foreing traffic to the coordinator. However, the case where a client decides simply not
to send information directly to the coordinator is feasible. The following graph shows how `Wallet 1` only send request through peer proxies, even
when it connects as `satoshi` to get status updates.

```{.dot render="dot -Tsvg -o %o.svg %i"
         img="../../src/images/wasabito_p2p_relay_graph"
         out="/home/lontivero/Documents/Wiki/src/images" }

digraph G {
	fontname="Helvetica,Arial,sans-serif"
	node [fontname="Helvetica,Arial,sans-serif"]
	edge [dir=both]

	"Wallet 1" -> "Coordinator" [style=dashed; label="satoshi"];
	"Wallet 1" -> "Wallet 2";
	"Wallet 1" -> "Wallet 3";
	"Wallet 2" -> "Wallet 3";
	"Wallet 2" -> Coordinator;
	"Wallet 3" -> Coordinator;
}
```

## Encryption

In this scenario a client could be eclipsed by some entity controlling most (or all) of the peers and learn about `Wallet 1`. To prevent 
such a thing **we need to encrypt the traffic** from end to end. One alternative is to connect to the coordinator as `satoshi` to get the
encryption key and use the Diffie-Hellman key exchange to compute a symmetric key to be used with AES. This assumes dishonest peers
and honest coordinator.

It's also recommended to include some nonce value to the messages to make sure the chiper text (message) looks different to each peer
that recieves it. 

## Isolation requirements

Using a clear connection could be enough for connecting to peers except for that fact that that would be reveal the IP addresses. So,
Tor, I2P or any other anonymization network is required. Howewer, that's only to hide the real IP address and not for isolating requests
under different identites.


This would make the implementation of the wallet much easier because it would allow us to remove all the Tor management complexities and
replace it by a simple algorithm that broadcast requests to peers randmonly. The only requirement then would be to have the ability to
utilize a SOCKs proxy because at least Tor and I2P provide that interface.


-------


