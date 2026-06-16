namespace Domain;

public class SimpleState : StateComponent
{
    private readonly List<Action> _entryActions = new();
    private readonly List<Action> _doActions = new();
    private readonly List<Action> _exitActions = new();

    public SimpleState(string identifier, string name) : base(identifier, name) { }

    public IReadOnlyList<Action> EntryActions => _entryActions.AsReadOnly();
    public IReadOnlyList<Action> DoActions => _doActions.AsReadOnly();
    public IReadOnlyList<Action> ExitActions => _exitActions.AsReadOnly();

    public override void AddAction(Action action)
    {
        switch (action.Type)
        {
            case ActionType.EntryAction: _entryActions.Add(action); break;
            case ActionType.DoAction:    _doActions.Add(action);    break;
            case ActionType.ExitAction:  _exitActions.Add(action);  break;
            default:
                throw new InvalidOperationException(
                    $"Action type {action.Type} cannot be added to a state. Use ENTRY_ACTION, DO_ACTION, or EXIT_ACTION.");
        }
    }

    public override void Accept(IFsmVisitor visitor) => visitor.Visit(this);
}
