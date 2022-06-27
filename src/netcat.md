---
title: netcat tricks
...


### "Web server"

```bash
while true; do printf 'HTTP/1.1 200 OK\nContent-Length:%i\n\n%si\n' "$(wc -c < index.html)" "$(cat index.html)" | nc -l 8888; done
```

For static content the following is easier:

```bash
nc -l 8888 << EOF
HTTP/1.1 200 OK
Content-Length: 20

<h1>Hello world!</h1>
EOF
```
