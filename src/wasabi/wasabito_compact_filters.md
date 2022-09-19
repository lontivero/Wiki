---
title: Compact filters
toc: false
---


Analizing 270013 filters....

```{.fsx icmd="fsx" }
open System
let filters = 270013.0
let data = [
  100 , 21  
  200 , 56 
  300 , 75 
  400 , 99 
  500 , 129 
  600 , 156 
  700 , 168 
  800 , 212 
  900 , 226 
  1000, 259 
  1100, 310 
  1200, 315 
  1300, 332 
  1400, 399 
  1500, 411 
  1600, 446 
  1700, 459 
  1800, 420 
  1900, 472 
  2000, 517 
  2100, 557 
  2200, 569 
  2300, 592 
  2400, 553 
  2500, 612 
  2600, 649 
  2700, 679 
  2800, 771 
  2900, 742 
  3000, 770 
  3100, 800 
  3200, 864 
  3300, 822 
  3400, 868 ]

Console.WriteLine("| Wallet Size | False Positives | Saving  | Aprox. Download |")
Console.WriteLine("|-------------|-----------------|---------|-----------------|")
for wz, fp in data do
  Console.WriteLine($"| {wz} | {fp} | {1.0 - (float(fp) / filters)} | {float(fp) * 1.7 / 1000.0} GB |")
```
