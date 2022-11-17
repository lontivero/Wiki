
## Delete elements 

```
cat trace.speedscope.json | jq 'del(.profiles[] | select(.name != "Thread (3536717) (.NET ThreadPool)"))' > trace.speedscope.cache.json

```
