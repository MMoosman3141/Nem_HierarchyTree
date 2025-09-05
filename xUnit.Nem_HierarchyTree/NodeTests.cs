using Nem_HierarchyTree;
using System.Numerics;

namespace xUnit.Nem_HierarchyTree;
public class NodeTests {
  [Fact]
  public void Constructor_SetsPropertiesCorrectly() {
    Node node = new("TestNode") {
      BitFlag = 42
    };

    Assert.Equal("TestNode", node.Name);
    Assert.Equal((BigInteger)42, node.BitFlag);
    Assert.Equal((BigInteger)42, node.CheckValue);
    Assert.Equal(Guid.Empty, node.ParentId);
    Assert.Null(node.ParentNode);
    Assert.Empty(node.Children);
  }

  [Fact]
  public void AddChild_AddsChildAndUpdatesCheckValue() {
    Node parent = new("Parent") {
      Id = Guid.NewGuid(),
      BitFlag = 1,
    };
    Node child = new("Child") {
      Id = Guid.NewGuid(),
      BitFlag = 2,
    };

    bool result = parent.AddChild(child);

    Assert.True(result);
    Assert.Contains(child, parent.Children);
    Assert.Equal(parent.Id, child.ParentId);
    Assert.Equal(parent, child.ParentNode);
    Assert.Equal((BigInteger)3, parent.CheckValue);
  }

  [Fact]
  public void AddChild_DuplicateChild_ReturnsFalse() {
    Node parent = new("Parent") {
      Id = Guid.NewGuid(),
      BitFlag = 1,
    };
    Node child = new("Child") {
      Id = Guid.NewGuid(),
      BitFlag = 2,
    };

    Assert.True(parent.AddChild(child));
    Assert.False(parent.AddChild(child));
  }

  [Fact]
  public void RemoveChild_RemovesChildAndUpdatesCheckValue() {
    Node parent = new("Parent") {
      Id = Guid.NewGuid(),
      BitFlag = 1,
    };
    Node child = new("Child") {
      Id = Guid.NewGuid(),
      BitFlag = 2,
    };
    parent.AddChild(child);

    Node removed = parent.RemoveChild(child);

    Assert.Equal(child, removed);
    Assert.DoesNotContain(child, parent.Children);
    Assert.Equal((BigInteger)1, parent.CheckValue);
  }

  [Fact]
  public void RemoveChild_NonExistentChild_ReturnsNull() {
    Node parent = new("Parent") {
      Id = Guid.NewGuid(),
      BitFlag = 1,
    };
    Node child = new("Child") {
      Id = Guid.NewGuid(),
      BitFlag = 2,
    };

    Node result = parent.RemoveChild(child);

    Assert.Null(result);
  }

  [Fact]
  public void Contains_ReturnsTrueIfNodeIsContained() {
    Node parent = new("Parent") {
      Id = Guid.NewGuid(),
      BitFlag = 1,
    };
    Node child = new("Child") {
      Id = Guid.NewGuid(),
      BitFlag = 2,
    };
    parent.AddChild(child);

    Assert.True(parent.Contains(child));
  }

  [Fact]
  public void Contains_ReturnsFalseIfNodeIsNotContained() {
    Node parent = new("Parent") {
      Id = Guid.NewGuid(),
      BitFlag = 1,
    };
    Node child = new("Child") {
      Id = Guid.NewGuid(),
      BitFlag = 2,
    };

    Assert.False(parent.Contains(child));
  }

  [Fact]
  public void ToString_ReturnsNodeName() {
    Node node = new("MyNode") {
      Id = Guid.NewGuid(),
      BitFlag = 123,
    };

    Assert.Equal("MyNode", node.ToString());
  }
}