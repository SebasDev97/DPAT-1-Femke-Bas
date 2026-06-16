namespace DPAT_1_Femke_Bas;

using Domain;
using DPAT_1_Femke_Bas.Parser;
using Presentation;

class Program
{
    static void Main(string[] args)
    {
        // 1. Inlezen van de configuratiefile en het StateMachine object opbouwen
        var parser = new FsmFileParser();
        var stateMachine = parser.Parse("example_lamp.fsm");
        
        // 2. State Machine doorgeven aan je presentatielaag
        var consoleUi = new ConsoleUi();
        consoleUi.Render(stateMachine);
    }
}