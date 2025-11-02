using TextCopy;

var c = new Clipboard();
await c.SetTextAsync("hello world");
var text = await c.GetTextAsync();
var g = 0;