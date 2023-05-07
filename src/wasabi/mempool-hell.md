---
title: Mempool hell
subtitle: A possible solution for mempool inconsistences
...

# Introduction

Each Bitcoin node keeps a list of tansactions that have been validated and that are waiting to be confirmed. This list of transactions is known as mempool and the rules governing it are up to the node's operator. Among those policies ruling a mempool are the transaction expiration time and the mempool capacity (which is used to adjust the minimum fee required for a transaction to be accepted in the mempool). Moreover, operators can decide how what to do with those transactions that double spend, they can reject them or replace the previously seen transactions with the new ones when the later pay more fee (Full-RBF) 


# Mempool and Wasabi

Wasabi clients keep their own list of unconfirmed transactions that they received via P2P bitcoin network and try to maintaine it synchronized with the coordinator's node mempool. This is, they remove
from the local mempool those transactions that are not in the coordinator's node mempool. Clients also reject transactions that doublespend inputs.

# Possible inconsistencies

The inconsistencies can happen for multiple reasons. Here only a few:

* Transaction eviction because mempool pressure
* Dynamic Mempool minimum fee differences
* Double-spend/replacemnts policies in local node, p2p network and client

For the sake of the analysis lets focus only in the case of coinjoin transactions, where the coordinator is the only one that broadcast them and all the policies are well known.

| tx broadcasted by | coordinator node | The Mempool     | Wasabi client                         | After a while                  | Result                   |
|-------------------|------------------|-----------------|---------------------------------------|--------------------------------|--------------------------|
| backend           | ok               | mostly accepted | received via p2p ok                   | the tx is confirm              | consistency              |
| backend           | ok               | mostly accepted | received via p2p ok                   | it is evicted from The Mempool | long-live inconsistence  |
| backend           | ok               | mostly accepted | received via p2p ok                   | it is replaced full-rbf        | short-live inconsistence |
| backend           | ok               | mostly rejected | most clients will never know about it | na                             | short-live inconsistency |
|                   |                  |                 |                                       |                                |                          |

What would the wasabi client do?

* In case the cj is evicted from The Mempool the client remains inconsistent until a doublespending tx is mined or until the cj tx is evicted from the coordinators node.
* In case the cj is replaced (full-rbf) the client remains inconsistent until the replacement tx is mined
* In case the cj is rejected by The Mempool the situation will depend on the reason: doublespending? not enough fee? 
  
In at least the two first cases the client can spend the unconfirmed utxo and ruin a bit more the utxoset. A block containing a competing transaction or the coordinator's node expiring the never-confirming transaction fixes the problem.

# Alternative solutions

## The nuclear bomb

Note that all this happens because of the whole mempool concept. Without mempool there are not more mempool inconsistencies. That's the final goal behind `-blockonly` mode which instructs wasabi to not receive transactions via p2p. Sadly, it still uses the transactions stored in the `Mempool.dat` file. Anyway, by fully implementing the `-blockonly` mode we can start wasabi and see what is our real utxo set/balance (the one that the Bitcoin network has reached consensus about)

## Make long-live inconsistencies not so long

The eviction time is fully controlable by us and we can reduce it from the default (two weeks) to two days or so. In that case the clients' utxo set will be reseted to the one before the coinjoin.

## full-RBF

* **Enable full-RBF on the clients** would allow the wasabi clients to replace transactions that have been replaced in The Mempool.
* **Enable full-RBF on the coordinator's node** would allow to replace transactions and reflex that change on the clients via mempool synchronization mechanism.





