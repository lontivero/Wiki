---
title: Wasabi 2.0.1 install script
...


This script downloads the `.zip` file from GitHu as well as the `.asc` file containing the GPG signature
that allow verifying the signature, verifies the signature is correct and uncompress the file.

```bash
set -e
wget https://github.com/zkSNACKs/WalletWasabi/releases/download/v2.0.1.0/Wasabi-2.0.1-linux-x64.zip{,.asc}
gpg --keyserver pgp.mit.edu --recv-keys B4B72266C47E075E
gpg --verify Wasabi-2.0.1-linux-x64.zip{.asc,}
unzip Wasabi-2.0.1-linux-x64.zip -d WasabiWallet
```
