using Domain;
using DPAT_1_Femke_Bas.Parser;

namespace Test;

[TestFixture]
public class FsmFileParserTests
{
    private StateMachine _fsm = null!;

    private const string ToggleLampFsm = """
        STATE initial _ "powered off" : INITIAL;
        STATE powered _ "Powered up" : COMPOUND;
        STATE off powered "Lamp is off" : SIMPLE;
        STATE on powered "Lamp is on" : SIMPLE;
        STATE final _ "powered off" : FINAL;
        TRIGGER power_on "turn power on";
        TRIGGER push_switch "Push switch";
        TRIGGER power_off "turn power off";
        ACTION on "Turn lamp on" : ENTRY_ACTION;
        ACTION off "Turn lamp off" : EXIT_ACTION;
        ACTION t2 "reset off timer" : TRANSITION_ACTION;
        TRANSITION t1 initial off power_on;
        TRANSITION t2 off on push_switch "time off > 10s";
        TRANSITION t3 on off push_switch;
        TRANSITION t4 powered final power_off;
        """;

    private static StateMachine ParseText(string content, string name = "test") =>
        new FsmFileParser().Parse(new StringReader(content), name);

    [OneTimeSetUp]
    public void ParseOnce() => _fsm = ParseText(ToggleLampFsm, "toggle-lamp");

    [Test]
    public void ParseFSM1_RootStates_AreInitialPoweredFinal()
    {
        var rootIds = _fsm.RootStates.Select(s => s.Identifier).ToList();
        Assert.That(rootIds, Is.EquivalentTo(["initial", "powered", "final"]));
    }

    [Test]
    public void ParseFSM1_Powered_IsCompoundWithChildrenOffAndOn()
    {
        var powered = _fsm.Find("powered") as CompoundState;
        Assert.That(powered, Is.Not.Null);

        var childIds = powered!.Children.Select(c => c.Identifier).ToList();
        Assert.That(childIds, Is.EquivalentTo(["off", "on"]));
    }

    [Test]
    public void ParseFSM1_ParentChains_AreCorrect()
    {
        var powered = _fsm.Find("powered") as CompoundState;
        var off = _fsm.Find("off");
        var on = _fsm.Find("on");

        Assert.That(off!.Parent, Is.SameAs(powered));
        Assert.That(on!.Parent,  Is.SameAs(powered));
        Assert.That(_fsm.Find("initial")!.Parent, Is.Null);
        Assert.That(_fsm.Find("final")!.Parent,   Is.Null);
    }

    [Test]
    public void ParseFSM1_FourTransitions_WithCorrectSourcesAndDestinations()
    {
        Assert.That(_fsm.Transitions.Count, Is.EqualTo(4));

        var t4 = _fsm.Transitions.First(t => t.Identifier == "t4");
        Assert.That(t4.Source.Identifier,      Is.EqualTo("powered"));
        Assert.That(t4.Destination.Identifier, Is.EqualTo("final"));
    }

    [Test]
    public void ParseFSM1_T2_HasTriggerAndGuard()
    {
        var t2 = _fsm.Transitions.First(t => t.Identifier == "t2");
        Assert.That(t2.Trigger, Is.Not.Null);
        Assert.That(t2.Trigger!.Identifier, Is.EqualTo("push_switch"));
        Assert.That(t2.Guard, Is.Not.Null);
        Assert.That(t2.Guard!.Condition, Is.EqualTo("time off > 10s"));
    }

    [TestCase("t1")]
    [TestCase("t3")]
    [TestCase("t4")]
    public void ParseFSM1_Transition_HasNoGuard(string transitionId)
    {
        var t = _fsm.Transitions.First(x => x.Identifier == transitionId);
        Assert.That(t.Guard, Is.Null);
    }

    [Test]
    public void ParseFSM1_Actions_CoupledCorrectly()
    {
        var stateOn  = _fsm.Find("on")  as SimpleState;
        var stateOff = _fsm.Find("off") as SimpleState;
        var t2       = _fsm.Transitions.First(t => t.Identifier == "t2");

        Assert.That(stateOn!.EntryActions, Has.Count.EqualTo(1));
        Assert.That(stateOn.EntryActions[0].Name, Is.EqualTo("Turn lamp on"));

        Assert.That(stateOff!.ExitActions.Count, Is.EqualTo(1));
        Assert.That(stateOff.ExitActions[0].Name, Is.EqualTo("Turn lamp off"));

        Assert.That(t2.Effect, Is.Not.Null);
        Assert.That(t2.Effect!.Name, Is.EqualTo("reset off timer"));
    }
}
