using Domain;
using DPAT_1_Femke_Bas.Parser;

namespace Test;

[TestFixture]
public class StateMachineBuilderTests
{
    [Test]
    public void AddState_NullParent_AddsToRootStates()
    {
        var machine = new StateMachineBuilder()
            .WithName("Test")
            .AddState("s1", null, "State 1", StateType.Simple)
            .Build();

        Assert.That(machine.RootStates.Count, Is.EqualTo(1));
        Assert.That(machine.RootStates[0].Identifier, Is.EqualTo("s1"));
        Assert.That(machine.RootStates[0].Parent, Is.Null);
    }

    [Test]
    public void AddState_WithParent_AddsAsChildOfCompound()
    {
        var machine = new StateMachineBuilder()
            .WithName("Test")
            .AddState("parent", null, "Parent", StateType.Compound)
            .AddState("child",  "parent", "Child", StateType.Simple)
            .Build();

        var parent = machine.Find("parent") as CompoundState;
        var child  = machine.Find("child")  as SimpleState;

        Assert.That(parent, Is.Not.Null);
        Assert.That(child,  Is.Not.Null);
        Assert.That(parent!.Children, Contains.Item(child));
        Assert.That(child!.Parent, Is.SameAs(parent));
    }

    [Test]
    public void Build_MissingSourceState_ThrowsInvalidOperationException()
    {
        var builder = new StateMachineBuilder()
            .WithName("Test")
            .AddState("dest", null, "Dest", StateType.Simple)
            .AddTransition("t1", "missing", "dest", null, null);

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Test]
    public void Build_MissingTrigger_ThrowsInvalidOperationException()
    {
        var builder = new StateMachineBuilder()
            .WithName("Test")
            .AddState("s1", null, "S1", StateType.Simple)
            .AddState("s2", null, "S2", StateType.Simple)
            .AddTransition("t1", "s1", "s2", "ghost_trigger", null);

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Test]
    public void AddTransition_AutomaticTransition_NullTriggerAndEmptyGuard_BuildsCorrectly()
    {
        var machine = new StateMachineBuilder()
            .WithName("Auto")
            .AddState("a", null, "A", StateType.Simple)
            .AddState("b", null, "B", StateType.Simple)
            .AddTransition("t1", "a", "b", null, "")
            .Build();

        var t = machine.Transitions[0];
        Assert.That(t.Trigger, Is.Null);
        Assert.That(t.Guard,   Is.Null);
    }

    [Test]
    public void Build_TransitionActionForwardReference_ResolvedCorrectly()
    {
        var machine = new StateMachineBuilder()
            .WithName("Forward")
            .AddState("a", null, "A", StateType.Simple)
            .AddState("b", null, "B", StateType.Simple)
            .AddAction("t1", "do something", ActionType.TransitionAction)
            .AddTransition("t1", "a", "b", null, null)
            .Build();

        var effect = machine.Transitions[0].Effect;
        Assert.That(effect, Is.Not.Null);
        Assert.That(effect!.Name, Is.EqualTo("do something"));
        Assert.That(effect.Type, Is.EqualTo(ActionType.TransitionAction));
    }
}
