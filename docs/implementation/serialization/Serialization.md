# Bad Script Serialization Format Documentation

## Serializer Hints
### Default
Enables Caching of String Elements in the script. This should drastically decrease the serialized file size as each string is only saved once
### NoCache
Disables string caching, file size might be bigger.

## Serializing
The BSSerializer.Serialize Method takes an array of BSExpressions, a stream instance and [Serializer Hints](#serializer-hints).
When invoked, the expressions and a small header will be serialized to the specified Stream.

## Deserializing
The BSSerializer.Deserialize method takes a stream instance and deserializes the serialized header and the serialized expressions

## Information loss
When Serializing, the Source Positions of the Expressions will get lost and replaced with SourcePosition.Unknown on deserialization.
If Optimizations are enabled, the Expression tree gets optimized before the serialization.