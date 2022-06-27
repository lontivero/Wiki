---
title: Python
...


# Fee rates under mempool pressure simulation 

```python

import sys, random, math

def hours(hrs):
    return hrs * minutes(60)

def minutes(mins):
    return 60 * mins

def megabytes(mega):
    return mega * 1024 * 1024


def get_next_block_elapsed_time():
    return int(random.gauss(minutes(10), minutes(1.5)))

# parameters
days = 1
txs_per_second = 8
block_size_mb = megabytes(1.1)
mempool_max_size_mb = megabytes(300)

tip = 0
time = 0;
next_block_time = get_next_block_elapsed_time()
mempool = []
mempool_size = 0
buckets = int(days * 20) * [1]

while time < days * hours(24):
    target = int(random.gauss(3, 1))
    estimated_fee_rate = buckets[target];
    fee_rate = random.uniform(0.75 * estimated_fee_rate, 1.5 * estimated_fee_rate)
    tx_size = int(random.gauss(500, 100))
    fee = int(tx_size * fee_rate)
    amount = int(random.uniform(0.0005, 10) * 100_000_000)
    if fee < (amount / 100):
#        if mempool_size > mempool_max_size_mb:
#            mempool.sort(key=lambda tx: tx["paid_fee"], reverse=True)
#            mempool = mempool[:(len(mempool) // 10) * 8]
#            mempool_size = sum(tx["size"] for tx in mempool)
        if mempool_size < mempool_max_size_mb:
            mempool.append({"paid_fee": fee_rate, "broadcasted_block_time": tip, "size": tx_size})
            mempool_size = mempool_size + tx_size

        if int(time) == next_block_time:
            cur_mempool = len(mempool)
            mempool.sort(key=lambda tx: tx["paid_fee"], reverse=True)
            tx_index = 0
            block_size = 0
            while tx_index < len(mempool) and (block_size < block_size_mb):
                block_size = block_size + mempool[tx_index]["size"]
                tx_index = tx_index + 1
            block, mempool = mempool[:tx_index], mempool[tx_index:]
            buckets_t = len(buckets) * [[]]
            for tx in block:
                mempool_size = mempool_size - tx["size"]
                past_blocks = tip - tx["broadcasted_block_time"]
                buckets_t[past_blocks] = buckets_t[past_blocks] + [tx["paid_fee"]]
                buckets[past_blocks] = sum(buckets_t[past_blocks]) / len(buckets_t[past_blocks])
            print(tip+1, block_size, cur_mempool, len(mempool))

            tip = tip + 1
            next_block_time = next_block_time + get_next_block_elapsed_time()
    time = time + (1 / txs_per_second)

print(buckets)
#for fee in iter(buckets):
#    print("{:.8f}".format(fee))

```
