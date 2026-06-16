namespace Domain;

public class Action
{
    public string OwnerIdentifier { get; }
    public string Name { get; }
    public ActionType Type { get; }

    public Action(string ownerIdentifier, string name, ActionType type)
    {
        OwnerIdentifier = ownerIdentifier;
        Name = name;
        Type = type;
    }
}
