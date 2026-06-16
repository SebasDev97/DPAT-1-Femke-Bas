namespace Domain;

public class CompoundState(string identifier, string name) : StateComponent(identifier, name)
{
    private readonly List<StateComponent> _children = new();

    public IReadOnlyList<StateComponent> Children => _children.AsReadOnly();

    public void AddChild(StateComponent child)
    {
        if (child == this)
            throw new InvalidOperationException($"State '{Identifier}' cannot be a child of itself.");
        if (_children.Contains(child))
            throw new InvalidOperationException($"State '{child.Identifier}' is already a child of '{Identifier}'.");
        _children.Add(child);
        child.SetParent(this);
    }

    public void RemoveChild(StateComponent child)
    {
        if (_children.Remove(child))
            child.SetParent(null);
    }

    public override StateComponent? Find(string identifier)
    {
        if (Identifier == identifier) return this;
        foreach (var child in _children)
        {
            var found = child.Find(identifier);
            if (found != null) return found;
        }
        return null;
    }

    public override void Accept(IFsmVisitor visitor) => visitor.Visit(this);
}
