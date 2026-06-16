using Domain;

namespace DPAT_1_Femke_Bas.Validation;

public class InitialFinalValidator : IValidationStrategy
{
    public IEnumerable<ValidationError> Validate(StateMachine fsm)
    {
        var allStates = Flatten(fsm.RootStates);
        var initialIds = allStates.OfType<InitialState>().Select(s => s.Identifier).ToHashSet();
        var finalIds   = allStates.OfType<FinalState>().Select(s => s.Identifier).ToHashSet();

        foreach (var t in fsm.Transitions)
        {
            if (initialIds.Contains(t.Destination.Identifier))
                yield return new ValidationError(
                    $"Transition '{t.Identifier}' targets initial state '{t.Destination.Identifier}' " +
                    "— initial states must not have incoming transitions.");

            if (finalIds.Contains(t.Source.Identifier))
                yield return new ValidationError(
                    $"Transition '{t.Identifier}' leaves final state '{t.Source.Identifier}' " +
                    "— final states must not have outgoing transitions.");
        }
    }

    private static IEnumerable<StateComponent> Flatten(IEnumerable<StateComponent> states)
    {
        foreach (var s in states)
        {
            yield return s;
            if (s is CompoundState compound)
                foreach (var child in Flatten(compound.Children))
                    yield return child;
        }
    }
}
