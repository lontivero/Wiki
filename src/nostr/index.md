---
title: Private Nostr direct messages proposal
subtitle: Breaking linkeability between chat participants
toc: true
...

During the third day of the #Nostrica conference, [@styppo](npub1duedmhed2nevtejwz4c2hjuu0gz7spqm4s8wnaprta55ln9k3dwssvgpq4) [hamstr.to](https://hamstr.to) dev and [@anton](npub1h72rkut9ljnpdfyrcmw8q9jx52tgn2m8zyg0ehd6z236tz2vmg2sr5k5rt) [nostream](https://github.com/Cameri/nostream) 
contributor and [eden.nostr.land](https://eden.nostr.land) nostr relay dev/maintainer came to me to help them to validate an idea that they
were sleeping on since the previous day.

In this document I will explain the main idea at high level and I hope they come with their own explanation/discussions.

:::::::: Warning ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
__DISCLAIMER__ --- This proposal is about an interactive protocol for agreeing in fake identities, as such, I think is not really practical.
On the other side, it is so simple that can be useful for some use cases, specially in those where humans are not involved. This means,
it could be good for machine to machine communication protocols.
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

## Abstract

Nostr NIP 04 details a proposal for exchanging "encrypted direct message". The mechanism consists in encrypting the event's `content` using AES-256
with a shared secret key (Diffie-Hellman key exchange).

However, the proposal warnings about the leak of metadata that can be used to learn important information about the interaction and suggests to only
use relays that implement authentication to minimize this risk.

## Motivation

NIP 04 is a good starting point for implementing encrypted direct messages but it is important to mitigate its weaknesses in terms of privacy. The 
fact that the sender and receiver's public keys are exposed in plain allows anyone to see who the parties are, what is the main criticism to the 
proposal.

Removing the link between the two parties can improve the privacy properties of the encrypted direct messages and remove the need for using auth; or at least for the purpose of pretecting privacy.

## High-Level explanation

**Alice** wants to start chatting with **Bob** so she creates a new ephemeral key pair $(s_{fakeA}, P_{fakeA})$ and uses it for sending and encrypted message to **Bob**
saying "Hey Bob, this is Alice writting. Please write to me back to this fake identity." The message also contains some kind of proof to convince **Bob**
that the message really comes from **Alice**.

**Bob** then, after verifying the authenticity of the message received does the same, he generates a new ephemeral $(s_{fakeB}, P_{fakeB})$ and uses it to send
a similar message to **Alice** saying "Hey Alice, this is Bob. Let's use this fake identity." The message also contains a proof of authenticity to 
convince **Alice** that this message comes from **Bob**.

After this short dance both can exchange messages using the NIP 04 but using their new fake ad-hoc identities.

### Prove of authenticity

As we mentioned above it is necessary to prove somehow that the initial message comes from **Alice**, and the same for **Bob**. These initial messages (handshake)
can be authenticated in different ways, the simplest one is just to encrypt the message with the secret shared key generated from publicly known **Alice**
$(s_a, P_a)$ and **Bob** $(s_b, P_b)$ identities and then reencrypt it with the $s_{fakeA}P_bG$ `from Alice to Bob` and $s_{fakeB}P_{fakeA}G$ `from Bob to Alice`. 

The other alternative is to digitally sign the plain text content with $s_a$ and the Bob's response with $s_b$. This however adds a new cryptographic
primitive that was not necessary before. The extra space used by a signature is not really important and it is not worse than the extra initialization vector
required by the double encryption suggested before.

### What about relays? 

Many relays implement authentication and one reason to do that is to restrict access to encrypted direct messages only to those participating in conversations.
This solution requires to trust on the relays (it can be okay to do that) but it is a problem for implementing the proposal described here because the use
of ephemeral keys.

### Alternatives

It was discussed the idea of using a different ephemeral key pair for each message but that assumes the interactions between **Alice** and **Bob** follows a 
well defined pattern, it increases the complexity of the implementation and also requires to avoid `e` tags to reference previous messages.

## Analysis (metadata)?

An observer can only see that someone (unknown) sent an encrypted message to **Bob**. In case the handshake protocol doesn't use some king of random padding then the observer
can determine the message is a handshake (based on the length of the message). Then, the same observer can see than the unknown someone receives a direct message from someone
else (presumiblely **Bob**). **We clearly need a second fake identity for _Alice_ ** so, **Alice** can say "Hey Bob, this is Alice. Please write to me to this other fake identity".

As mentioned, the encrypted messages used in the handshake need to have some random padding to make them not so evident. It's also important to don't reintroduce linkeability via 'e' tag.

Relays that implement AUTH can still see that a client is interested on direct messages to some public key other thatn the one it was used to authenticate and in that case it could
infer the client is using a protocol like the one described here so, it is better to avoid relays that require authentication (also because in general the would refuse to relay direct messages to
strangers).


