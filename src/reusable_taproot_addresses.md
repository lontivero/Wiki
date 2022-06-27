---
title: Reusable taproot addresses
...

:::::::: Alert ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
This is an alternative to bip47 by ©  Kixunil, MIT License (MIT).

<https://gist.github.com/Kixunil/0ddb3a9cdec33342b97431e438252c0ad>

:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

:::::::: Warning ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
__DISCLAIMER__ --- This is here just because i always forget everything so, 
got to the gist for an up to dated version.
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::



## Abstract

This document proposes a new scheme to avoid address reuse while retaining some of the convenience of address reuse,
keeping recoverability purely from Bitcoin time chain and avoiding visible fingerprint.
The scheme has negligible average overhead.

## Motivation

Address reuse is one of the most horrible things people can do for privacy.
However, the convenience can not be ignored and human beings are therefore incentivized to reuse addresses.
This is especially the case in long-term business relationships,
when it's convenient to avoid roundtrip of generating a new address.

There are existing schemes that tried to address this issue:

* Stealth addresses - they used ECDH to generate a new address for a new transaction putting ephemeral public keys in OP_RETURN outputs
  As a result they had significant overhead and visible footprint. Nonetheless their version is being used in Monero.
* Reusable payment codes (BIP 47) - a slight improvement over Stealth addresses.
  While they only require special transaction for initialization, subsequent transactions being free from overhead and visible fingerprint,
  the initial transaction must happen before real business activity and it produces effectively toxic change that may be difficult to handle.
  The initial transaction has no value besides providing convenient privacy.
  An alternate version proposed sending the required data over Bitmessage.
  This solved the problem with cost and footprint but introduced inability to recover coins purely from seed.
  In other words, the key received over bitmessage has to be backed up for every counterparty.
  Further, it requires yet another protocol that is not widely used.

This document proposes to solve the size/footprint issues of BIP 47 without losing the ability to recover from seed even if one of the parties is unavailable.

## Construction

### Initial transaction

Similarly to BIP 47, this scheme uses a notification transaction.
However unlike BIP 47, the notification transaction is also a "real" transaction.
It is the transaction performed when the parties conduct business for the first time using this scheme.
The sender of notification transaction MUST be spending at least one input of one of these types:

* P2PK
* P2PKH
* P2SH-P2WPKH
* P2WPKH
* P2TR

The sender MUST be capable of receiving to Taproot.
The receiver MUST use Taproot for receiveing.
(In principle, P2PK could be used instead of Taproot but such would be very unusual and thus suspicious.)
The transacting parties MAY use PayJoin but MUST NOT involve a third party in it.

When using this scheme, the sender MUST generate a change using the following algorithm.

0. Select an input with lowest index, belonging to the sender, being one of the types listed above. Let's call it sender key input.
1. Let `p_sender` be the private key associated with sender key input.
2. Let `P_receiver` be the Taproot address used by the receiver.
3. Calculate `shared_secret = SHA256(p_sender*P_receiver)` (ECDH)
4. Calculte `offset = HMAC(shared_secret, CHANGE_KEY_CONSTANT)` where `CHANGE_KEY_CONSTANT` is an arbitrary constant defined by the protocol
5. Calculate `P_change = (offset + p_sender)*G`
6. Calculate and securely cache `relationship_seed = HMAC(shared_secret, RELATIONSHIP_SEED_CONSTANT)` where `RELATIONSHIP_SEED_CONSTANT` is an arbitrary constant defined by the protocol that MUST NOT be equal to `CHANGE_KEY_CONSTANT`
7. Use `P_change` in the change output script

The receiver watches the address and reacts to received transaction by performing this check:

0.  Select an input with lowest index, **not** belonging to the receiver, being one of the types listed above - the sender key input.
1.  Let `P_sender` be the *public* key associated with sender key input.
2.  Let `p_receiver` be the private key associated with Taproot address used by the receiver.
3.  Calculate `shared_secret = SHA256(P_sender*p_receiver)` (ECDH)
4.  Calculte `offset = HMAC(shared_secret, CHANGE_KEY_CONSTANT)`
5.  Calculate `P_change = offset*G + P_sender`
6.  Check that `P_change` matches the key used in change.
7.  If the key doesn't match, don't continue
8.  Calculate `relationship_seed = HMAC(shared_secret, RELATIONSHIP_SEED_CONSTANT)` and cache it securely
9.  Precompute a reasonably large number of offsets using a *cryptographically secure PRNG* seeded by `relationship_seed`
10.  For each offset compute `P_i = (offset_i + p_receiver) * G`
11.  Start watching the chain for incoming transactions on public keys generated above

### Repeated sending

Each time the sender wishes to send to the receiver he computes `P_i = offset_i * G + P_receiver`, where `offset_i` denotes a random nonce generated by same PRNG mentioned in step 9 above.
The sender then sends coins to `P_i`.
The receiver has these addresses and private keys pre-cached so he can easily identify incoming transaction and spend the associated coins whenever needed.

### Recovery

#### Sender

In case of data loss, the sender recovers the history, coins and relationships using this algorithm:

1. Scan the chain for all transactions associated with BIP32 addresses
2. Every time a transaction is found which cointains at least two Taproot outputs that don't match any of the BIP32 addresses perform the following
3. For each output attempt to repeat the algorithm above deriving the `shared_secret` and the change output and check if the change output matches one of the outputs in the transaction
4. If match was found precompute offsets and the associated addresses and add them with the change output to the list of scanned addresses.

#### Receiver

1. Scan the chain for all transactions associated with BIP32 addresses
2.  Every time a transaction is found which contains at least one additional Taproot output, attempt to recompute `shared_secret` and check if any of the outputs matches expected change
3.  If a match is found, precompute the offsets and add the corresponding keys to the list of scanned addresses.

## Conclusion

As can be seen from the above, the notification transaction looks like a regular transaction.
Since the outside observers can not compute `shared_secret` (assuming ECDH is secure), they can not determine whether this protocol is being used nor compute the following addresses.
The protocol has same advantages over Stealth addresses as BIP 47, but in addition, the notification transactions can not be identified by outside parties,
the changes are not neccessarily *that* toxic (mixing is obviously still recommended) and the overhead is generally lower.

While it may seem that the overhead is zero, it's not exactly the case.
The overhead of notification transaction over not using this protocol is:

* The contruction requires Taproot which is on average a little bit larger than P2WPKH.
* Skipping the change output in the rare cases when it wouldn't be needed (sufficient UTXO available) is not possible.
* Tricks using [PayJoin to open a Lightning channel](https://github.com/Kixunil/loptos) or participate in other contracts are not possible or require additional output.
* Opening a Lightning channel with the change or sending the change to someone else is impossible.
* Batching several notification transactions produces a change output for each receiver.
  On the bright side, this can be used to simulate Samourai-style Stonewall, getting a bit more privacy for the added fee.
  
It can be argued that until various batching tricks become widely used this overhead is small.

