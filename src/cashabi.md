---
title: Cashabi
toc: true
comments: true
...

:::::::: Alert ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
This is a highly private, censorship resistant, nostr-native ecash system based on WabiSabi Keyed-Verification Anonymous Credentials

<https://github.com/lontivero/Nostra/tree/nostrsabi>


:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

:::::::: Warning ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
__DISCLAIMER__ --- This is a **WORK IN PROGRESS**.
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

## Abstract

This document proposes a new private and decentralized ecash system based WabiSabi's Keyed-Verification Anonymous Credentials and Nostr 
E2E encryption messages and censorship resistance arquitecture.

## Motivation

Layer 2 solutions like the lightning network are how bitcoin can scale and it's possible to have special solutions for specific use cases.
Some needs like private payments, sub-satoshi payments or even payments of non-monetary value units can be easily implemented with an 
ecash system.

## Why WabiSabi?

Many popular echash systems today are implemented using blind signatures, a cryptographic signature in which the signer doesn't sign the real message but a "blinded" version
of it that can later be verified against the original message. This was invented by David Chaum for his digital cash system and it is a simple and well-understood
solution. It was used in old versions of Wasabi coinjoins system and it is still being used by other coinjoin providers like Samourai's Whirpool system.

Blind signatures however impose a strong limitation in what developers can do with them because they can carry only one bit of information, this is, they are valid or
invalid. In the very first version of Wasabi coinjoin system a blind signature was used by the participants to prove against the coordinator that they had right to register a 0.1btc output in
the coinjoin round. Next version was extended to support multiple denominations of 0.1, 0.2, 0.4 and so on thanks to the usage of multiple signing keys, one per denomination.
In the case of Samourai's Whirlpool the exact same solution is used but instead of one key per denomination within a round it is one signing key per pool.

Basically all systems based on blind signatures suffer from the exact same problem and all they use the same solution: multiple signing keys. This limitation is specially problematic
for ecash systems because that means that they can use only standard denominations and because the verification of the amounts is costly given it involves to
check all the signatures agains every key to see which of them matches and then compute the real values sum.

Wabisabi doesn't suffer from any of this because it was designed precisely to solve this exact problem by carrying arbitrary and verifiable amounts. In contrast to many
ecash systems which maintain a collection of signatures that can be spent, with Wabisabi it is possible to have only one credential, what means that many complexities
like the selection of tokens to be used or the bandwith/cpu involves in the transactions is eliminated.


## Construction

:::::::: Warning ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
What I describe here is just what was developed as a **PROOF OF CONCEPT**
and it will be used as a crush dummy implementation to study the 
its privacy properties. 

This is **NOT** how Cashabi will look like at the end. 100% sure about it.
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


### Minters discovery

Minters (the entities that mint the cashabis) announce themselves by posting a nostr public note containing their WabiSabi parameters (public key). This is the only
unencrypted message that the minters send or receive, after the annoncement all the following communication is e2e encrypted.

### Cashabi wallets initialization

Cashabi wallets subscribe to minters' announcements and e2e encrypted direct messages. After been notified about a minter announcement it requests null credentials to it
(valueless credentials required for the protocol to fullfil the issuance requests to satisfy the required number of credentials to be presented in each request).

### Get cashabi units

The Cashabi wallets can request the minter to mint a given amount of units for them in exchange for some amount of bitcoin or based on any other criteria like a proof 
of participation in a coinjoins or simply as a reward for content created and publish using a given relay. This is clearly not defined and it shouldnt be part of the 
protocol.

The minter creates the credentials and sends them back to the wallet. 

The wallet makes sure to have a pool of at least two null credentials available all the time.

### Make payments

The Cashabi wallet takes the credential that contains the wallet balance and one extra null credential and presents them to the minter as part of a reissuance request 
for two new credentials: one credntial for payment (containing the units to be sent the the payee) and one credential for the rest. 
After that, the Wallet sends the credential to the payee's Nostr pubkey.

The payee receives the credentail and presents it to the minter with its own balñance credentail to consolidate them in one single credential.


## Use cases

Lets examine a real-world example here: Nostr. Nostr can be abused because it doesn't have a mechanism that protects the relays from flooding
them with tons of events. Many alternatives are being tried:

* Events PoW: makes it more expensive to attak a relay by imposing a cost to everyone. This socialization of the cost would be acceptable if there werent special hardware 
  for creating the pow, what means that attackers can increase the cost of protection by forcing the relay administrator to rise the difficulty.

* Public Key PoW: similar to the previous mechanism it attempts to mitigate the spam by requiring to mine a public key with a higher difficulty. However, something that is
  considered difficult to compute today can be trivial next year and spammers can start mining public key in the meanwhile.

* Expensive relays: requires a payment to be whitelisted. This requires the relay to implement authentication and makes more expensive to publish in multiple relays. This 
  alternative can lead to centralization.
  
One possible solution that could be explored is to pay by content where every note pays for a char/hour fee. This is, if I want to publish a 100 characters lenght note for 
one hour I need to pay 100char/hr units while storing the same note for a day would cost me 24 * 100char/hr units. A similar scheme could be applied for the subscriptions.

Such a payment scheme would allow relays to pay content creators for quality content just by rewarding the most consumed publications using the money that comes from the 
subscriptions.

Cashabi makes this scheme really simple because it pays to Nostr public keys. Additionally, the contacts metadata sharing mechanism in Nostr facilitates to pay to people that
we don't know without having to interact with them previously.



