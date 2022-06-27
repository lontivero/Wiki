---
title: Bash
toc: false
---

### Delete the last N lines

```bash
$ echo -e "a\nb\nc" | head -n -1
a
b
$ echo -e "a\nb\nc" | head -n -2
a
```

### Run multiple processes write to the same pipe

```bash
{ (echo zzz; echo aaa; echo kkk) } | sort
aaa
kkk
zzz
```
