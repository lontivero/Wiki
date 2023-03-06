---
title: Reduce the number of Tor circuits
subtitle: A possible solution for reusing circuits
...

# Problem

## The Wasabi coordinator as the enemy

Wasabi is an Open Source software project and as such anyone can glone the repository, compile the project and deploy its own coordinator.
Nothing prevent a developer to modify the code and deploy a modified version on its own infrastructure and that's why Wasabi is developed
with the mindset that there could be multiple coordinators and that the users should need to trust on those or, in better terms, that the
Wasabi client should protect the users by assuming that the coordinator that it is connected to is malicious.

## Client and Tor circuit building 

Wasabi client must then not only to make sure also that a coordinator cannot correlate a sequence of requests to a single identity (IP address)
and it uses a different Tor circuit for each request which identity isolation have to be guarranted.

However, Tor circuits creation is expensive in terms of network roundtrips and CPU, what forced the Wasabi team to introduce many mechanism/
optimizations to reduce the number and frequence of circuit creation. 


# Current approach

Currently Wasabi uses stream isolation, this is one stream by circuit what also meand that you need to build as many circuits as streams you need.
But lets introduce some concepts first and discuss more about this after.


## What is a Tor circuit?

A Tor circuit is simple a chain of onion routers, tipically is consists on a sequence of three routers: an entry guard router, a middle router
and the exit router. However that is only the composition used by default and a circuit can be build with more and less routers.


In the graph below you can see how it would looks like to connect to the Wasabi coordinator using three different identities (circuits).

Circuits:

* Entry guard -> node 1 -> node 2 -> Coordinator
* Entry guard -> node 11 -> node 5 -> Coordinator
* Entry guard -> node 10 -> node 11 -> Coordinator


```{.dot render="dot -Gsize='10,10' -Tsvg -o %o.svg %i"
         img="../../src/images/three_tor_circuits"
         out="/home/lontivero/Documents/Wiki/src/images" }
digraph G {
	fontname="Helvetica,Arial,sans-serif"
	node [fontname="Helvetica,Arial,sans-serif"]
	edge [fontname="Helvetica,Arial,sans-serif"]

	style=filled;
	color=lightgrey;
	node [style=filled,color=white];
	edge [style=dotted]
	uc [label="user computer"];
	n0 [label = "entry guard"];
	nx [label = "coordinator"];
	uc -> n0 
	n1 -> n3 -> n4 -> n5 -> n6 -> n7 -> n8;
	n2 -> n3;
	n2 -> n5 -> n8;
	n0 -> n3 -> n7;
	n1 -> n9 -> n10; n11 -> n12 ->n13;
	n2 -> n10; n5 ->n12; n5 -> n13; n10 -> n3; n11 -> n7; n8 -> n13; n13 -> n8;
	n13 -> n3;
	n11 -> n9; n4 -> n9;
	
    n1 [shape="circle"; style="filled"; color="grey"]
    n2 [shape="circle"; style="filled"; color="grey"]
    n5 [shape="circle"; style="filled"; color="grey"]
    n10 [shape="circle"; style="filled"; color="grey"]
    n11 [shape="circle"; style="filled"; color="grey"]
    n0 -> n1 -> n2 -> nx [arrowsize=1.5; style="solid"];
    n0 -> n11 -> n5 -> nx [arrowsize=1.5; style="solid"];
    n0 -> n10 ->  n11 -> nx[arrowsize=1.5; style="solid"];
}
```

The coordinator receives http requests from three different nodes: node 2, node 5 and node 11. In other words, the coordinator only knows the IP addresses of those routers.


## TCP stream

An stablished circuit can encapsulate multiple tcp streams independently of their final destination. However, it is important to know that if well the tipical mantal image 
of a Tor circuit looks like a pipe with two holes, one entry and one exit, the reality is more like a pipe with multiple holes that leak. 

<table><tr>
<th>Tipical stream routing</th>
<th>Alternative stream routing</th>
</tr>
<tr><td>
```{.dot render="dot -Tsvg -o %o.svg %i"
         img="../../src/images/tor_streams_tipical"
         out="/home/lontivero/Documents/Wiki/src/images" }
digraph G {
	fontname="Helvetica,Arial,sans-serif"
	node [fontname="Helvetica,Arial,sans-serif"]
	edge [fontname="Helvetica,Arial,sans-serif"]

	style=filled;
	color=lightgrey;
	node [style=filled,color=white];
	edge [style=dotted]
	uc [label="user computer"];
	n0 [label = "entry guard"];
	nx0 [label = "destination 1"];
	nx1 [label = "destination 2"];
	nx2 [label = "destination 3"];
	uc -> n0 
	
    n1 [shape="circle"; style="filled"; color="grey"]
    n2 [shape="circle"; style="filled"; color="grey"]
    n0 -> n1 -> n2 -> nx0 [arrowsize=1.5; style="solid"];
    n2 -> nx1 [arrowsize=1.5; style="solid"];
    n2 -> nx2 [arrowsize=1.5; style="solid"];
}
```
</td>
<td>
```{.dot render="dot -Tsvg -o %o.svg %i"
         img="../../src/images/tor_streams_holes"
         out="/home/lontivero/Documents/Wiki/src/images" }
digraph G {
	fontname="Helvetica,Arial,sans-serif"
	node [fontname="Helvetica,Arial,sans-serif"]
	edge [fontname="Helvetica,Arial,sans-serif"]

	style=filled;
	color=lightgrey;
	node [style=filled,color=white];
	edge [style=dotted]
	uc [label="user computer"];
	n0 [label = "entry guard"];
	nx0 [label = "destination 1"];
	nx1 [label = "destination 2"];
	nx2 [label = "destination 3"];
	uc -> n0 
	
    n1 [shape="circle"; style="filled"; color="grey"]
    n2 [shape="circle"; style="filled"; color="grey"]
    n3 [shape="circle"; style="filled"; color="grey"]
    n4 [shape="circle"; style="filled"; color="grey"]
    n0 -> n1 -> n2 -> n3 -> n4 -> nx0 [arrowsize=1.5; style="solid"];
    n2 -> nx1 [arrowsize=1.5; style="solid"];
    n3 -> nx2 [arrowsize=1.5; style="solid"];
    n4 -> nx3 [arrowsize=1.5; style="solid"];
}
```
</td>
</tr>
<tr>
<td>One circuit, three streams to three different destinations. Same IP address.</td>
<td>One circuit, three streams to three different destinations. Different IP addresses.</td>
</tr>
</table>


# Proposal

Once we know that circuits can have more than only three routers and that more than one routers can act as exit routers
it is easy to imagine how the three circuits ilustrated in the first image can be replaces by only one circuit with three
exits like the one below:


```{.dot render="dot -Gsize='10,10' -Tsvg -o %o.svg %i"
         img="../../src/images/one_tor_circuit"
         out="/home/lontivero/Documents/Wiki/src/images" }

digraph G {
    graph [width="10"]
	fontname="Helvetica,Arial,sans-serif"
	node [fontname="Helvetica,Arial,sans-serif"]
	edge [fontname="Helvetica,Arial,sans-serif"]

	style=filled;
	color=lightgrey;
	node [style=filled,color=white];
	edge [style=dotted]
	uc [label="user computer"];
	n0 [label = "entry guard"];
	nx [label = "coordinator"];
	uc -> n0 
	n1 -> n3 -> n4 -> n5 -> n6 -> n7 -> n8;
	n2 -> n3;
	n2 -> n5 -> n8;
	n0 -> n3 -> n7;
	n1 -> n9 -> n10; n11 -> n12 ->n13;
	n5 ->n12; n5 -> n13; n10 -> n3; n11 -> n7; n8 -> n13; n13 -> n8;
	n13 -> n3;
	n11 -> n9; n4 -> n9;
	
    n1 [shape="circle"; style="filled"; color="grey"]
    n2 [shape="circle"; style="filled"; color="grey"]
    n5 [shape="circle"; style="filled"; color="grey"]
    n10 [shape="circle"; style="filled"; color="grey"]
    n11 [shape="circle"; style="filled"; color="grey"]
    n0 -> n1 -> n2 -> nx [arrowsize=1.5; style="solid"];
    
    n11 -> n5 -> nx [arrowsize=1.5; style="solid"];
    n0 -> n11;
    n0 -> n10;
    n10 -> n11 -> nx [arrowsize=1.5; style="solid"];
    n2 -> n10 [arrowsize=1.5; style="solid"];
}         
```
