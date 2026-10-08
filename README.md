## General Form
Each line represents on instruction blank lines or lines that are entirely comments are not included in the token array and should not be counted when jumping.
Terms in a token are separated by spaces.
Comments are denoted by ‘#’ and anything after this will be ignored.
All variables are stored as BigInteger, no other types exist.

## Instructions
Declaration - Create a new int with a name and value. Format: int name expression
Assignment - Update the value of an existing variable. Format: name expression
Jump - Move the instruction head (current token). The amount to jump is an integer offset from the current Jump token, make sure not to count blank lines or comments when going forward or backward. Format: Jump expression amount
Function Call - Calls a function currently only function is Send to output values to the console. Format: Send expression

## How to use
Build the script and call the exe with 1 argument for the file location of the script

## Example Scripts

```
# Compute 10 factorial
int n 10
int result 1

result result * n
n n - 1
Jump n > 0 -2

Send result
```

```
# Initialise variables
int i 1
int fizz 0
int buzz 0

# Calculate divisibility flags
fizz 1 - i % 3
buzz 1 - i % 5

# Choose output branch
Jump fizz & buzz 5
Jump fizz 7
Jump buzz 8

# Neither fizz nor buzz
Send i
Jump 1 7

# Both fizz and buzz
Send 1111
Send 2222
Jump 1 4

# Fizz only
Send 1111
Jump 1 2

# Buzz only
Send 2222

# Next number
i i + 1
Jump i < 16 -14
```

```
#10 Fibonacci numbers
int a 0
int b 1
int next 0
int count 10

Send a

next a + b
a b
b next
count count - 1

Jump count -5
```

```
# Print all prime numbers from 2 to 30
int n 2
int d 2
int prime 1

Jump d < n 2
Jump prime 8
Jump n % d = 0 3
d d + 1
Jump 1 -4

prime 0
n n + 1
d 2
Jump 1 4

Send n
n n + 1
d 2
prime 1
Jump n < 31 -15
```