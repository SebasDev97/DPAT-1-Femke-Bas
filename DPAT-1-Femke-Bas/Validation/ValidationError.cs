namespace DPAT_1_Femke_Bas.Validation;

public class ValidationError(string message)
{
    public string Message { get; } = message;
}
