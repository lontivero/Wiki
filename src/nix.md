---
title: Nixops - deploy to VirtualBox 
...

This will create a VM in `VirtualBox` and deploy web server (and an openssh server) inside it.

# VirtualBox

First thing to do it to install `VirtualBox`. Modify the nixos `configuration.nix` and add the following lines:

File: **configuration.nix**

```
  nixpkgs.config.allowUnfree = true;

  virtualisation.virtualbox.host.enable = true;
  virtualisation.virtualbox.host.enableExtensionPack = true;
  users.extraGroups.vboxusers.members = [ "lontivero" ];
```

The `allowUnfree = true` is because the extension pack is not free. I am not sure the extension pack is needed, anyway.  

Then `sudo nixos-rebuild switch` 

# Deployment

## Defining the deployment

Two files need to be created:

File: **trivial.nix**

```
{
  network.description = "The Simplest Web server";
  webserver =
    { config, pkgs, ... }:
    {
      services.openssh = {
        enable = true;
        passwordAuthentication = false;
        permitRootLogin = "yes";
      };
      #users.users."root".openssh.authorizedKeys.keys = [
      #   "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQCXHUJtICvHLT2LOPy01ITN8B8xyCM3Qcu5v3bUGQ8VTvP+/jQ2wJy1xITiyTY52f7jPCYys6aSDKpzeGhXuH5l1OtS+P34JmlYsRgHq1/akqApo+gtpDH+aCwfTk8SRgcnqxDI+aJOeGrGGnGxTaWnjpUVIItxRoQrMNXK482FxUwo6Mgk8lv5Gije45nFIjEaq1mk6UiF0Lyr5zwYhgo44A7biqSxaWqo1q63TgX2ckwT7Uv/imWNzJ0Vuowq3Ki7DLyEjb9uuccY4KV6Jhj96LtiuDoJMsi5qTnwb2mZBgUWfSvSTcPCT4NSiSnT/nSwD6TVuflo7J3fWFkez73luVqbI4lDwgOPspBgvduAVqD5FxpP/ObCjGKZ36ThSSsRk8hFY/xUFKyubXLKHTG/z+qFESLdIOdIs1758eb7y3ivdNT5cy6cj3vJdLLqmni8bbKx/rx79z9DRNDIWSNNx90BIe9yEvK2G7ev4DbCSMKcJziHYpNuPd5g8n4el1SWlz5dE8mp7GckGEsAZ2Zyf6S81/5p5D95PrCY5ejNYsmditlj3TeeMzgnKqcGdg+Rtz1qkvHozZMfb9EPm40Z4+zQlYYR8syxfBBcA9LEQ1FO6PDqneCDLJPkutgqGyoEp2l1Uy6I3XsqhWeSTxslBJoDucR947l7mUfqzrlb6Q== lontivero@lontivero-MAX-G5"
      #];

      services.httpd.enable = true;
      services.httpd.adminAddr = "alice@example.org";
      networking.firewall.allowedTCPPorts = [ 22 80 ];

      services.httpd.logFormat = "combined";
      services.httpd.virtualHosts = {
        "main" = {
          documentRoot = "/tmp";
        };
      };
    };
}
```

**trivial-vbox.nix**

```
{
  webserver =
    { config, pkgs, ... }:
    { deployment.targetEnv = "virtualbox";
      deployment.virtualbox.memorySize = 1024; # megabytes
      deployment.virtualbox.vcpu = 2; # number of cpus
      deployment.virtualbox.headless = true;
    };
}
```

**NOTE:** using `headless = true` prevents the `VirtualBox` GUI console to appears, what is exactly what we want, but we can open it manually anyway and see the VM. 

## Creating the deployment

This of all we have to install `nixops`.

### Install Nixops

Installing nixops fails because it is marked as insecure so, the easiest way to bypass that is as follow:

```
$ NIXPKGS_ALLOW_INSECURE=1 nix-shell -p nixops

```

### Create and deploy

```
$ nixops create ./trivial.nix ./trivial-vbox.nix -d trivial
$ nixops deploy -d trivial 
```

At this point we should have a VM running, however it can happen that `nixops` fails to connect to the VM through SSH with `too many attemps`. To fix this add `indentitiesOnly yes` to the `.ssh/config` file. 

### SSH session

We can get access to the VM using:

```
$ nixops ssh -d trivial webserver
```

```
$ nixops info -d trivial
$ nixops destroy -d trivial
$ nixops delete -d trivial
$ rm -rf ~/nixops
```

**Test it**

```
$ curl 192.168.56.111
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">
<html>
 <head>
  <title>Index of /</title>
 </head>
 <body>
<h1>Index of /</h1>
  <table>
   <tr><th valign="top"><img src="/icons/blank.gif" alt="[ICO]"></th><th><a href="?C=N;O=D">Name</a></th><th><a href="?C=M;O=A">Last modified</a></th><th><a href="?C=S;O=A">Size</a></th><th><a href="?C=D;O=A">Description</a></th></tr>
   <tr><th colspan="5"><hr></th></tr>
   <tr><th colspan="5"><hr></th></tr>
</table>
</body></html>
```

