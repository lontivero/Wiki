---
title: Create users in skSNACKs servers
...

## The Script

```bash
#!/bin/bash

USERNAME=$1
USER_HOME=/home/"$USERNAME"
shift 
SSH_PUBLIC_KEY="$*"
PASSWORD=$(cat /dev/urandom | tr -dc 'a-zA-Z0-9' | fold -w 32 | head -n 1)

echo =========================================================================================
echo Create user "$USERNAME" with password "$PASSWORD" in jumphost:
echo =========================================================================================
echo
echo JUMPHOST:
echo -------------------------------------------------------------
echo -n ssh jumphost -t \"
echo sudo useradd  "$USERNAME" --home-dir="$USER_HOME" --create-home --shell=/bin/rbash\;
echo sudo adduser "$USERNAME" jumpers\;
echo "echo $USERNAME:$PASSWORD | sudo chpasswd"\;
echo sudo mkdir /home/"$USERNAME"/.ssh\;
echo "echo $SSH_PUBLIC_KEY >> $USER_HOME/.ssh/authorised_keys"\;
echo sudo chown -R "$USERNAME":"$USERNAME" "$USER_HOME"/.ssh\;
echo echo "The account is setup"
echo \"
echo
echo TESTING:
echo -------------------------------------------------------------
echo -n ssh testing -t \"
echo sudo useradd  "$USERNAME" --home-dir="$USER_HOME" --create-home --shell=/bin/rbash\;
echo sudo usermod -aG sudo "$USERNAME"\;
echo "echo $USERNAME:$PASSWORD | sudo chpasswd"\;
echo sudo mkdir /home/"$USERNAME"/.ssh\;
echo "echo $SSH_PUBLIC_KEY >> $USER_HOME/.ssh/authorised_keys"\;
echo sudo chown -R "$USERNAME":"$USERNAME" "$USER_HOME"/.ssh\;
echo echo "The account is setup"
echo \"
```

## How to use

```
$ ./createuser.sh lucas thepublickeyishereblahblahblah

=========================================================================================
Create user lucas with password OVd3FUavbjT2IBNIJcAVJwYRVzAdByzL in jumphost:
=========================================================================================

JUMPHOST:
-------------------------------------------------------------
ssh jumphost -t "sudo useradd lucas --home-dir=/home/lucas --create-home --shell=/bin/rbash;
sudo adduser lucas jumpers;
echo lucas:OVd3FUavbjT2IBNIJcAVJwYRVzAdByzL | sudo chpasswd;
sudo mkdir /home/lucas/.ssh;
echo thepublickeyishereblahblahblah >> /home/lucas/.ssh/authorised_keys;
sudo chown -R lucas:lucas /home/lucas/.ssh;
echo The account is setup
"

TESTING:
-------------------------------------------------------------
ssh testing -t "sudo useradd lucas --home-dir=/home/lucas --create-home --shell=/bin/rbash;
sudo usermod -aG sudo lucas;
echo lucas:OVd3FUavbjT2IBNIJcAVJwYRVzAdByzL | sudo chpasswd;
sudo mkdir /home/lucas/.ssh;
echo thepublickeyishereblahblahblah >> /home/lucas/.ssh/authorised_keys;
sudo chown -R lucas:lucas /home/lucas/.ssh;
echo The account is setup
```
