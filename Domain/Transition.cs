namespace Domain;

public class Transition(
    string identifier,
    StateComponent source,
    StateComponent destination,
    Trigger? trigger,
    Guard? guard,
    Action? effect = null)
{
    public string Identifier { get; } = identifier;
    public StateComponent Source { get; } = source;
    public StateComponent Destination { get; } = destination;
    public Trigger? Trigger { get; } = trigger;
    public Guard? Guard { get; } = guard;
    public Action? Effect { get; } = effect;

    public void Accept(IFsmVisitor visitor) => visitor.Visit(this);
}
