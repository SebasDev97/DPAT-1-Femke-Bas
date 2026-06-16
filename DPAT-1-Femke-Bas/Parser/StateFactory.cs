using Domain;

namespace DPAT_1_Femke_Bas.Parser;

public class StateFactory
{
    public StateComponent CreateState(StateType type, string identifier, string name) =>
        type switch
        {
            StateType.Initial  => new InitialState(identifier, name),
            StateType.Simple   => new SimpleState(identifier, name),
            StateType.Compound => new CompoundState(identifier, name),
            StateType.Final    => new FinalState(identifier, name),
            _ => throw new ArgumentException($"Unknown state type: {type}", nameof(type))
        };
}
