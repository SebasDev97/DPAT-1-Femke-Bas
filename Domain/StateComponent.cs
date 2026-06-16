namespace Domain;

public abstract class StateComponent(string identifier, string name)
{
    public string Identifier { get; } = identifier;
    public string Name { get; } = name;
    public CompoundState? Parent { get; private set; }

    internal void SetParent(CompoundState? parent) => Parent = parent;

    public string FullPath => Parent == null ? Identifier : $"{Parent.FullPath}/{Identifier}";

    public int Depth => Parent == null ? 0 : Parent.Depth + 1;

    public StateComponent Root => Parent == null ? this : Parent.Root;

    // Base implementation: a leaf matches only itself.
    // CompoundState overrides to also search children recursively.
    public virtual StateComponent? Find(string identifier) =>
        Identifier == identifier ? this : null;

    public virtual void AddAction(Action action) =>
        throw new InvalidOperationException(
            $"State '{Identifier}' ({GetType().Name}) does not support ENTRY/DO/EXIT actions.");

    public abstract void Accept(IFsmVisitor visitor);
}
