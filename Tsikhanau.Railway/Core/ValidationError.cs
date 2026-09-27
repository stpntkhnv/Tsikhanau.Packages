namespace Tsikhanau.Railway;

public sealed class ValidationError : Error
{
    public IReadOnlyList<String> Messages { get; }
    
    private ValidationError(IReadOnlyList<String> messages) : base("VALIDATION", FormatMessage(messages))
    {
        Messages = messages;
    }
    
    public static ValidationError From(IEnumerable<String> messages)
    {
        var list = messages
            .Where(m => !String.IsNullOrWhiteSpace(m))
            .Distinct()
            .ToList();

        if (list.Count == 0)
        {
            throw new ArgumentException("ValidationError must contain at least one not empty message");
        }

        return new ValidationError(list);
    }

    public static ValidationError From(IEnumerable<Error> errors)
    {
        var list = errors
            .Select(x => x.Message)
            .Where(m => !String.IsNullOrWhiteSpace(m))
            .Distinct()
            .ToList();

        if (list.Count == 0)
        {
            throw new ArgumentException("ValidationError must contain at least one not empty message");
        }

        return new ValidationError(list);
    }
    
    public static ValidationError Single(String message) => new([message]);
    
    private static String FormatMessage(IEnumerable<String> messages) =>
        String.Join(";\n", messages);
}