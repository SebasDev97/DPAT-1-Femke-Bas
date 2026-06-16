using Domain;
using DPAT_1_Femke_Bas.Parser;

namespace Test;

[TestFixture]
public class StateFactoryTests
{
    private StateFactory _factory = null!;

    [SetUp]
    public void Setup() => _factory = new StateFactory();

    [TestCase(StateType.Initial,  typeof(InitialState))]
    [TestCase(StateType.Simple,   typeof(SimpleState))]
    [TestCase(StateType.Compound, typeof(CompoundState))]
    [TestCase(StateType.Final,    typeof(FinalState))]
    public void CreateState_ReturnsCorrectSubtype(StateType type, Type expectedType)
    {
        var state = _factory.CreateState(type, "id", "Name");
        Assert.That(state, Is.InstanceOf(expectedType));
    }

    [Test]
    public void CreateState_SetsIdentifierAndName()
    {
        var state = _factory.CreateState(StateType.Simple, "s1", "Start");
        Assert.That(state.Identifier, Is.EqualTo("s1"));
        Assert.That(state.Name, Is.EqualTo("Start"));
    }

    [Test]
    public void CreateState_InvalidType_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _factory.CreateState((StateType)99, "x", "X"));
    }
}
