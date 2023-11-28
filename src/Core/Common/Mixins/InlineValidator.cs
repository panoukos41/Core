namespace FluentValidation;

public static class InlineValidator
{
    public static IValidator<T> For<T>(Action<InlineValidator<T>> data)
    {
        var validator = new InlineValidator<T>();
        data(validator);
        return validator;
    }
}
