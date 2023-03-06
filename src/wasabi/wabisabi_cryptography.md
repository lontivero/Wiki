---
title: WabiSabi cryptography documentations
---

# Linear relations

## Equation

`Equation` represents the equality $P = <x, G>$. Internally it is expressed as (P, G[]). 

$$
Verify(R, e, s[]) = 
\begin{cases}
    true &if <s, G> = R + e*P \\
    false &otherwise \\
\end{cases}
$$

## Statement

`Statement` is just a list of equations. Internally it is represented as `Equation[]`

* $Verify(R[], e, s[]) -->

* Knowledge is (Stmt, x)
