using Domain;

namespace DPAT_1_Femke_Bas.Visitor;

public class TextRenderVisitor : IFsmVisitor
{
    public void Visit(StateMachine stateMachine) { }
    public void Visit(CompoundState state) { }
    public void Visit(SimpleState state) { }
    public void Visit(InitialState state) { }
    public void Visit(FinalState state) { }
    public void Visit(Transition transition) { }
}
