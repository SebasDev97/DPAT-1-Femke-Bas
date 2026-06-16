using Domain;

namespace DPAT_1_Femke_Bas.Validation;

public class ValidationRunner(IEnumerable<IValidationStrategy> strategies)
{
    private readonly IReadOnlyList<IValidationStrategy> _strategies = strategies.ToList().AsReadOnly();

    public IReadOnlyList<ValidationError> RunAll(StateMachine fsm) =>
        _strategies.SelectMany(s => s.Validate(fsm)).ToList().AsReadOnly();
}
