---
title: Haskell
...


## WabiSabi toy implementation 

```haskell
module WabiSabi where

data GE = GE Integer Integer | Infinity
  deriving Show
 
instance S.Semigroup GE where
  (<>) = (⊕)

instance M.Monoid GE where
  mempty = Infinity
  mappend = (S.<>)

instance Eq GE where
  p == q = dist p q == 0
    where
      dist Infinity Infinity = 0
      dist Infinity _        = 1
      dist _        Infinity = 1
      dist (GE x1 y1) (GE x2 y2) = (x2-x1)^2 + (y2-y1)^2
 
inv :: GE -> GE
inv Infinity = Infinity
inv (GE x y) = GE x (-y)

(⊕) :: GE -> GE -> GE
(⊕) Infinity p = p
(⊕) p Infinity = p
(⊕) p@(GE x1 y1) q@(GE x2 y2)
  | p == inv q = Infinity
  | p == q     = mkGE $ 3*x1^2 `div` (2*y1)
  | otherwise  = mkGE $ (y2 - y1) `div` (x2 - x1)
    where
      mkGE l = let x = (l^2 - x1 - x2) `mod` nn
                   y = (l*(x1 - x) - y1) `mod` pp
                in GE x y

pp :: Integer
pp = 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F 
nn :: Integer
nn = 0xFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141 

g :: GE
g = GE 
    0x79BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798 
    0x483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8

(⋅) :: Integer -> GE -> GE
(⋅) n p
  | n == 0 = Infinity
  | n == 1 = p
  | n == 2 = p ⊕ p
  | n < 0  = inv ((-n) ⋅ p)
  | even n = 2 ⋅ ((n `div` 2) ⋅ p)
  | odd  n = p ⊕ ((n -1) ⋅ p)
  | otherwise = p

onCurve :: GE -> Bool
onCurve Infinity = False
onCurve (GE x y) =
  (y^2 - x^3) `mod` pp == 7

data Proof = Proof [GE] [Integer]
    deriving Show

challenge :: [GE] -> Integer
challenge gs = x 
    where (GE x _) = foldr1 (⊕) gs

prove :: [Integer] -> [Integer]-> [GE] -> Proof
prove ws rs gs = Proof nonces s
  where
    nonces = zipWith (⋅) rs gs
    e      = challenge gs
    eg     = map (*e) ws
    s      = zipWith (-) rs eg 

verify :: [GE] -> [GE] -> Proof -> Bool
verify ps gs (Proof rs ss) = lhe == rhe
  where
    e      = challenge $ rs ++ gs
    lhe    = foldr1 (⊕) $ zipWith (⋅) ss gs
    rhe    = foldr1 (⊕) $ rs ++ (map (\x-> e⋅x) ps)


proofExp :: Integer -> Proof
proofExp w = prove [w] [73623] [g]
```

