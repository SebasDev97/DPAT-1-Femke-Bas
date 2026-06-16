namespace Domain;

public class InitialState(string identifier, string name) : StateComponent(identifier, name)
{
    public override void Accept(IFsmVisitor visitor) => visitor.Visit(this);
}
