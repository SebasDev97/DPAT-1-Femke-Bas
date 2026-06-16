using Domain;

namespace DPAT_1_Femke_Bas.Validation;

public class CompoundTargetValidator : IValidationStrategy
{
    public IEnumerable<ValidationError> Validate(StateMachine fsm)
    {
        foreach (var t in fsm.Transitions.Where(t => t.Destination is CompoundState))
            yield return new ValidationError(
                $"Transition '{t.Identifier}' targets compound state '{t.Destination.Identifier}' " +
                "— transitions must target a leaf state within it, not the compound state itself.");
    }
}
