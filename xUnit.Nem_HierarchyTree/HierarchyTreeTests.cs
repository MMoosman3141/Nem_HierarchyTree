using Nem_HierarchyTree;
using Xunit;
using System;
using System.Text.Json;
using System.Text;

namespace xUnit.Nem_HierarchyTree; 
public class HierarchyTreeTests {
  [Fact]
  public void CreateDuplicateNodeName() {
    HierarchyTree tree = new();
    Node parent = new("Parent");
    Node child1 = new("Child") {
      ParentId = parent.Id
    };
    Node child2 = new("Child") {
      ParentId = parent.Id
    };

    Assert.True(tree.Add(parent));
    Assert.True(tree.Add(child1));
    Assert.False(tree.Add(child2));

    Assert.True(tree.FlatTree.ContainsKey(parent.Id));
    Assert.True(tree.FlatTree.ContainsKey(child1.Id));
    Assert.False(tree.FlatTree.ContainsKey(child2.Id));
  }

  [Fact]
  public void CreateDuplicateNodeId() {
    HierarchyTree tree = new();
    Node parent = new("Parent");

    Guid childId = Guid.NewGuid();

    Node child1 = new("Child1") {
      Id = childId,
      ParentId = parent.Id
    };
    Node child2 = new("Child2") {
      Id = childId,
      ParentId = parent.Id
    };

    Assert.True(tree.Add(parent));
    Assert.True(tree.Add(child1));
    Assert.False(tree.Add(child2));

    Assert.True(tree.FlatTree.ContainsKey(parent.Id));
    Assert.True(tree.FlatTree.ContainsKey(child1.Id));
    Assert.NotEqual(child2.Name, tree.FlatTree[child2.Id].Name);
  }

  [Fact]
  public void SerializeDeserializeJson() {
    HierarchyTree originalTree = new();

    originalTree.Add(new Node("Child 1.2.1") {
      Id = Guid.Parse("00000000-0000-0000-0000-000000000121"),
      ParentId = Guid.Parse("00000000-0000-0000-0000-000000000012")
    });
    originalTree.Add(new Node("Child 1.1") {
      Id = Guid.Parse("00000000-0000-0000-0000-000000000011"),
      ParentId = Guid.Parse("00000000-0000-0000-0000-000000000001")
    });
    originalTree.Add(new Node("Child 1.2") {
      Id = Guid.Parse("00000000-0000-0000-0000-000000000012"),
      ParentId = Guid.Parse("00000000-0000-0000-0000-000000000001")
    });
    originalTree.Add(new Node("Root 1") {
      Id = Guid.Parse("00000000-0000-0000-0000-000000000001")
    });

    originalTree.Add(new Node("Root 2") {
      Id = Guid.Parse("00000000-0000-0000-0000-000000000002")
    });
    originalTree.Add(new Node("Child 2.1") {
      Id = Guid.Parse("00000000-0000-0000-0000-000000000021"),
      ParentId = Guid.Parse("00000000-0000-0000-0000-000000000002")
    });
    originalTree.Add(new Node("Child 2.2") {
      Id = Guid.Parse("00000000-0000-0000-0000-000000000022"),
      ParentId = Guid.Parse("00000000-0000-0000-0000-000000000002") 
    });

    
    originalTree.Add(new Node("Child 3.1") {
      Id = Guid.Parse("00000000-0000-0000-0000-000000000031"),
      ParentId = Guid.Parse("00000000-0000-0000-0000-000000000003")
    });
    originalTree.Add(new Node("Root 3") {
      Id = Guid.Parse("00000000-0000-0000-0000-000000000003")
    });
    originalTree.Add(new Node("Child 3.2") {
      Id = Guid.Parse("00000000-0000-0000-0000-000000000032"),
      ParentId = Guid.Parse("00000000-0000-0000-0000-000000000003")
    });

    string json = JsonSerializer.Serialize(originalTree);
    
    HierarchyTree tree = JsonSerializer.Deserialize<HierarchyTree>(json);

    Assert.Equal(3, tree.Roots.Count);
    foreach(Node expectedRoot in originalTree.Roots) {
      Assert.Contains(expectedRoot, tree.Roots);
    }

    Assert.Equal(10, tree.FlatTree.Count);
    foreach (Node expectedNode in originalTree.FlatTree.Values) {
      Assert.Contains(expectedNode, tree.FlatTree.Values);
    }
 
  }


  [Fact]
  public void Add_NodeWithoutParent_AddsToRootsAndFlatTree() {
    HierarchyTree tree = new();
    Node node = new("Root") {
      Id = Guid.NewGuid(),
    };

    bool result = tree.Add(node);

    Assert.True(result);
    Assert.Contains(node, tree.Roots);
    Assert.True(tree.FlatTree.ContainsKey(node.Id));
  }

  [Fact]
  public void Add_NodeWithParent_AddsAsChildAndToFlatTree() {
    HierarchyTree tree = new();
    Node parent = new("Parent") {
      Id = Guid.NewGuid(),
      BitFlag = 1,
    };
    tree.Add(parent);

    Node child = new("Child") {
      Id = Guid.NewGuid(),
      ParentId = parent.Id,
      BitFlag = 2,
    };
    bool result = tree.Add(child);

    Assert.True(result);
    Assert.Contains(child, parent.Children);
    Assert.True(tree.FlatTree.ContainsKey(child.Id));
  }

  [Fact]
  public void Remove_NodeWithoutChildren_RemovesFromRootsAndFlatTree() {
    HierarchyTree tree = new();
    Node node = new("Root") {
      Id = Guid.NewGuid(),
      BitFlag = 1,
    };
    tree.Add(node);

    bool result = tree.Remove(node);

    Assert.True(result);
    Assert.DoesNotContain(node, tree.Roots);
    Assert.False(tree.FlatTree.ContainsKey(node.Id));
  }

  [Fact]
  public void Remove_NodeWithChildren_RemovesRecursively() {
    HierarchyTree tree = new();
    Node parent = new("Parent") {
      Id = Guid.NewGuid(),
    };
    tree.Add(parent);

    Node subParent1 = new("SubParent1") {
      Id = Guid.NewGuid(),
      ParentId = parent.Id,
    };
    tree.Add(subParent1);
    Node subParent2 = new("SubParent2") {
      Id = Guid.NewGuid(),
      ParentId = parent.Id,
    };
    tree.Add(subParent2);

    Node child1 = new("Child1") {
      Id = Guid.NewGuid(),
      ParentId = subParent1.Id,
    };
    tree.Add(child1);
    Node child2 = new("Child2") {
      Id = Guid.NewGuid(),
      ParentId = subParent2.Id,
    };
    tree.Add(child2);

    bool result = tree.Remove(parent);

    Assert.True(result);
    Assert.False(tree.FlatTree.ContainsKey(parent.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent1.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent2.Id));
    Assert.False(tree.FlatTree.ContainsKey(child1.Id));
    Assert.False(tree.FlatTree.ContainsKey(child2.Id));
  }

  [Fact]
  public void Remove_ChildrenUp() {
    HierarchyTree tree = new();
    Node parent = new("Parent") {
      Id = Guid.NewGuid(),
    };
    tree.Add(parent);

    Node subParent1 = new("SubParent1") {
      Id = Guid.NewGuid(),
      ParentId = parent.Id,
    };
    tree.Add(subParent1);
    Node subParent2 = new("SubParent2") {
      Id = Guid.NewGuid(),
      ParentId = parent.Id,
    };
    tree.Add(subParent2);

    Node child1 = new("Child1") {
      Id = Guid.NewGuid(),
      ParentId = subParent1.Id,
    };
    tree.Add(child1);
    Node child2 = new("Child2") {
      Id = Guid.NewGuid(),
      ParentId = subParent2.Id,
    };
    tree.Add(child2);

    bool result = tree.Remove(child1);
    Assert.True(result);

    result = tree.Remove(child2);
    Assert.True(result);

    result = tree.Remove(subParent1);
    Assert.True(result);

    result = tree.Remove(subParent2);
    Assert.True(result);

    result = tree.Remove(parent);
    Assert.True(result);

    Assert.False(tree.FlatTree.ContainsKey(parent.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent1.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent2.Id));
    Assert.False(tree.FlatTree.ContainsKey(child1.Id));
    Assert.False(tree.FlatTree.ContainsKey(child2.Id));
  }

  [Fact]
  public void Remove_ReaddWorks() {
    HierarchyTree tree = new();
    Node parent = new("Parent") {
      Id = Guid.NewGuid(),
    };
    tree.Add(parent);

    Node subParent1 = new("SubParent1") {
      Id = Guid.NewGuid(),
      ParentId = parent.Id,
    };
    tree.Add(subParent1);
    Node subParent2 = new("SubParent2") {
      Id = Guid.NewGuid(),
      ParentId = parent.Id,
    };
    tree.Add(subParent2);

    Node child1 = new("Child1") {
      Id = Guid.NewGuid(),
      ParentId = subParent1.Id,
    };
    tree.Add(child1);
    Node child2 = new("Child2") {
      Id = Guid.NewGuid(),
      ParentId = subParent2.Id,
    };
    tree.Add(child2);

    bool result = tree.Remove(parent);
    Assert.True(result);

    tree.Add(parent);
    tree.Add(subParent1);
    tree.Add(subParent2);
    tree.Add(child1);
    tree.Add(child2);

    result = tree.Remove(parent);

    Assert.True(result);
    Assert.False(tree.FlatTree.ContainsKey(parent.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent1.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent2.Id));
    Assert.False(tree.FlatTree.ContainsKey(child1.Id));
    Assert.False(tree.FlatTree.ContainsKey(child2.Id));
  }

  [Fact]
  public void Add_NodeWithNonexistentParent_CreatesFalseParent() {
    HierarchyTree tree = new();
    Guid parentId = Guid.NewGuid();
    Node node = new("Child") {
      Id = Guid.NewGuid(),
      ParentId = parentId,
      BitFlag = 1
    };

    bool result = tree.Add(node);

    Assert.True(result);
    Assert.True(tree.FlatTree.ContainsKey(parentId));
    Assert.True(tree.FlatTree.ContainsKey(node.Id));
    Assert.Contains(node, tree.FlatTree[parentId].Children);
  }

  [Fact]
  public void MaximumSize() {
    HierarchyTree tree = new();

    for(int i = 0; i < HierarchyTree.MAX_NODES; i++) {
      tree.Add(new Node($"Node {i}") {
        Id = Guid.NewGuid()
      });
    }

    Assert.False(tree.Add(new Node($"Node {HierarchyTree.MAX_NODES}") {
      Id = Guid.NewGuid()
    }));
  }

  [Fact]
  public void UnalbeToAddFalseParent() {
    HierarchyTree tree = new();

    for (int i = 0; i < HierarchyTree.MAX_NODES - 1; i++) {
      tree.Add(new Node($"Node {i}") {
        Id = Guid.NewGuid()
      });
    }

    Node child = new("Child") {
      Id = Guid.NewGuid(),
      ParentId = Guid.NewGuid()
    };

    Assert.False(tree.Add(child));
  }

}