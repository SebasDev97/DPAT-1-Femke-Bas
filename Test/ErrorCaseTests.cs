using Domain;
using DPAT_1_Femke_Bas.Parser;

namespace Test;

[TestFixture]
public class ErrorCaseTests
{
    private static StateMachine ParseText(string content) =>
        new FsmFileParser().Parse(new StringReader(content), "test");

    [Test] public void UnknownKeyword_ThrowsFormatException()
        => Assert.Throws<FormatException>(() => ParseText("BLORP foo _ \"bar\" : SIMPLE;"));

    [Test] public void TruncatedStateLine_ThrowsFormatException()
        => Assert.Throws<FormatException>(() => ParseText("STATE foo;"));

    [Test] public void InvalidStateType_ThrowsFormatException()
        => Assert.Throws<FormatException>(() => ParseText("STATE foo _ \"bar\" : POTATO;"));

    [Test] public void TransitionReferencesNonExistentState_ThrowsInvalidOperation()
        => Assert.Throws<InvalidOperationException>(() => ParseText(
            "STATE a _ \"A\" : SIMPLE;\nTRANSITION t1 a ghost;"));

    [Test] public void TransitionReferencesNonExistentTrigger_ThrowsInvalidOperation()
        => Assert.Throws<InvalidOperationException>(() => ParseText("""
            STATE a _ "A" : SIMPLE;
            STATE b _ "B" : SIMPLE;
            TRANSITION t1 a b ghost_trigger;
            """));

    [Test] public void TransitionActionOnUnknownTransition_ThrowsInvalidOperation()
        => Assert.Throws<InvalidOperationException>(() => ParseText(
            "STATE a _ \"A\" : SIMPLE;\nACTION nope \"desc\" : TRANSITION_ACTION;"));

    [Test] public void EntryActionOnCompound_ThrowsInvalidOperation()
        => Assert.Throws<InvalidOperationException>(() => ParseText("""
            STATE outer _ "Outer" : COMPOUND;
            STATE inner outer "Inner" : SIMPLE;
            ACTION outer "entry" : ENTRY_ACTION;
            """));

    [Test] public void ChildDefinedBeforeParent_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => ParseText("""
            STATE child parent "Child" : SIMPLE;
            STATE parent _ "Parent" : COMPOUND;
            """));
    }

    [Test] public void DuplicateIdentifier_ThrowsFormatException()
    {
        var ex = Assert.Throws<FormatException>(() =>
            ParseText("STATE a _ \"First A\" : SIMPLE;\nSTATE a _ \"Second A\" : SIMPLE;"));
        Assert.That(ex!.Message, Does.Contain("Duplicate identifier"));
    }

    [Test] public void FileNotFound_ThrowsFileNotFoundException()
        => Assert.Throws<FileNotFoundException>(() =>
            new FsmFileParser().Parse("/tmp/does_not_exist_xyz.txt"));
}
