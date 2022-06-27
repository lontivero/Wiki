# Wasabi Command Line Interface (cli)
 
A tiny bash script to effortless interaction with Wasabi RPC Server.

## Prerequisites

```bash

chmod u+x wcli
```

## Examples

### Dump all the coins

```
$ ./wcli.sh listkeys | head -10 

fullkeypath       internal  keystate  label  p2wpkhscript                              pubkey                                                              pubkeyhash
84'/0'/0'/1/0     true      2         0      b0ba6bb14314bacd1f908eb2b9ecc74e0b041717  039d67f2c7c3dd1ed0ac301e677fe3abf6f059067796553211d562f46f2e420043  b0ba6bb14314bacd1f908eb2b9ecc74e0b041717
84'/0'/0'/1/1     true      2         0      6b470de643697581b2717e6d2b878470a26d5e83  02efaf8fddc729e51826439ebac6e5782f60890262fd6c0702f537062bc4fe00f2  6b470de643697581b2717e6d2b878470a26d5e83
84'/0'/0'/1/2     true      2         0      6e16974cb396a40fed7a0beaa561dcc411056843  0230a07c357acc2d1ff18f7d9efbd4d33fb18e5d57d65402a6a454e5afe7c03078  6e16974cb396a40fed7a0beaa561dcc411056843
84'/0'/0'/1/3     true      2         0      af3a7f07b296d3523f0d6860c6ca232bbcb8c87f  02b6aca09cb632ac208dd2396a74dc20a67b955faf6688211a08584bc291470018  af3a7f07b296d3523f0d6860c6ca232bbcb8c87f
84'/0'/0'/1/4     true      2         0      469d19d3db1967ffc54e9f73f66568d5370e7fd3  032c92e3dd33a79795fbc494ee49212a12e0e052d7342921288b9d2be9ea5c06bf  469d19d3db1967ffc54e9f73f66568d5370e7fd3
```

### Dump the wallet history

```
$ ./wcli.sh gethistory | head -10 
datetime                   height   amount     tx                                                                islikelycoinjoin
2018-07-26T05:25:59+00:00  1355500  50000      6144f487705096397a39d2f0e7e23dd3e9c13d71e9cfc74b04e633eaf2f32a7a  false
2018-07-26T05:25:59+00:00  1355500  -555       ce41d4a5fe0c0da955210bf8722e43af8f580f87a005d7429272bfeb376426ea  false
2018-07-26T05:25:59+00:00  1355500  -720       5429cf93723f37af21f3c4ff5bb11e04af6e79a31aedcd4bbe93e9ae88989e1b  false
2018-07-26T05:25:59+00:00  1355500  -890       405822d6e574019c9f286b0f548d3824a3e305031556fa62a537b13745a0b0a0  false
2018-08-02T17:33:52+00:00  1356647  -17835     a98cca3e7e920bc97b85c17eb256bcaeabf7dec84491a8dfda8850ec4ec9bebf  false
```

### Generates a new address 

```
$ ./wcli.sh getnewaddress "Ricardo"                                                                           9 changed files  Fixes/HwiProcessBridge 
{
  "address": "tb1q4glx2d8j9l86ltsxf9smdnkkdef8ut835zzvzg",
  "keyPath": "84'/0'/0'/0/100",
  "label": [
    "Ricardo"
  ],
  "publicKey": "032de9254c8ea96cb6d83e680336ed3abb9b3095b69635c422106d9e5d4df07d72",
  "p2wpkh": "0014aa3e6534f22fcfafae064961b6ced66e527e2cf1"
}

```

## The Code

```bash
#!/usr/bin/env bash

function config_extract() {
    echo $(cat ~/.walletwasabi/client/Config.json | jq -r "$1")
}

ENABLED="$(config_extract '.JsonRpcServerEnabled')"
CREDENTIALS=$(config_extract '.JsonRpcUser + ":" + .JsonRpcPassword')
ENDPOINT=$(config_extract '.JsonRpcServerPrefixes[0]')

if [[ "$ENABLED" == "false" ]]; then
    echo "RPC server is disabled. Make sure to enable it by setting \"JsonRpcEnabled\":\"true\" in the Config.json file."
    quit
fi

if [[ "$CREDENTIALS" == ":" ]]; then
    BASIC_AUTH=""
else
    BASIC_AUTH="--user ${CREDENTIALS}"
fi


METHOD=$1
shift

if [ $# -ge 1 ]; then
    if [[ "$1" ]]; then
        PARAMS="\"$1\""
        shift
    else
        PARAMS="\"\""
        shift
    fi

    while (( "$#" )); do
        if [[ "$1" ]]; then
            PARAMS="$PARAMS, $1"
        else
            PARAMS='$PARAMS, ""'
        fi
        shift
    done
fi

REQUEST="{\"jsonrpc\":\"2.0\", \"id\":\"curltext\", \"method\":\"$METHOD\", \"params\":[$PARAMS]}"
RESULT=$(curl -s $BASIC_AUTH --data-binary "$REQUEST" -H "content-type: text/plain;" $ENDPOINT)
RESULT_ERROR=$(echo $RESULT | jq -r .error)

rawprint=(help)

if [[ "$RESULT_ERROR" == "null" ]]; then
    if [[ " ${rawprint[@]} " =~ " ${METHOD} " ]]; then
       echo $RESULT | jq -r .result
    else
        IS_ARRAY=$(echo $RESULT | jq -r '.result | if type=="array" then "true" else "false" end')
        if [[ "$IS_ARRAY" == "true" ]]; then
           echo $RESULT | jq -r '.result | [.[]| with_entries( .key |= ascii_downcase ) ]
                                         |    (.[0] |keys_unsorted | @tsv)
                                            , (.[]|.|map(.) |@tsv)' | column -t
        else
           echo $RESULT | jq  .result
        fi
    fi
else
   echo $RESULT_ERROR | jq -r .message
fi
```


