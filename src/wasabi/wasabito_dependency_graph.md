---
title: Credentials Dependency Graph
subtitle: Analyze possible optimization
...

# Credential reissuance

## How Wasabi currently works

Wasabi implements a mechanism that allows to register outputs in parallel. That is, a ways to 
break/consolidate credentials in order to get the exact number of credentials with the exact 
value and vsize necessary to register the outputs.

Imagine we have two inputs `{7; 3}` and want to split the total amount in outputs `{3; 3; 2; 1; 1;}`
, then it needs to:

1. break the `3`btc credential by two new credential of `2`btc and `1`btc 
2. break the new `2`btc credential is two new credentials of `1`btc and `1`btc
3. break the `7`btc credential in two new credencials of `5`btc and `2`btc
4. change the new `1`btc and `5`btc in two new credentials of `3`btc and `3`btc

After this **4 steps** we have all the credentials that we need to redister the 5 outputs in parallel. However,
note that the credential reissuance process requires, many times, to finish one reissuance before starting 
others. 

```{.dot render="dot -Tsvg -o %o.svg %i"
         img="../../src/images/wasabito_dependency_graph"
         out="/home/lontivero/Documents/Wiki/src/images" }
digraph G {
    subgraph cluster_inputs {
  0 [label="7s 0b"];
  1 [label="3s 0b"];
    }
    subgraph cluster_reissuance {
  7 [label=""];
  8 [label=""];
    }
    subgraph cluster_outputs {
  2 [label="-3s 0b"];
  3 [label="-3s 0b"];
  4 [label="-2s 0b"];
  5 [label="-1s 0b"];
  6 [label="-1s 0b"];
    }
  {
    edge [color=blue, fontcolor=blue];
    0 -> 7 [label="5s"];
    7 -> 2 [label="3s"];
    7 -> 3 [label="3s"];
    0 -> 4 [label="2s"];
    1 -> 8 [label="2s"];
    1 -> 7 [label="1s"];
    8 -> 5 [label="1s"];
    8 -> 6 [label="1s"];
  }
}
```

It worth to note that there are multiple ways to get the exact same set of credentials, for example this is equivalent but requires one less reissuance:


```{.dot render="dot -Tsvg -o %o.svg %i"
         img="../../src/images/wasabito_dependency_graph_alternative"
         out="/home/lontivero/Documents/Wiki/src/images" }
digraph G {
    subgraph cluster_inputs {
  0 [label="7s 0b"];
  1 [label="3s 0b"];
    }
    subgraph cluster_outputs {
  2 [label="-3s 0b"];
  3 [label="-3s 0b"];
  4 [label="-2s 0b"];
  5 [label="-1s 0b"];
  6 [label="-1s 0b"];
    }
    subgraph cluster_reissuance {
  7 [label=""];
    }
  {
    edge [color=blue, fontcolor=blue];
    0 -> 7 [label="6s"];
    0 -> 5 [label="1s"];
    7 -> 2 [label="3s"];
    7 -> 3 [label="3s"];
    1 -> 4 [label="2s"];
    1 -> 6 [label="1s"];
  }
}
```

## Alternative implementation for Wasabito

It could worth to explore an alternative system where there is not a _credential reissuance concept_ nor _mechanism_
for the only purpose of breaking/consolidating credentials but instead, have this as part of the _output 
registration_ mechanism.


