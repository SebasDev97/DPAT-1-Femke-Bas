namespace Domain;

public class Trigger(string identifier, string name)
{
    public string Identifier { get; } = identifier;
    public string Name { get; } = name;
}
