using Domain;
using System.Text;

namespace DPAT_1_Femke_Bas.Visitor;

public class TextRenderVisitor : IFsmVisitor
{
    private readonly StringBuilder _stringBuilder = new StringBuilder();

    public void Visit(StateMachine stateMachine) 
    { 
        _stringBuilder.AppendLine("Rendering StateMachine:");
        
        foreach (var state in stateMachine.RootStates)
        {
            state.Accept(this);
        }
    }
    
    public void Visit(CompoundState state) 
    { 
        _stringBuilder.AppendLine("- CompoundState");

        foreach (var childState in state.Children) 
        {
            childState.Accept(this);
        }
    }
    
    public void Visit(SimpleState state) 
    { 
        _stringBuilder.AppendLine("- SimpleState");
    }
    
    public void Visit(InitialState state) 
    { 
        _stringBuilder.AppendLine("- InitialState");
    }
    
    public void Visit(FinalState state) 
    { 
        _stringBuilder.AppendLine("- FinalState");
    }
    
    public void Visit(Transition transition) 
    { 
        _stringBuilder.AppendLine("  -> Transition");
    }

    public string GetRenderedText()
    {
        return _stringBuilder.ToString();
    }
}
