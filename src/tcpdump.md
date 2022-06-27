---
title: How to capture network traffic
...

Use `tcpdump`. You will need to `sudo` by sure. Example:

```bash
tcpdump -i lo -s 0 -w tcp-traffic-dump.pcap
```


* `-i <interface>`  `lo` for local, wlp4s0 for my wan
* `-w <file>`       saves the packet data to the file for later analysis (with wireshark for example)
* `-s <snaplen>`    how many bytes of data to take for each package. The default value is 262144 and should be enough in most of the cases. (`-s 0` capture the full packet)
* `-XX`             print/dump the data of each packet in hex and ASCII
* `-A`              print each packet in ASCII. Handly for capturing web traffic.
* `-n`              do not convert addresses to names. Do not use dns

## Capture all bitcoin traffic

Use `tcpdump --list-interfaces` to list the available interfaces first. Then call the script with the right interface. For example:


```bash
$ WIFI=$(tcpdump --list-interfaces | grep Running | grep Wireless | cut -d' ' -f1 | cut -d'.' -f 2)
$ ./bitdump.sh $WIFI
```

Here the script:


``` bash
#!/usr/bin/env sh

SELF=`basename $0`

if [[ $1 = "" ]]; then
  INTERFACE="en1"
  echo "$SELF: no interface name provided as argument: using $DEFAULT as default"
else
  INTERFACE=$1
fi

connected_nodes() {
  netstat -an |
  awk '/8333/ && /ESTA/ { print $5 }' |
  sed 's/[.:]8333//'
}

# tcpdump config
IF="-i $INTERFACE"
ASCII="-A"
BINARY="-XX"
NO_DNS="-n"
FULL_PACKETS="-s 0"
NODES=($(connected_nodes))
PORT="8333"
FILE="bitcoin-traffic-dump-$(date --iso-8601=minutes).pcap"

if [[ -z "$NODES" ]]; then
  echo "$SELF: No peer found, check your internet connection and that Bitcoin is running"
else
  for (( i = 0; i < ${#NODES[*]}; i++ )); do
    ANY_NODE="host ${NODES[i]} and $ANY_NODE"
  done

  # -t : don't print a timestamp on each dump line
  # -q : stay quiet

  tcpdump \
    $IF \
    $FILE \ 
    $FULL_PACKETS \
    $BINARY \
    $NO_DNS \
    $ANY_NODE \
    port $PORT

fi

```

### For NixOS

```bash
nix-shell -p tcpdump
```
