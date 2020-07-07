# System.Collections.Navigation

This project provides a tool called `Position<T>` for navigating structures like `T[]`, `Span<T>`, `IList<T>`, `string` and `StringSegment`. It is a similar concept to `Span<T>`, but it holds only a scalar position that points into a sequence of items and provides various operations around it.

The capabilities of a `Position<T>` are:
 - navigate a sequence of items, move forward/backward, seek to a specific item, and other various operations
 - track the current index
 - peek in or out of range safely
 - provide read (and write) access to the item pointed by the index
 - manipulate (writeable) sequences, insert, replace, remove items
 - provide access to the items before and after the index
 - allocation free operation, as it is built as a `ref struct`

## Example

### Finding in strings

```cs
var email = "user@example.org";
var position = email.GetPositionOfStart();
position.MoveForwardTo('@'); // "user"
var user = position.AllBefore();
var hostname = position.AllAfter();  // "example.org"
position.MoveForwardTo('.');
var tld = position.AllAfter(); // "org"
```

## Limitations
 - Not all operations supported on each type of sequence. For example:
   - writing a `string`
   - slicing a `IList<T>`