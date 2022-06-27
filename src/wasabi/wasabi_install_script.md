---
title: Wasabi 2.0.1 install script
...


This script downloads the `.tar.gz` file from GitHu as well as the `.asc` file containing the GPG signature
that allow verifying the signature, verifies the signature is correct and uncompress the file.

```bash
set -e
wget https://github.com/zkSNACKs/WalletWasabi/releases/download/v2.0.1/WasabiLinux-2.0.1.tar.gz{,.asc}
gpg --keyserver pgp.mit.edu --recv-keys B4B72266C47E075E
gpg --verify WasabiLinux-1.1.0.tar.gz{.asc,}
tar -xf WasabiLinux-1.1.0.tar.gz
```
