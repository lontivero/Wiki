---
title: KVAC alternative to the so called Expensive Nostr Relays
toc: true
comments: true
...

:::::::: Warning ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
__DISCLAIMER__ --- This is a **WORK IN PROGRESS**.
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

## Abstract

[expensive-relay, a sybil-free corner of nostr](https://github.com/fiatjaf/relayer/tree/master/expensive) is a Nostr relay that can be immune to spam thanks to requiring
users to manually register themselves to be able to publish events and pay a fee via Lighting Network (it could
be also using other means).

The payment is associated with the user's public key, what can introduce some limitations for users posting events
to paid relays using different pubkeys.  


This document proposes a new private and decentralized ecash system based WabiSabi's Keyed-Verification Anonymous Credentials and Nostr 
E2E encryption messages and censorship resistance arquitecture.

## Motivation

Layer 2 solutions like the lightning network are how bitcoin can scale and it's possible to have special solutions for specific use cases.
Some needs like private payments, sub-satoshi payments or even payments of non-monetary value units can be easily implemented with an 
ecash system.

## Why WabiSabi?


