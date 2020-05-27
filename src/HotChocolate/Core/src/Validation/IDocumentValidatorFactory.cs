namespace HotChocolate.Validation
{
    public interface IDocumentValidatorFactory
    {
        IDocumentValidator CreateValidator(string? schemaName = null);
    }
}
