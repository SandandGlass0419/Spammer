# Spammer
Spamming Reimagined. The only tool you need for Spamming Anywhere. **_Windows_ Only!!**

## Command Line Arguments
**Usage:** `Spammer "content_path" "repeat_time" "sleep_time"`\
`"content_path"`: Path to your spamming content.  
`"repeat_time"`: Times to repeat your spamming content.  
`"sleep_time"`: how long to sleep between each repetition. (ms)  

## How should I write my spamming content?
Each line represents one entry. After typing every entry, the Enter key is pressed.\
If you want to add special actions to your spamming content, you can use the following parameters:\
Anything that isn't a parameter will be typed as is. (even if it has a % character)
* `%endl`: Press Shift + Enter. Use this to add new lines in one entry.
* `%ctrlv`: Press Ctrl + V. Use this to paste whatever is copied.
* `%up`: Press Up Arrow. Use this to cycle through past entries. (if it's supported on your spamming environment)
* `%rint`: Replaces the parameter with a random int.
* `%rfloat`: Replaces the parameter with a random float.
* `%rtime`: Replaces the parameter with a random time.
* `%rrgb`: Replaces the parameter with a random hex code. (starting with #)
* `%rguid`: Replaces the parameter with a random guid.\

### Example
> _content.txt_\
> Hello,%endlWorld!\
> This is a random int!: %rint

cmd: `Spammer content.txt 2 0`\
Result:
> Hello,\
> World!\
> This is a random int!: 12345\
> Hello,\
> World!\
> This is a random int!: -564673
