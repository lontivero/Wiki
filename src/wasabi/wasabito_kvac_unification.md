---
title: KVAC unification
subtitle: Simplify the whole credentials system
...

[WabiSabi: Centrally Coordinated CoinJoins with Variable Amounts](https://eprint.iacr.org/2021/206.pdf) paper is based on the work [The Signal Private Group System and Anonymous
Credentials Supporting Efficient Verifiable Encryption](https://eprint.iacr.org/2019/1416.pdf) which describes a cryptographic primitive
called `keyed-verification anonymous credential` (KVAC) designed to be used by the Signal Messaging system. The researcher presented a
credentials scheme which supports multiple `attributes`, this is, a credential can "contain" multiple pieces of information (eg. 
age, sex, eye color, department, dob, nationality or any other). 

Wasabi uses exactly the same scheme except for the fact that Wasabi's credentials only allow **one** attribute. It is for these reason 
that the concept of _credential type_ was introduced, because instead of having one credential with two attributes: `Amount` and `Vsize`,
there are two kind of credentials: `Amount credential` and `Vsize credential`.



