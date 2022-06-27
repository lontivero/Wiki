---
title: Bitcoin
...

## Create raw sefl-spending transaction

```bash
bbcli="bitcoin-cli -regtest -datadir=/home/lontivero/tmp/bitcoind"
miner_address=$($bbcli getnewaddress)
address=$($bbcli getnewaddress)
generated_blocks=$($bbcli generatetoaddress 101 $miner_address)
txid=$($bbcli listunspent | jq '.[].txid')
unsigned_raw_tx=$($bbcli createrawtransaction "[{\"txid\":\"$txid\", \"vout\":0}]" "[{\"$address\":30}]")

$bbcli testmempoolaccept "[\"$unsigned_raw_tx\"]"
``` 
