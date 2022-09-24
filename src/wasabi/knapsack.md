---
title: CoinJoins for dummies
subtitle: Introduction to the concepts multiparty transactions for anonymity
---

---
title: CoinJoin Anonymity Analysis
---

# Introduction

CoinJoin is a technique for combining multiple Bitcoin transactions into
a single one to make it more difficult for blockchain analyzers to
determine which the original transactions are.\
\
In this document we want to keep a record of our attempts to create a
general approach for analyzing the pros and cons of the different mixing
algorithms.

# Mixing

Let's say we have these two transactions:

::: center

<table style="border:0px solid transparent;">
<tr><th>Transaction #1</th><th>Transaction #2</th></tr>
<tr><td>
|   Inputs |  Outputs |
|---------:|---------:|
| $i = 25$ | $o = 49$ |
| $i = 31$ |  $o = 7$ |

</td><td>
|     Inputs |  Outputs |
|-----------:|---------:|
|  $i = 18$ | $o = 21$ |
| $i = 13$ | $o = 10$ |

</td></tr> </table>

:::

They can be combined in one transaction as follow:

::: center

<table>
<tr><th>Combined Transaction</th></tr>
<tr><td>
|     Inputs |     Outputs |
|-----------:|------------:|
| $i =   25$ | $o =     49$ |
|  $i =  31$ | $o =          7$ |
|  $i =  18$ | $o =         21$ |
| $i =   13$ | $o =         10$ |
</td></tr>
<tr><td>Figure 1</td></tr>
</table>

:::

# Analysis

We are going to focus specifically on blockchain analysis where the
analysts goal is attach identities to coins (outputs) and tracking them
using two well-known heuristics:

-   H1: All inputs to a transaction are owned by the same entity

-   H2: The one-time change address is controlled by the same entity as
    the input addresses

These heuristics cannot be applied to CoinJoin transactions but only to
single payment transactions instead. For that reason the analysts have
to unjoin the coinjoined transactions with the hope of finding the
original payment transactions to apply the heuristics individually.\
Side note: H1 is clearly a more robust assumption than H2.

## Unjoining coinjoined transactions

Let's take the coinjoin transaction in figure 1. That transaction can be
interpreted in many different ways but, *just for simplicity*, lets
assume that we know this is a coinjoin transaction between two
participants (this is not obvious) and that each transaction has no more
than two outputs. This is enough for extracting the original
transactions by grouping the outputs, the inputs and then match the sum
of both sets:\


Outputs:

    \#  OutputSet    Sum(OSet)      \#  OutputSet    Sum(OSet)
  ---- ----------- ----------- -- ---- ----------- -----------
     0    {49}              49       5  {49, 21}            70
     1     {7}               7       6  {49, 10}            59
     2    {21}              21       7   {7, 21}            28
     3    {10}              10       8   {7, 10}            17
     4   {49, 7}        **56**       9  {21, 10}        **31**

\
\
Inputs:

    \#  InputSet    Sum(ISet)      \#    InputSet      Sum(ISet)
  ---- ---------- ----------- -- ---- -------------- -----------
     0    {25}             25       7    {31, 18}             49
     1    {31}         **31**       8    {31, 13}             44
     2    {18}             18       9    {18, 13}         **31**
     3    {13}             13      10  {25, 31, 18}           74
     4  {25, 31}       **56**      11  {25, 31, 13}           69
     5  {25, 18}           43      12  {25, 18, 13}       **56**
     6  {25, 13}           38      13  {31, 18, 13}           62

\
\
Before continuing with the example, it worth to note that given the
coinjoin transaction has 4 inputs and that we know there are two
sub-transaction then, the maximum number of inputs a sub-transaction can
have is 3. These 3 inputs can be combined in 14 different ways. The
number of ways a set with of $n$ elements can be partitioned into
disjoint subsets is called 'Bell number' and it grows rapidly as the
value of $n$ increases.\
Having built this table it is possible now to extract all the possible
transactions. In this case we have 4 sub-transactions (#1, #2, #3 and
#4)

::: center

<table>
<tr><th colspan=2>Valid Transactions Set #1</th></tr>
<tr><th>Transaction #1</th><th>Transaction #2</th></tr>
<tr><td>
|           Inputs |      Outputs |
|-----------------:|-------------:|
|      $i =    25$ | $o =     49$ |
| $i =         31$ | $o =      7$ |

</td><td>
|          Inputs |  Outputs |
|----------------:|---------:|
|     $i =    18$ | $o = 21$ |
| $i =        13$ | $o = 10$ |

</td></tr> </table>


<table>
<tr><th colspan=2>Valid Transactions Set #2</th></tr>
<tr><th>Transaction #3</th><th>Transaction #4</th></tr>
<tr><td>
|          Inputs |      Outputs |
|----------------:|-------------:|
|     $i =    25$ | $o =     49$ |
| $i =        18$ | $o =      7$ |
| $i =        13$ |              |

</td><td>

|   Inputs |       Outputs |
|---------:|--------------:|
| $i = 31$ | $o =      21$ |
|          |    $o =   10$ |

</td></tr> </table>

:::

Also, we can see that if well all of these look perfectly valid, there
are only two valid sets of transactions that could result in the
original coinjoined transaction and in the image above we had wrapped
them together in the _Valid Transactions Set_ tables. They are

-   Transaction #1 and Transaction #2

-   Transaction #3 and Transaction #4

Clearly, other combinations are invalid because same inputs appears in
more than one sub-transaction.\
It is important to note that in this really simple example an analyzer
has only 0.5 probability of choosing the right original sub-transactions
set. In practice this does not use to happen but the good part is that
we can make it happen simply by choosing the outputs values (that's what
knapsack[^1] and traditional coinjoins do).\
Finally, it worth to have in mind that if well for the sake of
simplicity we said that we already knew the transaction had two
participants, this information is not available in advance in a
real-world transaction. The only thing we know is that given the
transaction has four inputs, there are at least one
participant and at most four.

## Linkeage 

However the analysis effectiveness can be greatly improved, at the point 
to completely solve the puzzle, by adding more information to the process.
By observing the sets of valid transactions we can see that outputs $\{o=49, o=7\}$
and $\{o=21, o=10\}$ are always together, that is known as the `output-to-output`
link and means that both are part of the same sub transaction.

In the same way it is evident that the set $\{\{i=25\}, \{o=49, o=7\}\}$
is always present, what is known as `input-to-output` link and tells us
that those are part of the same sub transaction too.

Finally, there exists the `input-to-input` links that reveal two or more
inputs belong to the same sub transaction. Unfortunatelly there is none
in the previous example.



## Math of combinations

Given *n* inputs, the number of all possible subsets of at most *k*
elements is
$$\sum_{1<k<n}\binom{n}{k} = \sum_{1<k<n}\frac{n!}{k!\left (n - k \right )!}$$
This can be rapidly calculated by using Pascal Triangle and summing the
elements of *nth* row from index 1 to k. For example, in our example we
have 4 inputs and we analyzed 14 subsets of at most 3 elements, we can
verify that 4 + 6 + 4.\

  ------- --- --- --- --- ---- ------- ---- ------- ---- ------- ---- --- --- --- --- ----- --
  $n=0$                                        1                                        1   
  $n=1$                                 1            1                                  2   
  $n=2$                           1            2            1                           4   
  $n=3$                    1            3            3            1                     8   
  $n=4$                1        **4**        **6**        **4**        1               16   
  $n=5$            1       5            10           10           5        1           32   
  $n=6$        1       6         15           20           15          6       1       64   
  $n=7$    1       7       21           35           35           21       7       1   128  
  ------- --- --- --- --- ---- ------- ---- ------- ---- ------- ---- --- --- --- --- ----- --

\
In the same way, and using the same example, we can check that given the
4 outputs ($n$), the number of subsets of at most $2 (k)$ outputs can be
calculated by summing the elements of the 4th row from index 1 to 2,
this is: 4 + 6.\
Lets call *n* to the number of all possible subsets of inputs and *m* to
the number of all possible subsets of outputs, the number of comparison
to perform is $n*m$. A more simple formula that will be useful is
$2^{n+m}$

## Equal-output coinjoin comparison

Lets say two people want to coinjoin together to obtain coins of the
exact same value (for this example lets say 10). Both provide their
transactions:

::: center

<table style="border:0px solid transparent;">
<tr><th>Transaction #1</th><th>Transaction #2</th></tr>
<tr><td>
|     Inputs | Outputs |
|-----------:|--------:|
| $i =     9$ | $o =     10$ |
| $i =         8$ | $o_c =      7$ |

</td><td>
|          Inputs |        Outputs |
|----------------:|---------------:|
|     $i =     6$ |   $o =     10$ |
| $i =         5$ | $o_c =      1$ |

</td></tr> </table>

:::

After join them, they get the following:

::: center

<table>
<tr><th>CoinJoin Transaction</th></tr>
<tr><td>
|    Inputs |          Outputs |
|----------:|-----------------:|
| $i =   8$ |     $o_1 =   10$ |
| $i =   9$ | $o_2 =       10$ |
| $i =   6$ | $o_c =        7$ |
| $i =   5$ | $o_c =        1$ |

</td></tr> </table>
:::

This coinjoin transaction can be analyzed in the exact same way that we
did before but in this case we don't need any of the previous
assumptions because, unlike the naive coinjoin transaction we used
initially, this transaction has a clear pattern and it is easy to know
the number of participants involved simply by counting the number of
indistinguishable outputs. Also, the change outputs are clearly
identifiable. Finally, extracting the possible sub-transactions is
trivial and doesn't require building big tables as before. Just take a
change value, add it 10 and look what inputs sum gives us that result.

::: center

<table>
<tr><th colspan=2>Valid Transactions Set #1</th></tr>
<tr><th>Transaction #1</th><th>Transaction #2</th></tr>
<tr><td>
|           Inputs |      Outputs |
|-----------------:|-------------:|
|      $i =    9$ | $o_1 =     10$ |
| $i =         8$ | $o_c =      7$ |

</td><td>
|          Inputs |  Outputs |
|----------------:|---------:|
|     $i =    6$ | $o_2 = 10$ |
| $i =        5$ | $o_c = 1$ |

</td></tr> </table>


<table>
<tr><th colspan=2>Valid Transactions Set #2</th></tr>
<tr><th>Transaction #3</th><th>Transaction #4</th></tr>
<tr><td>
|           Inputs |      Outputs |
|-----------------:|-------------:|
|      $i =    9$ | $o_2 =     10$ |
| $i =         8$ | $o_c =      7$ |

</td><td>
|         Inputs |    Outputs |
|---------------:|-----------:|
|     $i =    6$ | $o_2 = 10$ |
| $i =        5$ |  $o_c = 1$ |
</td></tr> </table>

:::

We can see in this example that the equal-outputs coinjoin transaction
provides 0.5 probability to track the \"anonymized\" coin.

## Complex example

Lets see the transaction *X* below and see what can we learn from it. At
first glance it consists of *six* inputs and *eight* where 5.39986 units
in and the same units out (no fees were paid). Assuming no addresses are
reused and same script types, there is nothing else we can conclude.

::: center

<table>
<tr><th>CoinJoin Transaction</th></tr>
<tr><td>
|      Inputs |     Outputs |
|------------:|------------:|
| i = 1.88841 | o = 1.89031 |
| i = 1.79381 | o = 1.81421 |
| i = 0.95031 | o = 0.92801 |
| i = 1.52261 | o = 0.66050 |
| i = 1.39181 | o = 0.96651 |
| i = 1.21961 | o = 1.11521 |
| i = 0.68991 | o = 1.81781 |
|             | o = 0.26391 |
</td></tr> </table>
:::

It is not obvious if it is a coinjoin transaction or a pay to many batch
transaction. Assuming it is a coinjoin, we know there are no more than
*seven* participants so, lets analyze the number of subsets we can split
it using the formula used above and calling the number of inputs $n$ and
the number of outputs $m$:

$$\sum_{1<k_i<n}\binom{n}{k_i} * \sum_{1<k_o<m}\binom{m}{k_o} = 126 * 254 = 32,004$$

The partitions analysis found 45 valid sub transactions[^2]. Moreover,
another interesting result is the number of participation of each
input/output in those extracted transactions:\

      Input  participation        Output  participation
  --------- --------------- -- --------- ---------------
    1.52261       31             1.89031       31
    1.21961       31             1.81421       33
    1.39181       33             0.92801       33
    0.68991       33             0.66050       29
    1.88841       31             0.96651       31
    1.79381       31             1.11521       31
    0.95031       31             1.81781       31
                                 0.26391       31

\
\
Even when there is no much that we can conclude from this, intuitively
we can say that this level of ambiguity is highly unusual. After all,
what are the chances that almost every input and every output
participate in $70\%$ of the 45 sub-transactions? In fact, the existence
of such a number of transactions is also unusual (intuitively again).\

[^1]: https://www.comsys.rwth-aachen.de/fileadmin/papers/2017/2017-maurer-trustcom-coinjoin.pdf

[^2]: https://github.com/lontivero/Knapsack/blob/master/data/knapsack-3-participants-demo.txt
