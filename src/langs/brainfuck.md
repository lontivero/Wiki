---
title: Brainfuck interpreter
...

My Brainfuck language interpreter in exactly 19 LOC.

## The code

```coffeescript
run=(code, input)->
	[dptr, cptr, marks, mem, output, stack]=[0, 0, [], (0 for [1..100]), [], []]

	for cmd, pos in code
		if cmd is '[' then stack.push(pos)
		if cmd is ']' then marks[marks[pos] = stack.pop()]=pos  
  
	while cptr < code.length
		switch code[cptr]
			when '+' then mem[dptr] = mem[dptr] + 1 or 1
			when '-' then mem[dptr] = mem[dptr] - 1 or 0
			when '>' then dptr++
			when '<' then dptr--
			when '[' then cptr = marks[cptr] if mem[dptr] is 0
			when ']' then cptr = marks[cptr] if mem[dptr] isnt  0
			when '.' then output.push(String.fromCharCode(mem[dptr]))
			when ',' then c=input.shift(); mem[dptr] = c.charCodeAt(0) if c instanceof string
		cptr++
	output
```

## Examples


### Fibonacci

```brainfuck
alert run(""">++++++++++>+>+[
    [+++++[>++++++++<-]>.<++++++[>--------<-]+<<<]>.>>[
        [-]<[>+<-]>>[<<+>+>-]<[>+<-[>+<-[>+<-[>+<-[>+<-[>+<-
            [>+<-[>+<-[>+<-[>[-]>+>+<<<-[>+<-]]]]]]]]]]]+>>>
    ]<<<
]""").join('')
```

### Hello World

```brainfuck
alert run("++++++++++[>+++++++>++++++++++>+++>+<<<<-]>++.>+.+++++++..+++.>++.<<+++++++++++++++.>.+++.------.--------.>+.>.").join('')
```
