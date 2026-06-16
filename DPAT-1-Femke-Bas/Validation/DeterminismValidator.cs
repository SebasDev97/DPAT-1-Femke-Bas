using Domain;

namespace DPAT_1_Femke_Bas.Validation;

public class DeterminismValidator : IValidationStrategy
{
    public IEnumerable<ValidationError> Validate(StateMachine fsm)
    {
        foreach (var group in fsm.Transitions.GroupBy(t => t.Source.Identifier))
        {
            var transitions = group.ToList();
            if (transitions.Count <= 1) continue;

            // An automatic (triggerless) transition blocks all others from the same source.
            if (transitions.Any(t => t.Trigger == null))
            {
                yield return new ValidationError(
                    $"State '{group.Key}' has an automatic (triggerless) transition alongside " +
                    $"{transitions.Count - 1} other transition(s) — the others are unreachable.");
                continue;
            }

            // Duplicate trigger+guard pair = non-determinism.
            var seen = new HashSet<string>();
            foreach (var t in transitions)
            {
                var key = $"{t.Trigger!.Identifier}|{t.Guard?.Condition ?? ""}";
                if (!seen.Add(key))
                    yield return new ValidationError(
                        $"Non-determinism on state '{group.Key}': multiple transitions share " +
                        $"trigger '{t.Trigger.Identifier}' and guard '{t.Guard?.Condition ?? "(none)"}'.");
            }
        }
    }
}
