namespace Domain;

public class StateMachine(string name, List<StateComponent> rootStates, List<Transition> transitions)
{
    public string Name { get; } = name;
    public IReadOnlyList<StateComponent> RootStates { get; } = rootStates.AsReadOnly();
    public IReadOnlyList<Transition> Transitions { get; } = transitions.AsReadOnly();

    public StateComponent? Find(string identifier)
    {
        return RootStates.Select(root => root.Find(identifier)).OfType<StateComponent>().FirstOrDefault();
    }

    public void Accept(IFsmVisitor visitor) => visitor.Visit(this);
}
