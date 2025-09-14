using Nem_HierarchyTree;
using NuGet.Frameworks;
using System.Numerics;

namespace xUnit.Nem_HierarchyTree;
public class NodeTests {
  [Fact]
  public void NodeEquality() {
    Node nullNode1 = null;
    Node nullNode2 = null;
    Node node1 = new("node1") {
      ParentId = Guid.NewGuid()
    };
    Node node1a = new("node1") {
      ParentId = node1.ParentId,
      Id = node1.Id
    };
    Node node1b = node1;

    Node node2 = new("node2") {
      ParentId = Guid.NewGuid()
    };
    Node node2a = new("node2") {
      ParentId = node2.ParentId,
      Id = node2.Id
    };
    Node node2b = node2;

    Assert.Equal(nullNode1, nullNode2);
    Assert.True(nullNode1 == nullNode2);

    Assert.Equal(node1, node1b);
    Assert.Equal(node1, node1a);
    Assert.Equal(node1a, node1);

    Assert.Equal(node2, node2b);
    Assert.Equal(node2, node2a);
    Assert.Equal(node2a, node2);

    Assert.True(node1 == node1b);
    Assert.True(node1 == node1a);
    Assert.True(node1a == node1);

    Assert.True(node2 == node2b);
    Assert.True(node2 == node2a);
    Assert.True(node2a == node2);

    Assert.NotEqual(node1, node2);
    Assert.NotEqual(node2, node1);

    Assert.True(node1 != node2);
    Assert.True(node2 != node1);

    Assert.False(node1.Equals(nullNode1));
    Assert.NotEqual(node1, new Object());
    Assert.True(node1 != nullNode1);

  }

  [Fact]
  public void NodeEqualityLowLevelPropeties() {
    Node baseNode = new("node") {
      ParentId = Guid.NewGuid(),
      Id = Guid.NewGuid(),
      BitFlag = 16,
      CheckValue = 16 | 8
    };
    Node diffId = new(baseNode.Name) {
      ParentId = baseNode.ParentId,
      Id = Guid.NewGuid(),
      BitFlag = baseNode.BitFlag,
      CheckValue = baseNode.CheckValue
    };
    Node diffName = new("other") {
      ParentId = baseNode.ParentId,
      Id = baseNode.Id,
      BitFlag = baseNode.BitFlag,
      CheckValue = baseNode.CheckValue
    };
    Node diffParent = new(baseNode.Name) {
      ParentId = Guid.NewGuid(),
      Id = baseNode.Id,
      BitFlag = baseNode.BitFlag,
      CheckValue = baseNode.CheckValue
    };
    
    Assert.NotEqual(baseNode, diffId);
    Assert.NotEqual(baseNode, diffName);
    Assert.NotEqual(baseNode, diffParent);
  }

  [Fact]
  public void TestGetHashCode() {
    //Two different objects that are equal should produce the same hash code
    Node node1 = new("node1") {
      ParentId = Guid.NewGuid(),
      BitFlag = 1

    };
    Node node1b = new("node1") {
      ParentId = node1.ParentId,
      Id = node1.Id,
      BitFlag = node1.BitFlag
    };

    Assert.Equal(node1, node1b);
    Assert.Equal(node1.GetHashCode(), node1b.GetHashCode());
  }

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