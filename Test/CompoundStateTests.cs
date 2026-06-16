using Domain;

namespace Test;

[TestFixture]
public class CompoundStateTests
{
    [Test]
    public void AddChild_SetsParentCorrectly()
    {
        var parent = new CompoundState("parent", "Parent");
        var child = new SimpleState("child", "Child");

        parent.AddChild(child);

        Assert.That(child.Parent, Is.SameAs(parent));
        Assert.That(parent.Children, Contains.Item(child));
    }

    [Test]
    public void RemoveChild_ClearsParent()
    {
        var parent = new CompoundState("parent", "Parent");
        var child = new SimpleState("child", "Child");
        parent.AddChild(child);

        parent.RemoveChild(child);

        Assert.That(child.Parent, Is.Null);
        Assert.That(parent.Children, Does.Not.Contain(child));
    }

    [Test]
    public void AddChild_Duplicate_ThrowsInvalidOperationException()
    {
        var parent = new CompoundState("parent", "Parent");
        var child = new SimpleState("child", "Child");
        parent.AddChild(child);

        Assert.Throws<InvalidOperationException>(() => parent.AddChild(child));
    }

    [Test]
    public void AddChild_Self_ThrowsInvalidOperationException()
    {
        var compound = new CompoundState("c", "C");
        Assert.Throws<InvalidOperationException>(() => compound.AddChild(compound));
    }

    [Test]
    public void FullPath_AndDepth_CorrectForNestedStates()
    {
        var root = new CompoundState("root", "Root");
        var mid = new CompoundState("mid", "Mid");
        var leaf = new SimpleState("leaf", "Leaf");

        root.AddChild(mid);
        mid.AddChild(leaf);

        Assert.That(root.FullPath, Is.EqualTo("root"));
        Assert.That(root.Depth,    Is.EqualTo(0));
        Assert.That(mid.FullPath,  Is.EqualTo("root/mid"));
        Assert.That(mid.Depth,     Is.EqualTo(1));
        Assert.That(leaf.FullPath, Is.EqualTo("root/mid/leaf"));
        Assert.That(leaf.Depth,    Is.EqualTo(2));
    }

    [Test]
    public void Find_RecursivelyLocatesDescendant()
    {
        var root = new CompoundState("root", "Root");
        var child = new CompoundState("child", "Child");
        var grandchild = new SimpleState("grand", "Grand");
        root.AddChild(child);
        child.AddChild(grandchild);

        Assert.That(root.Find("grand"), Is.SameAs(grandchild));
        Assert.That(root.Find("child"), Is.SameAs(child));
        Assert.That(root.Find("nope"),  Is.Null);
    }
}
