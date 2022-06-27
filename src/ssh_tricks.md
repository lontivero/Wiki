---
title: SSH tricks
...

## Basics

### Run remote command

```bash
ssh zk-testing -t 'tail -1000 ~/.walletwasabi/backend/Logs | grep whatever'
```

To run complex remote shell cmds over ssh, without escaping quotes

```bash
ssh host -l user $(<cmd.txt)
```

Much simpler method. More portable version: ssh host -l user “`cat cmd.txt`”

#### Download a file

```bash
scp zk-testing:./.bitcoin/mempool.dat ./mempool.dat
```

## Create a persistent connection to a machine

Create a persistent ssh connection so all future sessions will reuse it. 

* `-f` Requests ssh to go to background just before command execution.
* `-M` Places  the  ssh  client into ``master'' mode for connection sharing.

```bash
ssh -MNf zk-testing
```

## Tunneling

### Port forward testnet RPC

- `-L local_socket:host:hostport`
- `-N` Do not execute a remote command.  This is useful for just forwarding ports. 

```bash
ssh -N -L 18332:localhost:18332 zk-testing 
```

### Port forward testnet P2P

```bash
ssh -N -L 8332:localhost:8332 zk-testing 
```

## Edit remote files (Vim)

First, create a persistent connection to the server.

```bash
ssh -MNf zk-testing
```

**Note:** you can use `-v` to see what is going on. Also, his command line can be run in background finishing the command line wih `&` but it will not ask you the password :(

### Edit remote files

#### Using relative paths

```bash
vim scp://zk-testing/report.sh
```

It is also okay to open a direcory

```bash
vim scp://zk-testing/
```

#### Using absolute paths

```bash
vim scp://zk-testing//home/user/report.sh
```

## Enable password-less ssh login

Copy ssh key to the server. 

```bash
ssh-copy-id zk-testing
```

### Copy your ssh public key to a server from a machine that doesn’t have ssh-copy-id

```bash
cat ~/.ssh/id_rsa.pub | ssh user@machine “mkdir ~/.ssh; cat >> ~/.ssh/authorized_keys”
```

### Transfer ssh public key to another machine in one step

```bash
ssh-keygen; ssh-copy-id zk-testing; ssh zk-testing
```

This command sequence allows simple setup of (gasp!) password-less SSH logins. Be careful, as if you already have an SSH keypair in your ~/.ssh directory on the local machine, there is a possibility ssh-keygen may overwrite them. ssh-copy-id copies the public key to the remote host and appends it to the remote account’s ~/.ssh/authorized_keys file. When trying ssh, if you used no passphrase for your key, the remote shell appears soon after invoking ssh user@host.

To generate the keys use the command `ssh-keygen`

## Copy directory from testing to production

```bash
ssh zk-testing "cd /somedir/tocopy/ && tar -cf – ." | ssh zk-producion "cd /samedir/tocopyto/ && tar -xf -"
```

Good if only you have access to host1 and host2, but they have no access to your host (so ncat won’t work) and they have no direct access to each other.

## Mount filesystem through ssh

```bash
sshfs name@server:/path/to/folder /path/to/mount/point
```

Will allow you to mount a folder security over a network.

## SSH connection through host in the middle

```bash
ssh -t reachable_host ssh unreachable_host
```

Unreachable_host is unavailable from local network, but it’s available from reachable_host’s network. This command creates a connection to unreachable_host through “hidden” connection to reachable_host.


