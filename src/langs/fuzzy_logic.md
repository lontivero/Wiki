---
title: Fuzzy Logic Example
...

# Fuzzy logic example

Here we implement an example from Youtube where bankers can use 
fuzzy logic to evaluate the level of risk of someone according to his/her credit score.

<iframe width="560" height="315" src="https://www.youtube.com/embed/__0nZuG4sTw" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>

# Run the code

You can play with this using the same number used in the youtube video.

```
$ dotnet fsi fuzzy_logic_example.fsx

Enter credit score: 660
(bad, Trapezoidal (0.0, 0.0, 550.0, 650.0)) => 0
(neutral, Triangular (550.0, 650.0, 750.0)) => 0.9
(good, Trapezoidal (650.0, 750.0, 1000.0, 1000.0)) => 0.1
defuzzified value: 0.4607142857142857
```

# Code

```{.fsx include="{{SOURCE_DIRECTORY}}/src/scripts/fuzzy_logic_example.fsx"}
```


