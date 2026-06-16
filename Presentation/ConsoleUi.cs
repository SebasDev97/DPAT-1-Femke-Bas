using Domain;
using DPAT_1_Femke_Bas.Visitor;

namespace Presentation;

public class ConsoleUi
{
    public void Render(StateMachine stateMachine)
    {
        var visitor = new TextRenderVisitor();
        stateMachine.Accept(visitor);
        
        Console.WriteLine(visitor.GetRenderedText());
    }
}