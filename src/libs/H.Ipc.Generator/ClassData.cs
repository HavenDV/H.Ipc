using Microsoft.CodeAnalysis;

namespace H.Generators;

public readonly record struct ClassData(
    string Namespace,
    string Name,
    string InterfaceName,
    IReadOnlyCollection<IMethodSymbol> Methods);