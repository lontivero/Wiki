---
title: Application Layer Denial of Service Protection
...


# What's a Denial of Service attack?

A Denial-of-Service (DoS) attack is an attack meant to prevent the provision of a given service. 
A very complete explanation of the subject is available in [wikipedia](https://en.wikipedia.org/wiki/Denial-of-service_attack) 
but there are a very few concepts that are important to have in mind.

# Types of DoS attacks

Lets start by saying that there are two main kind of DoS attack that a **centralized** coordinator should
try to mitigate:

## (Distributed) Denial of Service Attacks (@ Transport Layer)

This is by far the most common type of attack and consists on a flooding the target with traffic. Given
it is generally speaking a transport layer attack, it is generic attack that can be performed using general-use
tools. 

It is also a cheap attack that is provided as a service by criminal groups who control botnets. That means that
anyone can pay for attacking any server and it doesn't require to have any kind of knowledge. 

Its **generic** nature allows the system adminitrator to mitigate it using **generic** infrastructure meassures
like a firewall.

## Specialized Denial of Service Attacks (@ Application Layer)

This is the kind of attack that can be performed only by those who understand how the system works and are
motivated to invest time and knowledge to design an attack for disrupting one specific target. It can be
achieved by reviewing the code and find programming errors or simply by understanding the dynamics of the
system.

By its **specialized** nature it requires the application to be designed with attacks in mind and to implement
counter-meassures that can mitigate the impact of such attacks. This is the type of attack that we want to 
discuss here.

# How to attack Wasabi

It is necessary to understand how Wasabi can be attacked in order to implement the prevention mechanism;
otherwise how could we possible know what to design for?

## Do not sign

The most obvious attack consists registering a coin for participating in a coinjoin but refuse to sign the
coinjoin transaction. This can be repeated as many times as coins you have, in fact, each coin can be used
to attack twice.

This attack is the simplest one because it doesn't require any real technical/programming knowledge, or at
least not at senior level. It is also an attack for free because it doesn't affect the attacker balance at
all.

The good news is that `Wasabi 2` is not specially vulnerable to this naïve attack because:

* it bans the offending coin,
* it runs a blame round with the remaining `whitelisted` coins,
* it runs many rounds in parallel,
* it runs rounds frequently (In WW 1 this was nto the case)


## Spend the participating coin

Another trivial attack is simply to register a coin for participating in a coinjoin but spending it before 
the coordination process finishes. In this case it is possible to disrupt a round but at least it is not for 
free, given the attacker has to pay the network fee.

:::::: Note :::::::::::::::::::::
Before continuing let me say that `Wasabi 2` allows users to spend the registered coin without needing to
unregister them first because in case the coordinator detects one participating coin has been spent, it 
simple removes that coin from the round except in the case of being in the critical phase (signing). This
improves the reliability of the rounds by shortening the window of time that can be used to disrupt the
round.
:::::::::::::::::::::::::::::::::

It is still possible to spend the coin in two critical moments:

### Spend during signing phase

Once the client is notified the round phased changed to signing phase it can sign the coinjoin transaction and the spend coin. In fact, in order to 
maximize the effectiveness of the attack the attacker client can wait until enough peers have signed before executing spending: otherwise it could
happen that she self spends her participating coin but someone else failed to sign and then she would have paid to disrupt a round that would have 
never happened anyway. Note that in this case it will not disrupt the blame round neither because it will be removed from the `whitelist` as soon 
the coordinator `input registration`.

### Spend after transaction broadcasted

The attacker can sign the coinjoin transaction and perform high-frequency requests to the wabisabi's `status` endpoint to see when **all** the
required signatures have been provided and when that happens (the coinjoin transaction is completed/can be broadcasted) the attacker that 
previously connected to a big number of bitcoins nodes in the network can broadcast a transaction that spends the participating coin. 

In this case, as well as in the previous one, it is the bitcoin network the one that will reject the broadcasted coinjoin transaction.

### Partial conclusion

Spending the participating coin is a simple and effective DoS attack against Wasabi that is **even in its basic form quite cheap** in terms of
the amount of bitcoins required to perform it (5000 sats).

:::::: Note :::::::::::::::::::::::
As any specialized attack, it can be cheap in terms of the amount of bitcoins required but expensive in terms of the effort/knowledge
the attacker must invest and the opportunity cost that that represent. This is IMO the reason why we have never seen an attack of this
kind.
:::::::::::::::::::::::::::::::::::


### What other are doing 

This kind of attacks affect most multiparty transactions and specially lightning (see: [Playing with full-rbf peers for fun and 
L2s security](https://lists.linuxfoundation.org/pipermail/bitcoin-dev/2022-June/020557.html)) There have been many discussion
about supporting **full-rbf** a-la Bitcoin Knots so, the transaction that is minded is simply the one that pays more. That would allow
coordinators to compete against attackers by fee or inflict them a greater economic damage.

## Banning offening coins

The general approach to prevent **future** disruptions by the same actor is by banning the coin used in the attack. Of course it is
important to be able to identify which coin was used, that's the first thing to do:

| Type                        | Is it a clear attack?     | Is being detected? | Can be detected? How?                         |
|-----------------------------|-------------------------- |--------------------|-----------------------------------------------|
| Spent during non-critical p.| No. It's Ok.              | Yes. No problem    |                                               |
| Did not sign                | No. But keep an eye on it | Yes                |                                               |
| Spent during critical phase | It seems so, yes.         | No                 | Yes, check again against the node (mempool)   |
| Spent after tx broadcasted  | Yes                       | No                 | Yes, listen for txs double spending cj inputs |
|                             |                           |                    |                                               |

:::::::: Error :::::::::::::
As we can see there are **two** attack vectors we are not even detecting!
:::::::::::::::::::::::::::

### To ban the offender

In case of detecting an attack (or a misbehaving coin), depending on the type of conduct it is involved (see table above) we  
have to ban it in order to prevent that coin from continue participating in next rounds.

In some cases like those in which the coin failed to sign it could convenient to ban the coin but give an opportunity to its descendants.

### To ban the offender and all its descendants

However, in other cases it could worth to consider banning `n` future generations so, even if an attacker self spends the offending coin
to continue attacking the rounds, those new coins will be already considered invalid to participate. 

This mechanism requires to check all future transaction and see if at least one banned coin is being spent by it and in case that's the case then
ban all the outputs of that transaction (this is how Wasabi 1 works)

### How long to ban

The `severity` of the penalty should take into account the type of infringement, the reinsidence and the amount involved.


