using Domain;

namespace DPAT_1_Femke_Bas.Validation;

public interface IValidationStrategy
{
    IEnumerable<ValidationError> Validate(StateMachine fsm);
}
