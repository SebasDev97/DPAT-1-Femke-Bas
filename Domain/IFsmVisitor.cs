namespace Domain;

public interface IFsmVisitor
{
    void Visit(StateMachine stateMachine);
    void Visit(CompoundState state);
    void Visit(SimpleState state);
    void Visit(InitialState state);
    void Visit(FinalState state);
    void Visit(Transition transition);
}
