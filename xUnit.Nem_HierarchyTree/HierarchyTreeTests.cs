using Nem_HierarchyTree;
using NuGet.Frameworks;
using System.Numerics;
using System.Text.Json;

namespace xUnit.Nem_HierarchyTree;
public class HierarchyTreeTests {
  [Fact]
  public void Performance() {
    HierarchyTree<string> tree = new() {
      MaxNodes = 5_000
    };
    int num = 1;
    Node<string> node = new($"node{num}");

    while(tree.TryAdd(node)) {
      num++;
      node = new($"node{num}") {
        ParentId = node.Id
      };
    }

    Assert.Equal(tree.MaxNodes, tree.Count);
    Assert.Equal(BigInteger.Pow(2, tree.MaxNodes) - 1, tree.Roots[0].CheckValue);

    tree.Remove(tree.Roots[0]);

    Assert.Equal(0, tree.Count);
  }

  [Fact]
  public void BuildBottomUp() {
    HierarchyTree<string> tree = [];
    Node<string> topParent = new("Top Parent");
    Node<string> subParent1 = new("SubParent 1") {
      ParentId = topParent.Id
    };
    Node<string> subParent2 = new("SubParent 2") {
      ParentId = subParent1.Id
    };
    Node<string> subParent3 = new("SubParent 3") {
      ParentId = subParent2.Id
    };
    Node<string> terminalNode = new("Terminal Node") {
      ParentId = subParent3.Id
    };

    tree.Add(terminalNode);
    tree.Add(subParent3);
    tree.Add(subParent2);
    tree.Add(subParent1);
    tree.Add(topParent);

    terminalNode = tree[terminalNode.Id];
    subParent3 = tree[subParent3.Id];
    subParent2 = tree[subParent2.Id];
    subParent1 = tree[subParent1.Id];
    topParent = tree[topParent.Id];

    Assert.Equal(5, tree.Count);
    Assert.Single(tree.Roots);

    Assert.True(topParent.Contains(subParent1));
    Assert.True(topParent.Contains(subParent2));
    Assert.True(topParent.Contains(subParent3));
    Assert.True(topParent.Contains(terminalNode));

    Assert.True(subParent1.Contains(subParent2));
    Assert.True(subParent1.Contains(subParent3));
    Assert.True(subParent1.Contains(terminalNode));
    Assert.False(subParent1.Contains(topParent));

    Assert.True(subParent2.Contains(subParent3));
    Assert.True(subParent2.Contains(terminalNode));
    Assert.False(subParent2.Contains(topParent));
    Assert.False(subParent2.Contains(subParent1));

    Assert.False(subParent3.Contains(topParent));
    Assert.False(subParent3.Contains(subParent1));
    Assert.False(subParent3.Contains(subParent2));
    Assert.True(subParent3.Contains(terminalNode));

  }

  [Fact]
  public void BuildRandomOrder() {
    List<Node<string>> nodes = [];

    HierarchyTree<string> tree = [];
    Node<string> topParent = new("Top Parent");
    Node<string> subParent1 = new("SubParent 1") {
      ParentId = topParent.Id
    };
    Node<string> subParent2 = new("SubParent 2") {
      ParentId = subParent1.Id
    };
    Node<string> subParent3 = new("SubParent 3") {
      ParentId = subParent2.Id
    };
    Node<string> subParent4 = new("SubParent 4") {
      ParentId = subParent3.Id
    };
    Node<string> subParent5 = new("SubParent 5") {
      ParentId = subParent4.Id
    };
    Node<string> subParent6 = new("SubParent 6") {
      ParentId = subParent5.Id
    };
    Node<string> subParent7 = new("SubParent 7") {
      ParentId = subParent6.Id
    };
    Node<string> subParent8 = new("SubParent 8") {
      ParentId = subParent7.Id
    };
    Node<string> subParent9 = new("SubParent 9") {
      ParentId = subParent8.Id
    };
    Node<string> subParent10 = new("SubParent 10") {
      ParentId = subParent9.Id
    };
    Node<string> subParent11 = new("SubParent 11") {
      ParentId = subParent10.Id
    };
    Node<string> subParent12 = new("SubParent 12") {
      ParentId = subParent11.Id
    };
    Node<string> subParent13 = new("SubParent 13") {
      ParentId = subParent12.Id
    };
    Node<string> subParent14 = new("SubParent 14") {
      ParentId = subParent13.Id
    };
    Node<string> subParent15 = new("SubParent 15") {
      ParentId = subParent14.Id
    };
    Node<string> subParent16 = new("SubParent 16") {
      ParentId = subParent15.Id
    };
    Node<string> subParent17 = new("SubParent 17") {
      ParentId = subParent16.Id
    };
    Node<string> subParent18 = new("SubParent 18") {
      ParentId = subParent17.Id
    };
    Node<string> subParent19 = new("SubParent 19") {
      ParentId = subParent18.Id
    };
    Node<string> subParent20 = new("SubParent 20") {
      ParentId = subParent19.Id
    };
    Node<string> terminalNode = new("Terminal Node<string>") {
      ParentId = subParent3.Id
    };

    nodes.Add(topParent);
    nodes.Add(subParent1);
    nodes.Add(subParent2);
    nodes.Add(subParent3);
    nodes.Add(subParent4);
    nodes.Add(subParent5);
    nodes.Add(subParent6);
    nodes.Add(subParent7);
    nodes.Add(subParent8);
    nodes.Add(subParent9);
    nodes.Add(subParent10);
    nodes.Add(subParent11);
    nodes.Add(subParent12);
    nodes.Add(subParent13);
    nodes.Add(subParent14);
    nodes.Add(subParent15);
    nodes.Add(subParent16);
    nodes.Add(subParent17);
    nodes.Add(subParent18);
    nodes.Add(subParent19);
    nodes.Add(subParent20);
    nodes.Add(terminalNode);

    Random rnd = new();

    while (nodes.Count > 0) {
      int j = rnd.Next(nodes.Count);
      tree.Add(nodes[j]);
      nodes.RemoveAt(j);
    }

    topParent = tree[topParent.Id];
    subParent1 = tree[subParent1.Id];
    subParent2 = tree[subParent2.Id];
    subParent3 = tree[subParent3.Id];
    subParent4 = tree[subParent4.Id];
    subParent5 = tree[subParent5.Id];
    subParent6 = tree[subParent6.Id];
    subParent7 = tree[subParent7.Id];
    subParent8 = tree[subParent8.Id];
    subParent9 = tree[subParent9.Id];
    subParent10 = tree[subParent10.Id];
    subParent11 = tree[subParent11.Id];
    subParent12 = tree[subParent12.Id];
    subParent13 = tree[subParent13.Id];
    subParent14 = tree[subParent14.Id];
    subParent15 = tree[subParent15.Id];
    subParent16 = tree[subParent16.Id];
    subParent17 = tree[subParent17.Id];
    subParent18 = tree[subParent18.Id];
    subParent19 = tree[subParent19.Id];
    subParent20 = tree[subParent20.Id];
    terminalNode = tree[terminalNode.Id];

    Assert.Equal(22, tree.Count);
    Assert.Single(tree.Roots);
    
    Assert.True(topParent.Contains(subParent1));
    Assert.True(topParent.Contains(subParent2));
    Assert.True(topParent.Contains(subParent3));
    Assert.True(topParent.Contains(subParent4));
    Assert.True(topParent.Contains(subParent5));
    Assert.True(topParent.Contains(subParent6));
    Assert.True(topParent.Contains(subParent7));
    Assert.True(topParent.Contains(subParent8));
    Assert.True(topParent.Contains(subParent9));
    Assert.True(topParent.Contains(subParent10));
    Assert.True(topParent.Contains(subParent11));
    Assert.True(topParent.Contains(subParent12));
    Assert.True(topParent.Contains(subParent13));
    Assert.True(topParent.Contains(subParent14));
    Assert.True(topParent.Contains(subParent15));
    Assert.True(topParent.Contains(subParent16));
    Assert.True(topParent.Contains(subParent17));
    Assert.True(topParent.Contains(subParent18));
    Assert.True(topParent.Contains(subParent19));
    Assert.True(topParent.Contains(subParent20));
    Assert.True(topParent.Contains(terminalNode));

    Assert.True(subParent1.Contains(subParent2));
    Assert.True(subParent1.Contains(subParent3));
    Assert.True(subParent1.Contains(subParent4));
    Assert.True(subParent1.Contains(subParent5));
    Assert.True(subParent1.Contains(subParent6));
    Assert.True(subParent1.Contains(subParent7));
    Assert.True(subParent1.Contains(subParent8));
    Assert.True(subParent1.Contains(subParent9));
    Assert.True(subParent1.Contains(subParent10));
    Assert.True(subParent1.Contains(subParent11));
    Assert.True(subParent1.Contains(subParent12));
    Assert.True(subParent1.Contains(subParent13));
    Assert.True(subParent1.Contains(subParent14));
    Assert.True(subParent1.Contains(subParent15));
    Assert.True(subParent1.Contains(subParent16));
    Assert.True(subParent1.Contains(subParent17));
    Assert.True(subParent1.Contains(subParent18));
    Assert.True(subParent1.Contains(subParent19));
    Assert.True(subParent1.Contains(subParent20));
    Assert.True(subParent1.Contains(terminalNode));
    Assert.False(subParent1.Contains(topParent));
  }


  [Fact]
  public void CreateDuplicateNodeName() {
    HierarchyTree<string> tree = [];
    Node<string> parent = new("Parent");
    Node<string> child1 = new("Child") {
      ParentId = parent.Id
    };
    Node<string> child2 = new("Child") {
      ParentId = parent.Id
    };

    Assert.True(tree.TryAdd(parent));
    Assert.True(tree.TryAdd(child1));
    Assert.False(tree.TryAdd(child2));

    Assert.True(tree.FlatTree.ContainsKey(parent.Id));
    Assert.True(tree.FlatTree.ContainsKey(child1.Id));
    Assert.False(tree.FlatTree.ContainsKey(child2.Id));
  }

  [Fact]
  public void CreateDuplicateNodeId() {
    HierarchyTree<string> tree = [];
    Node<string> parent = new("Parent");

    Guid childId = Guid.NewGuid();

    Node<string> child1 = new("Child1") {
      Id = childId,
      ParentId = parent.Id
    };
    Node<string> child2 = new("Child2") {
      Id = childId,
      ParentId = parent.Id
    };

    Assert.True(tree.TryAdd(parent));
    Assert.True(tree.TryAdd(child1));
    Assert.False(tree.TryAdd(child2));

    Assert.True(tree.FlatTree.ContainsKey(parent.Id));
    Assert.True(tree.FlatTree.ContainsKey(child1.Id));
    Assert.NotEqual(child2.Contents, tree.FlatTree[child2.Id].Contents);
  }

  [Fact]
  public void SerializeDeserializeJson() {
    HierarchyTree<string> originalTree = [];

    Node<string> parent1 = new("Parent 1");
    Node<string> child11 = new("Child 1.1") {
      ParentId = parent1.Id
    };
    Node<string> child12 = new("Child 1.2") {
      ParentId = parent1.Id
    };
    Node<string> child121 = new("Child 1.2.1") {
      ParentId = child12.Id
    };

    // Adding out of order is intentional to test ability to link parents to existing children correctly
    originalTree.Add(child121);
    originalTree.Add(child11);
    originalTree.Add(child12);
    originalTree.Add(parent1);

    Node<string> parent2 = new("Parent 2");
    Node<string> child21 = new("Child 2.1") {
      ParentId = parent2.Id
    };
    Node<string> child22 = new("Child 2.2") {
      ParentId = parent2.Id
    };

    originalTree.Add(parent2);
    originalTree.Add(child21);
    originalTree.Add(child22);

    Node<string> parent3 = new("Parent 3");
    Node<string> child31 = new("Child 3.1") {
      ParentId = parent3.Id
    };
    Node<string> child32 = new("Child 3.2") {
      ParentId = parent3.Id
    };

    originalTree.Add(child31);
    originalTree.Add(parent3);
    originalTree.Add(child32);

    string json = HierarchyTree<string>.SerializeJson(originalTree);
    HierarchyTree<string> tree = HierarchyTree<string>.DeserializeJson(json);

    Assert.Equal(3, tree.Roots.Count);
    foreach (Node<string> expectedRoot in originalTree.Roots) {
      Assert.Contains(expectedRoot, tree.Roots);
    }

    Assert.Equal(10, tree.FlatTree.Count);
    foreach (Node<string> expectedNode in originalTree.FlatTree.Values) {
      Assert.Contains(expectedNode, tree.FlatTree.Values);
    }

  }

  [Fact]
  public void Add_NodeWithoutParent_AddsToRootsAndFlatTree() {
    HierarchyTree<string> tree = [];
    Node<string> node = new("Root");

    bool result = tree.TryAdd(node);

    Assert.True(result);
    Assert.Contains(node, tree.Roots);
    Assert.True(tree.FlatTree.ContainsKey(node.Id));
  }

  [Fact]
  public void Add_NodeWithParent_AddsAsChildAndToFlatTree() {
    HierarchyTree<string> tree = [];
    Node<string> parent = new("Parent");
    tree.Add(parent);

    Node<string> child = new("Child") {
      ParentId = parent.Id
    };
    bool result = tree.TryAdd(child);

    Assert.True(result);
    Assert.Contains(child, parent.Children);
    Assert.True(tree.FlatTree.ContainsKey(child.Id));
  }

  [Fact]
  public void Remove_NodeWithoutChildren_RemovesFromRootsAndFlatTree() {
    HierarchyTree<string> tree = [];
    Node<string> node = new("Root");
    tree.Add(node);

    bool result = tree.TryRemove(node);

    Assert.True(result);
    Assert.DoesNotContain(node, tree.Roots);
    Assert.False(tree.FlatTree.ContainsKey(node.Id));
  }

  [Fact]
  public void Remove_NodeWithChildren_RemovesRecursively() {
    HierarchyTree<string> tree = [];
    Node<string> parent = new("Parent");
    tree.Add(parent);

    Node<string> subParent1 = new("SubParent1") {
      ParentId = parent.Id,
    };
    tree.Add(subParent1);
    Node<string> subParent2 = new("SubParent2") {
      ParentId = parent.Id,
    };
    tree.Add(subParent2);

    Node<string> child1 = new("Child1") {
      ParentId = subParent1.Id,
    };
    tree.Add(child1);
    Node<string> child2 = new("Child2") {
      ParentId = subParent2.Id,
    };
    tree.Add(child2);

    bool result = tree.TryRemove(parent);

    Assert.True(result);
    Assert.False(tree.FlatTree.ContainsKey(parent.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent1.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent2.Id));
    Assert.False(tree.FlatTree.ContainsKey(child1.Id));
    Assert.False(tree.FlatTree.ContainsKey(child2.Id));
  }

  [Fact]
  public void Remove_ChildrenUp() {
    HierarchyTree<string> tree = [];
    Node<string> parent = new("Parent");
    tree.Add(parent);

    Node<string> subParent1 = new("SubParent1") {
      ParentId = parent.Id,
    };
    tree.Add(subParent1);
    Node<string> subParent2 = new("SubParent2") {
      ParentId = parent.Id,
    };
    tree.Add(subParent2);

    Node<string> child1 = new("Child1") {
      ParentId = subParent1.Id,
    };
    tree.Add(child1);
    Node<string> child2 = new("Child2") {
      ParentId = subParent2.Id,
    };
    tree.Add(child2);

    bool result = tree.TryRemove(child1);
    Assert.True(result);

    result = tree.TryRemove(child2);
    Assert.True(result);

    result = tree.TryRemove(subParent1);
    Assert.True(result);

    result = tree.TryRemove(subParent2);
    Assert.True(result);

    result = tree.TryRemove(parent);
    Assert.True(result);

    Assert.False(tree.FlatTree.ContainsKey(parent.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent1.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent2.Id));
    Assert.False(tree.FlatTree.ContainsKey(child1.Id));
    Assert.False(tree.FlatTree.ContainsKey(child2.Id));
  }

  [Fact]
  public void RemoveParentLeavesOtherParents() {
    // Arrange: Build a tree with two top-level parents, each with their own child
    HierarchyTree<string> tree = [];
    Node<string> parent1 = new("Parent1");
    Node<string> parent2 = new("Parent2");
    Node<string> child1 = new("Child1") { ParentId = parent1.Id };
    Node<string> child2 = new("Child2") { ParentId = parent2.Id };

    tree.Add(parent1);
    tree.Add(parent2);
    tree.Add(child1);
    tree.Add(child2);

    // Precondition: Both parents and children exist
    Assert.Equal(4, tree.Count);
    Assert.True(tree.Contains(parent1.Id));
    Assert.True(tree.Contains(parent2.Id));
    Assert.True(tree.Contains(child1.Id));
    Assert.True(tree.Contains(child2.Id));
    Assert.Contains(parent1, tree.Roots);
    Assert.Contains(parent2, tree.Roots);

    // Act: Remove parent1 (should also remove child1)
    bool removed = tree.TryRemove(parent1);

    // Assert: parent1 and child1 are gone, parent2 and child2 remain
    Assert.True(removed);
    Assert.False(tree.Contains(parent1.Id));
    Assert.False(tree.Contains(child1.Id));
    Assert.True(tree.Contains(parent2.Id));
    Assert.True(tree.Contains(child2.Id));
    Assert.DoesNotContain(parent1, tree.Roots);
    Assert.Contains(parent2, tree.Roots);
    Assert.Contains(child2, tree.FlatTree.Values);
    Assert.DoesNotContain(child1, tree.FlatTree.Values);
  }

  [Fact]
  public void Remove_ReaddWorks() {
    HierarchyTree<string> tree = [];
    Node<string> parent = new("Parent");
    tree.Add(parent);

    Node<string> subParent1 = new("SubParent1") {
      ParentId = parent.Id,
    };
    tree.Add(subParent1);
    Node<string> subParent2 = new("SubParent2") {
      ParentId = parent.Id,
    };
    tree.Add(subParent2);

    Node<string> child1 = new("Child1") {
      ParentId = subParent1.Id,
    };
    tree.Add(child1);
    Node<string> child2 = new("Child2") {
      ParentId = subParent2.Id,
    };
    tree.Add(child2);

    bool result = tree.TryRemove(parent);
    Assert.True(result);
    Assert.False(tree.FlatTree.ContainsKey(parent.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent1.Id));
    Assert.False(tree.FlatTree.ContainsKey(subParent2.Id));
    Assert.False(tree.FlatTree.ContainsKey(child1.Id));
    Assert.False(tree.FlatTree.ContainsKey(child2.Id));

    tree.Add(parent);
    tree.Add(subParent1);
    tree.Add(subParent2);
    tree.Add(child1);
    tree.Add(child2);

    Assert.True(tree.Contains(parent.Id));
    Assert.True(tree.Contains(subParent1.Id));
    Assert.True(tree.Contains(subParent2.Id));
    Assert.True(tree.Contains(child1.Id));
    Assert.True(tree.Contains(child2.Id));
  }

  [Fact]
  public void Add_NodeWithNonexistentParent_CreatesFalseParent() {
    HierarchyTree<string> tree = [];
    Node<string> node = new("Child") {
      ParentId = Guid.NewGuid()
    };

    bool result = tree.TryAdd(node);

    Assert.True(result);
    Assert.True(tree.FlatTree.ContainsKey(node.ParentId));
    Assert.True(tree.FlatTree.ContainsKey(node.Id));
    Assert.Contains(node, tree.FlatTree[node.ParentId].Children);
  }

  [Fact]
  public void MaximumSize() {
    HierarchyTree<string> tree = [];

    for (int i = 0; i < tree.MaxNodes; i++) {
      tree.Add(new Node<string>($"Node {i}"));
    }

    Assert.False(tree.TryAdd(new Node<string>($"Node {tree.MaxNodes}")));
  }

  [Fact]
  public void UnableToAddFalseParent() {
    HierarchyTree<string> tree = [];

    for (int i = 0; i < tree.MaxNodes - 1; i++) {
      tree.Add(new Node<string>($"Node {i}"));
    }

    Node<string> child = new("Child") {
      ParentId = Guid.NewGuid()
    };

    Assert.False(tree.TryAdd(child));
  }

  [Fact]
  public void CleanTree_RemovesFalseParentsAndOrphans() {
    HierarchyTree<string> tree = [];

    Node<string> parent = new("Parent");
    Node<string> child1 = new("Child1") {
      ParentId = parent.Id
    };

    Node<string> orphan1 = new("Orphan1") {
      ParentId = Guid.NewGuid()
    };
    Node<string> orphan2 = new("Orphan2") {
      ParentId = Guid.NewGuid()
    };
    Node<string> orphan3 = new("Orphan3") {
      ParentId = Guid.NewGuid()
    };

    tree.Add(parent);
    tree.Add(child1);
    tree.Add(orphan1);
    tree.Add(orphan2);
    tree.Add(orphan3);

    // Precondition: all nodes present
    Assert.Equal(8, tree.Count);
    Assert.True(tree.Contains(parent.Id));
    Assert.True(tree.Contains(child1.Id));
    Assert.True(tree.Contains(orphan1.Id));
    Assert.True(tree.Contains(orphan2.Id));
    Assert.True(tree.Contains(orphan3.Id));
    Assert.True(tree.Contains(orphan1.ParentId));
    Assert.True(tree.Contains(orphan2.ParentId));
    Assert.True(tree.Contains(orphan3.ParentId));

    tree.CleanTree();

    // False parent and orphan should be removed
    Assert.Equal(2, tree.Count);
    Assert.True(tree.Contains(parent.Id));
    Assert.True(tree.Contains(child1.Id));
    Assert.False(tree.Contains(orphan1.Id));
    Assert.False(tree.Contains(orphan2.Id));
    Assert.False(tree.Contains(orphan3.Id));
    Assert.False(tree.Contains(orphan1.ParentId));
    Assert.False(tree.Contains(orphan2.ParentId));
    Assert.False(tree.Contains(orphan3.ParentId));
  }

  [Fact]
  public void CleanTree_DoesNothingIfTreeIsClean() {
    HierarchyTree<string> tree = [];
    Node<string> root = new("Root");
    Node<string> child = new("Child") {
      ParentId = root.Id 
    };
    tree.Add(root);
    tree.Add(child);
    int countBefore = tree.Count;
    tree.CleanTree();
    int countAfter = tree.Count;
    Assert.Equal(countBefore, countAfter);
    Assert.True(tree.FlatTree.ContainsKey(root.Id));
    Assert.True(tree.FlatTree.ContainsKey(child.Id));
  }

  [Fact]
  public void ClearTree() {
    HierarchyTree<string> tree = [];

    Node<string> root1 = new("Root1");
    Node<string> root2 = new("Root2");

    Node<string> parent11 = new("Parent1.1") {
      ParentId = root1.Id
    };
    Node<string> parent21 = new("Parent2.1") {
      ParentId = root2.Id
    };

    Node<string> child111 = new("Child1.1.1") {
      ParentId = parent11.Id
    };
    Node<string> child112 = new("Child1.1.2") {
      ParentId = parent11.Id
    };
    Node<string> child221 = new("Child2.2.1") {
      ParentId = parent21.Id
    };
    Node<string> child222 = new("Child2.2.2") {
      ParentId = parent21.Id
    };

    tree.Add(root1);
    tree.Add(root2);
    tree.Add(parent11);
    tree.Add(parent21);
    tree.Add(child111);
    tree.Add(child112);
    tree.Add(child221);
    tree.Add(child222);

    Assert.Equal(8, tree.Count);
    Assert.True(tree.Contains(root1.Contents));
    Assert.True(tree.Contains(root2));
    Assert.True(tree.Contains(parent11.Contents));
    Assert.True(tree.Contains(parent21));
    Assert.True(tree.Contains(child111.Contents));
    Assert.True(tree.Contains(child112));
    Assert.True(tree.Contains(child221.Contents));
    Assert.True(tree.Contains(child222));

    tree.Clear();

    Assert.Equal(0, tree.Count);
    Assert.False(tree.Contains(root1));
    Assert.False(tree.Contains(root2.Contents));
    Assert.False(tree.Contains(parent11));
    Assert.False(tree.Contains(parent21.Contents));
    Assert.False(tree.Contains(child111));
    Assert.False(tree.Contains(child112.Contents));
    Assert.False(tree.Contains(child221));
    Assert.False(tree.Contains(child222.Contents));
  }

  [Fact]
  public void GetNode() {
    HierarchyTree<string> tree = [];

    Node<string> root1 = new("Root1");
    Node<string> root2 = new("Root2");

    Node<string> parent11 = new("Parent1.1") {
      ParentId = root1.Id
    };
    Node<string> parent21 = new("Parent2.1") {
      ParentId = root2.Id
    };

    Node<string> child111 = new("Child1.1.1") {
      ParentId = parent11.Id
    };
    Node<string> child112 = new("Child1.1.2") {
      ParentId = parent11.Id
    };
    Node<string> child221 = new("Child2.2.1") {
      ParentId = parent21.Id
    };
    Node<string> child222 = new("Child2.2.2") {
      ParentId = parent21.Id
    };

    tree.Add(root1);
    tree.Add(root2);
    tree.Add(parent11);
    tree.Add(parent21);
    tree.Add(child111);
    tree.Add(child112);
    tree.Add(child221);
    tree.Add(child222);

    Assert.Equal(root1, tree[root1.Id]);
    Assert.Equal(root2, tree[root2.Contents]);
    Assert.Equal(parent11, tree[parent11.Id]);
    Assert.Equal(parent21, tree[parent21.Contents]);
    Assert.Equal(child111, tree[child111.Id]);
    Assert.Equal(child112, tree[child112.Contents]);
    Assert.Equal(child221, tree[child221.Id]);
    Assert.Equal(child222, tree[child222.Contents]);
  }

  [Fact]
  public void AddChildWithoutParent() {
    HierarchyTree<string> tree = [];
    Node<string> child = new("Child") {
      ParentId = Guid.NewGuid()
    };
    bool result = tree.TryAdd(child);

    Assert.True(result);
    Assert.True(tree.FlatTree.ContainsKey(child.ParentId));
    Assert.True(tree.FlatTree.ContainsKey(child.Id));
    Assert.Contains(child, tree[child.ParentId].Children);

    Assert.True(tree[child.ParentId].IsFalseParent);
  }

  [Fact]
  public void UnableToAddDuplicateNames() {
    HierarchyTree<string> tree = [];
    Node<string> node1 = new("Node");
    Node<string> node2 = new("Node");
    bool result1 = tree.TryAdd(node1);
    bool result2 = tree.TryAdd(node2);

    Assert.True(result1);
    Assert.False(result2);
    Assert.True(tree.Contains(node1.Id));
    Assert.False(tree.Contains(node2.Id));
  }

  [Fact]
  public void UnableToAddDuplicateIds() {
    HierarchyTree<string> tree = [];
    Guid duplicateId = Guid.NewGuid();
    Node<string> node1 = new("Node1") {
      Id = duplicateId
    };
    Node<string> node2 = new("Node2") {
      Id = duplicateId
    };
    bool result1 = tree.TryAdd(node1);
    bool result2 = tree.TryAdd(node2);

    Assert.True(result1);
    Assert.False(result2);
    Assert.True(tree.FlatTree.ContainsKey(node1.Id));
    Assert.Equal("Node1", tree.FlatTree[duplicateId].Contents);
  }

  [Fact]
  public void UnableToAddWithNullName() {
    HierarchyTree<string> tree = [];
    Node<string> node = new(null);
    Assert.False(tree.TryAdd(node));
  }

  [Fact]
  public void UnableToAddWithEmptyId() {
    HierarchyTree<string> tree = [];
    Node<string> node = new("Node") {
      Id = Guid.Empty
    };
    Assert.False(tree.TryAdd(node));
  }

  [Fact]
  public void TestEnumerationGetsAllNodes() {
    HierarchyTree<string> tree = [];

    Node<string> parent1 = tree.Add(new Node<string>("Parent 1"));
    Node<string> parent2 = tree.Add(new Node<string>("Parent 2"));
    Node<string> parent3 = tree.Add(new Node<string>("Parent 3"));

    Node<string> subParent11 = tree.Add(new Node<string>("SubParent 1.1") {
      ParentId = parent1.Id
    });
    Node<string> subParent12 = tree.Add(new Node<string>("SubParent 1.2") {
      ParentId = parent1.Id
    });
    Node<string> subParent21 = tree.Add(new Node<string>("SubParent 2.1") {
      ParentId = parent2.Id
    });
    Node<string> subParent22 = tree.Add(new Node<string>("SubParent 2.2") {
      ParentId = parent2.Id
    });
    Node<string> subParent31 = tree.Add(new Node<string>("SubParent 3.1") {
      ParentId = parent3.Id
    });
    Node<string> subParent32 = tree.Add(new Node<string>("SubParent 3.2") {
      ParentId = parent3.Id
    });

    Node<string> child111 = tree.Add(new Node<string>("Child 1.1.1") {
      ParentId = subParent11.Id
    });
    Node<string> child112 = tree.Add(new Node<string>("Child 1.1.2") {
      ParentId = subParent11.Id
    });
    Node<string> child121 = tree.Add(new Node<string>("Child 1.2.1") {
      ParentId = subParent12.Id
    });
    Node<string> child122 = tree.Add(new Node<string>("Child 1.2.2") {
      ParentId = subParent12.Id
    });
    Node<string> child211 = tree.Add(new Node<string>("Child 2.1.1") {
      ParentId = subParent21.Id
    });
    Node<string> child212 = tree.Add(new Node<string>("Child 2.1.2") {
      ParentId = subParent21.Id
    });
    Node<string> child221 = tree.Add(new Node<string>("Child 2.2.1") {
      ParentId = subParent22.Id
    });
    Node<string> child222 = tree.Add(new Node<string>("Child 2.2.2") {
      ParentId = subParent22.Id
    });
    Node<string> child311 = tree.Add(new Node<string>("Child 3.1.1") {
      ParentId = subParent31.Id
    });
    Node<string> child312 = tree.Add(new Node<string>("Child 3.1.2") {
      ParentId = subParent31.Id
    });
    Node<string> child321 = tree.Add(new Node<string>("Child 3.2.1") {
      ParentId = subParent32.Id
    });
    Node<string> child322 = tree.Add(new Node<string>("Child 3.2.2") {
      ParentId = subParent32.Id
    });

    List<Node<string>> traversed = [];
    foreach(Node<string> node in tree) {
      traversed.Add(node);
    }

    Assert.Equal(tree.Count, traversed.Count);
    Assert.Contains(parent1, traversed);
    Assert.Contains(parent2, traversed);
    Assert.Contains(parent3, traversed);
    Assert.Contains(subParent11, traversed);
    Assert.Contains(subParent12, traversed);
    Assert.Contains(subParent21, traversed);
    Assert.Contains(subParent22, traversed);
    Assert.Contains(subParent31, traversed);
    Assert.Contains(subParent32, traversed);
    Assert.Contains(child111, traversed);
    Assert.Contains(child112, traversed);
    Assert.Contains(child121, traversed);
    Assert.Contains(child122, traversed);
    Assert.Contains(child211, traversed);
    Assert.Contains(child212, traversed);
    Assert.Contains(child221, traversed);
    Assert.Contains(child222, traversed);
    Assert.Contains(child311, traversed);
    Assert.Contains(child312, traversed);
    Assert.Contains(child321, traversed);
    Assert.Contains(child322, traversed);
  }

  [Fact]
  public void TestEnumerationOnEmptyTree() {
    HierarchyTree<string> tree = [];
    List<Node<string>> traversed = [];
    foreach (Node<string> node in tree) {
      traversed.Add(node);
    }
    Assert.Empty(traversed);
  }

  [Fact]
  public void Add_DuplicateNodeName_ThrowsInvalidOperationException() {
    HierarchyTree<string> tree = [];
    Node<string> node1 = new("Node");
    Node<string> node2 = new("Node");
    tree.Add(node1);
    InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => tree.Add(node2));
    Assert.Contains("Failed to add node to the tree.", ex.Message);
  }

  [Fact]
  public void Add_DuplicateNodeId_ThrowsInvalidOperationException() {
    HierarchyTree<string> tree = [];
    Guid id = Guid.NewGuid();
    Node<string> node1 = new("Node1") { Id = id };
    Node<string> node2 = new("Node2") { Id = id };
    tree.Add(node1);
    InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => tree.Add(node2));
    Assert.Contains("Failed to add node", ex.Message);
  }

  [Fact]
  public void Add_NodeWithNullContents_ThrowsInvalidOperationException() {
    HierarchyTree<string> tree = [];
    Node<string> node = new(null);
    InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => tree.Add(node));
    Assert.Contains("Failed to add node to the tree.", ex.Message);
  }

  [Fact]
  public void Add_NodeWithEmptyId_ThrowsInvalidOperationException() {
    HierarchyTree<string> tree = [];
    Node<string> node = new("Node") { Id = Guid.Empty };
    InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => tree.Add(node));
    Assert.Contains("Failed to add node to the tree.", ex.Message);
  }

  [Fact]
  public void Add_NodeWhenTreeIsFull_ThrowsInvalidOperationException() {
    HierarchyTree<string> tree = [];
    for (int i = 0; i < tree.MaxNodes; i++) {
      tree.Add(new Node<string>($"Node{i}"));
    }
    InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => tree.Add(new Node<string>("Overflow")));
    Assert.Contains("Failed to add node to the tree.", ex.Message);
  }

  [Fact]
  public void Dispose_ClearsAllDataAndSuppressesFinalize() {
    HierarchyTree<string> tree = [];
    Node<string> root = new("Root");
    Node<string> child = new("Child") { ParentId = root.Id };
    tree.Add(root);
    tree.Add(child);

    // Precondition: tree is populated
    Assert.Equal(2, tree.Count);
    Assert.True(tree.Contains(root.Id));
    Assert.True(tree.Contains(child.Id));
    Assert.True(tree.Roots.Count > 0);

    tree.Dispose();

    // After Dispose, all collections should be empty and state reset
    Assert.Equal(0, tree.Count);
    Assert.Empty(tree.Roots);
    Assert.Empty(tree.FlatTree);
    Assert.False(tree.Contains(root.Id));
    Assert.False(tree.Contains(child.Id));
  }
  // Missing test: ArgumentNullException for indexer
  [Fact]
  public void Indexer_ByContents_NullContents_ThrowsArgumentNullException() {
    HierarchyTree<string> tree = [];
    Assert.Throws<ArgumentNullException>(() => tree[null]);
  }

  // Missing test: GetNode with null argument
  [Fact]
  public void GetNode_ByContents_NullContents_ReturnsNull() {
    HierarchyTree<string> tree = [];
    Assert.Throws<ArgumentNullException>(() => tree.GetNode((string)null));
  }

  // Missing test: Contains with null node
  [Fact]
  public void Contains_NullNode_ThrowsArgumentNullException() {
    HierarchyTree<string> tree = [];
    Assert.Throws<ArgumentNullException>(() => tree.Contains((Node<string>)null));
  }

}