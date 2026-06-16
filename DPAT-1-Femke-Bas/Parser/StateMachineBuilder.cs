using Domain;

namespace DPAT_1_Femke_Bas.Parser;

public class StateMachineBuilder
{
    private string _name = "Unnamed";
    private readonly StateFactory _factory = new();

    private readonly Dictionary<string, StateComponent> _states = new();
    private readonly List<StateComponent> _rootStates = new();
    private readonly Dictionary<string, Trigger> _triggers = new();

    // Buffered definitions, resolved in Build().
    private readonly List<(string ownerId, string description, ActionType type)> _pendingActions = new();
    private readonly List<(string id, string sourceId, string destId, string? triggerId, string? guard)> _transitionDefs = new();

    public StateMachineBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public StateMachineBuilder AddState(string identifier, string? parentIdentifier, string name, StateType type)
    {
        if (_states.ContainsKey(identifier))
            throw new InvalidOperationException($"Duplicate identifier '{identifier}': each state must have a unique identifier.");

        var component = _factory.CreateState(type, identifier, name);
        _states[identifier] = component;

        if (parentIdentifier == null)
        {
            _rootStates.Add(component);
        }
        else
        {
            if (!_states.TryGetValue(parentIdentifier, out var parentComponent))
                throw new InvalidOperationException(
                    $"Parent '{parentIdentifier}' not found when adding state '{identifier}'. Parents must be defined before their children.");
            if (parentComponent is not CompoundState compound)
                throw new InvalidOperationException(
                    $"Parent '{parentIdentifier}' is not a CompoundState and cannot have children.");
            compound.AddChild(component);
        }

        return this;
    }

    public StateMachineBuilder AddTrigger(string identifier, string description)
    {
        _triggers[identifier] = new Trigger(identifier, description);
        return this;
    }

    public StateMachineBuilder AddAction(string ownerIdentifier, string description, ActionType type)
    {
        _pendingActions.Add((ownerIdentifier, description, type));
        return this;
    }

    public StateMachineBuilder AddTransition(string identifier, string sourceId, string destId,
        string? triggerId, string? guard)
    {
        _transitionDefs.Add((identifier, sourceId, destId, triggerId, guard));
        return this;
    }

    public StateMachine Build()
    {
        var transitionEffects = BuildTransitionEffects();
        var transitions = BuildTransitions(transitionEffects);
        ValidateTransitionEffects(transitions, transitionEffects);
        ApplyStateActions();
        return new StateMachine(_name, _rootStates, transitions);
    }

    private static void ValidateTransitionEffects(
        List<Transition> transitions,
        Dictionary<string, Domain.Action> effects)
    {
        var builtIds = transitions.Select(t => t.Identifier).ToHashSet();
        foreach (var id in effects.Keys.Where(id => !builtIds.Contains(id)))
            throw new InvalidOperationException(
                $"TRANSITION_ACTION references unknown transition '{id}'.");
    }

    private Dictionary<string, Domain.Action> BuildTransitionEffects()
    {
        var effects = new Dictionary<string, Domain.Action>();
        foreach (var (ownerId, description, type) in _pendingActions)
        {
            if (type != ActionType.TransitionAction) continue;
            effects[ownerId] = new Domain.Action(ownerId, description, type);
        }
        return effects;
    }

    private List<Transition> BuildTransitions(Dictionary<string, Domain.Action> effects)
    {
        var transitions = new List<Transition>();
        foreach (var (id, sourceId, destId, triggerId, guard) in _transitionDefs)
        {
            if (!_states.TryGetValue(sourceId, out var source))
                throw new InvalidOperationException(
                    $"Transition '{id}': source state '{sourceId}' not found.");
            if (!_states.TryGetValue(destId, out var dest))
                throw new InvalidOperationException(
                    $"Transition '{id}': destination state '{destId}' not found.");

            Trigger? trigger = null;
            if (triggerId != null && !_triggers.TryGetValue(triggerId, out trigger))
                throw new InvalidOperationException(
                    $"Transition '{id}': trigger '{triggerId}' not found.");

            Guard? guardObj = string.IsNullOrEmpty(guard) ? null : new Guard(guard);
            effects.TryGetValue(id, out var effect);

            transitions.Add(new Transition(id, source, dest, trigger, guardObj, effect));
        }
        return transitions;
    }

    private void ApplyStateActions()
    {
        foreach (var (ownerId, description, type) in _pendingActions)
        {
            if (type == ActionType.TransitionAction) continue;

            if (!_states.TryGetValue(ownerId, out var state))
                throw new InvalidOperationException(
                    $"Action references unknown state '{ownerId}'.");
            state.AddAction(new Domain.Action(ownerId, description, type));
        }
    }
}
