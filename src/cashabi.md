---
title: Cashabi
toc: true
comments: true
...

:::::::: Alert ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
This is a highly private, censorship-resistant, nostr-native eCash system based on WabiSabi Keyed-Verification Anonymous Credentials

<https://github.com/lontivero/Cashabi>


:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

:::::::: Warning ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
__DISCLAIMER__ --- This is a **WORK IN PROGRESS**.
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

## Abstract

This document introduces Cashabi, an experimental eCash system that priorizes privacy, decentralization and censorship resistance. The system is based on the battle-tested
WabiSabi's Keyed-Verification Anonymous Credentials, known for enabling arbitrary output amount coinjoins in Wasabi Wallet. The underlying technology makes Cashabi pretty 
different from most of the already well-known eCash solutions. This document provides a comprehensive overview of Cashabi's architecture, highlighting its key features and
contributions to the realm of eCash systems.

## Motivation

In the pursuit of scaling Bitcoin, Layer 2 solutions like the Lightning Network have proven effective. However, specific use cases could require specific solutions, and
eCash systems emerge to address the requirements of private payments, sub-satoshi transactions, and exchanges involving non-monetary value units. Experimentation is crucial 
to explore the limits of the technology and this proposal aligns with that.

## Why WabiSabi?

Popular echash systems today rely on blind signatures, a cryptographic signature introduced by David Chaum for his digital chash system. In blind signatures the signer 
doesn't sign the actual message but a "blinded" version that can later be verified against the original message. This approach, simple and well-established, was initially 
used in the early versions of the Wasabi Wallet coinjoin system and continues to be adopted by other coinjoin providers like Samourai's Whirlpool system.

However, blind signatures come with a limitation—they can only carry one bit of information, i.e., they are either valid or invalid. In the first version of the
Wasabi coinjoin system, blind signatures were utilized by participants to prove to the coordinator their entitlement to register a 0.1 BTC output in the coinjoin round.
Subsequent versions expanded to support multiple denominations (0.1, 0.2, 0.4, etc.) by employing multiple signing keys, one for each denomination. Samourai's Whirlpool
employs a similar solution, utilizing one signing key per pool instead.


The common challenge among blind signature-based systems lies in their inherent limitation, prompting the use of multiple signing keys to overcome it. This constraint poses some issues for eCash systems, limiting them to standard denominations and making amount verification a costly process. Verification involves checking all signatures against every key to determine matches and then computing the actual values' sum.

WabiSabi distinguishes itself by resolving these challenges. Specifically designed to overcome these limitations, WabiSabi supports arbitrary and verifiable amounts. Unlike many eCash systems that maintain a collection of spendable signatures, WabiSabi can operate with only one credential. This design choice eliminates complexities such as token selection and reduces the bandwidth/CPU resources involved in transactions.

Finally, WabiSabi protects users' privacy against the minter, which learns nothing about the composition of the users' wallets.

## Construction

:::::::: Warning ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
What I describe here is just what was developed as a **PROOF OF CONCEPT**
and it will be used as a crush test dummy implementation to study the 
its privacy properties. 

This is **NOT** how Cashabi will look like at the end. 100% sure about it.
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


### Minters discovery

Minters (the entities that mint the cashabis) announce themselves by posting a nostr public note containing their WabiSabi parameters (public key). This is the only
unencrypted message that the minters send or receive, after the annoncement all the following communication is e2e encrypted.

### Cashabi wallets initialization

Cashabi wallets subscribe to minter announcements and encrypted direct messages. Upon receiving a minter's announcement, a wallet initiates a request for null credentials.
These valueless credentials are essential for the protocol to meet issuance requirements, ensuring the necessary number of credentials are available for each request.

### Get cashabi units

The Cashabi wallets can request the minter to mint a given amount of units for them in exchange for some amount of bitcoin or based on any other criteria like a proof 
of participation in a coinjoins or simply as a reward for content created and publish using a given relay. This is clearly not defined and it shouldnt be part of the 
protocol.

The minter creates the credentials and sends them back to the wallet. 

The wallet makes sure to have a pool of at least two null credentials available all the time.

### Make payments

When initiating a payment, the Cashabi wallet combines the credential containing the wallet balance with an extra null credential. This combination is presented to the
minter as part of a reissuance request for two new credentials: one for the actual payment to the recipient and another for the remaining balance. Subsequently, the 
wallet dispatches the payment credential to the payee's Nostr pubkey.

Upon receiving the payment credential, the payee submits it to the minter along with their own balance credential to consolidate them into a single, updated credential.
This streamlined process ensures the secure and efficient transfer of Cashabi units between wallets.

## Use cases

Lets examine a real-world example here: Nostr. Nostr can be abused because it doesn't have a mechanism that protects the relays from flooding
them with tons of events. Many alternatives are being tried:

* Events PoW: makes it more expensive to attak a relay by imposing a cost to everyone. This socialization of the cost would be acceptable if there weren't special hardware 
  for creating the pow, what means that attackers can increase the cost of protection by forcing the relay administrator to rise the difficulty.

* Public Key PoW: similar to the previous mechanism it attempts to mitigate the spam by requiring to mine a public key with a higher difficulty. However, something that is
  considered difficult to compute today can be trivial next year and spammers can start mining public key in the meanwhile.

* Expensive relays: requires a payment to be whitelisted. This requires the relay to implement authentication and makes more expensive to publish in multiple relays. This 
  alternative can lead to centralization.

* Antispam filters: analyzes the events to discard those with a spam score higher than a given threshold. An effective spam filter is complex and requires to be updated often. 
  
One possible solution that could be explored is to pay by content where every note pays for a char/hour fee. This is, if I want to publish a 100 characters lenght note for 
one hour I need to pay 100char/hr units while storing the same note for a day would cost me 24 * 100char/hr units. A similar scheme could be applied for the subscriptions.

Such a payment scheme would allow relays to pay content creators for quality content just by rewarding the most consumed publications using the money that comes from the 
subscriptions.

Cashabi makes this scheme really simple because it pays to Nostr public keys. Additionally, the contacts metadata sharing mechanism in Nostr facilitates to pay to people that
we don't know without having to interact with them previously.


