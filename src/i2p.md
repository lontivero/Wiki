---
title: I2P playground
subtitle: How to try new things with I2P
header-includes: |
  <style>
  pre { max-height: 300px; }
  </style>
...

# Install I2P

In nixos there are two packages: i2p and i2pd. Lets use the c++ version.

```bash
$ nix-shell -p i2pd
```

And now run it with http proxy enabled

```bash
$ i2pd --httpproxy.enabled 1
```

Finally test it is running (4444 is the port for http)

```bash
$ curl --proxy http://127.0.0.1:4444 http://jynrjzdp5qajf2jsdif6bgpijuk72zmuiohq3o7omeksw4pdgejq.b32.i2p
```



