namespace H.Ipc.Generator;

public static class EnumerableExtensions
{
    public static string Inject(this IEnumerable<string> values)
    {
        return string.Concat(values)
            .TrimStart('\r', '\n')
            .TrimEnd('\r', '\n');
    }
}