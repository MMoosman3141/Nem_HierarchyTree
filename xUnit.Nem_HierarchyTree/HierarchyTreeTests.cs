using Nem_HierarchyTree;
using NuGet.Frameworks;
using System.Numerics;
using System.Text.Json;

namespace xUnit.Nem_HierarchyTree;
public class HierarchyTreeTests {
  [Fact]
  public void Performance() {
    HierarchyTree tree = new() {
      MaxNodes = 5_000
    };
    int num = 1;
    Node node = new($"node{num}");

    while(tree.Add(node)) {
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
    HierarchyTree tree = new();
    Node topParent = new("Top Parent");
    Node subParent1 = new("SubParent 1") {
      ParentId = topParent.Id
    };
    Node subParent2 = new("SubParent 2") {
      ParentId = subParent1.Id
    };
    Node subParent3 = new("SubParent 3") {
      ParentId = subParent2.Id
    };
    Node terminalNode = new("Terminal Node") {
      ParentId = subParent3.Id
    };

    tree.Add(terminalNode);
    tree.Add(subParent3);
    tree.Add(subParent2);
    tree.Add(subParent1);
    tree.Add(topParent);

    terminalNode = tree.GetNodeById(terminalNode.Id);
    subParent3 = tree.GetNodeById(subParent3.Id);
    subParent2 = tree.GetNodeById(subParent2.Id);
    subParent1 = tree.GetNodeById(subParent1.Id);
    topParent = tree.GetNodeById(topParent.Id);

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
    List<Node> nodes = [];

    HierarchyTree tree = new();
    Node topParent = new("Top Parent");
    Node subParent1 = new("SubParent 1") {
      ParentId = topParent.Id
    };
    Node subParent2 = new("SubParent 2") {
      ParentId = subParent1.Id
    };
    Node subParent3 = new("SubParent 3") {
      ParentId = subParent2.Id
    };
    Node subParent4 = new("SubParent 4") {
      ParentId = subParent3.Id
    };
    Node subParent5 = new("SubParent 5") {
      ParentId = subParent4.Id
    };
    Node subParent6 = new("SubParent 6") {
      ParentId = subParent5.Id
    };
    Node subParent7 = new("SubParent 7") {
      ParentId = subParent6.Id
    };
    Node subParent8 = new("SubParent 8") {
      ParentId = subParent7.Id
    };
    Node subParent9 = new("SubParent 9") {
      ParentId = subParent8.Id
    };
    Node subParent10 = new("SubParent 10") {
      ParentId = subParent9.Id
    };
    Node subParent11 = new("SubParent 11") {
      ParentId = subParent10.Id
    };
    Node subParent12 = new("SubParent 12") {
      ParentId = subParent11.Id
    };
    Node subParent13 = new("SubParent 13") {
      ParentId = subParent12.Id
    };
    Node subParent14 = new("SubParent 14") {
      ParentId = subParent13.Id
    };
    Node subParent15 = new("SubParent 15") {
      ParentId = subParent14.Id
    };
    Node subParent16 = new("SubParent 16") {
      ParentId = subParent15.Id
    };
    Node subParent17 = new("SubParent 17") {
      ParentId = subParent16.Id
    };
    Node subParent18 = new("SubParent 18") {
      ParentId = subParent17.Id
    };
    Node subParent19 = new("SubParent 19") {
      ParentId = subParent18.Id
    };
    Node subParent20 = new("SubParent 20") {
      ParentId = subParent19.Id
    };
    Node terminalNode = new("Terminal Node") {
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

    topParent = tree.GetNodeById(topParent.Id);
    subParent1 = tree.GetNodeById(subParent1.Id);
    subParent2 = tree.GetNodeById(subParent2.Id);
    subParent3 = tree.GetNodeById(subParent3.Id);
    subParent4 = tree.GetNodeById(subParent4.Id);
    subParent5 = tree.GetNodeById(subParent5.Id);
    subParent6 = tree.GetNodeById(subParent6.Id);
    subParent7 = tree.GetNodeById(subParent7.Id);
    subParent8 = tree.GetNodeById(subParent8.Id);
    subParent9 = tree.GetNodeById(subParent9.Id);
    subParent10 = tree.GetNodeById(subParent10.Id);
    subParent11 = tree.GetNodeById(subParent11.Id);
    subParent12 = tree.GetNodeById(subParent12.Id);
    subParent13 = tree.GetNodeById(subParent13.Id);
    subParent14 = tree.GetNodeById(subParent14.Id);
    subParent15 = tree.GetNodeById(subParent15.Id);
    subParent16 = tree.GetNodeById(subParent16.Id);
    subParent17 = tree.GetNodeById(subParent17.Id);
    subParent18 = tree.GetNodeById(subParent18.Id);
    subParent19 = tree.GetNodeById(subParent19.Id);
    subParent20 = tree.GetNodeById(subParent20.Id);
    terminalNode = tree.GetNodeById(terminalNode.Id);

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

    Node parent1 = new("Parent 1");
    Node child11 = new("Child 1.1") {
      ParentId = parent1.Id
    };
    Node child12 = new("Child 1.2") {
      ParentId = parent1.Id
    };
    Node child121 = new("Child 1.2.1") {
      ParentId = child12.Id
    };

    // Adding out of order is intentional to test ability to link parents to existing children correctly
    originalTree.Add(child121);
    originalTree.Add(child11);
    originalTree.Add(child12);
    originalTree.Add(parent1);

    Node parent2 = new("Parent 2");
    Node child21 = new("Child 2.1") {
      ParentId = parent2.Id
    };
    Node child22 = new("Child 2.2") {
      ParentId = parent2.Id
    };

    originalTree.Add(parent2);
    originalTree.Add(child21);
    originalTree.Add(child22);

    Node parent3 = new("Parent 3");
    Node child31 = new("Child 3.1") {
      ParentId = parent3.Id
    };
    Node child32 = new("Child 3.2") {
      ParentId = parent3.Id
    };

    originalTree.Add(child31);
    originalTree.Add(parent3);
    originalTree.Add(child32);

    string json = JsonSerializer.Serialize(originalTree);
    HierarchyTree tree = JsonSerializer.Deserialize<HierarchyTree>(json);

    Assert.Equal(3, tree.Roots.Count);
    foreach (Node expectedRoot in originalTree.Roots) {
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
    Node node = new("Root");

    bool result = tree.Add(node);

    Assert.True(result);
    Assert.Contains(node, tree.Roots);
    Assert.True(tree.FlatTree.ContainsKey(node.Id));
  }

  [Fact]
  public void Add_NodeWithParent_AddsAsChildAndToFlatTree() {
    HierarchyTree tree = new();
    Node parent = new("Parent");
    tree.Add(parent);

    Node child = new("Child") {
      ParentId = parent.Id
    };
    bool result = tree.Add(child);

    Assert.True(result);
    Assert.Contains(child, parent.Children);
    Assert.True(tree.FlatTree.ContainsKey(child.Id));
  }

  [Fact]
  public void Remove_NodeWithoutChildren_RemovesFromRootsAndFlatTree() {
    HierarchyTree tree = new();
    Node node = new("Root");
    tree.Add(node);

    bool result = tree.Remove(node);

    Assert.True(result);
    Assert.DoesNotContain(node, tree.Roots);
    Assert.False(tree.FlatTree.ContainsKey(node.Id));
  }

  [Fact]
  public void Remove_NodeWithChildren_RemovesRecursively() {
    HierarchyTree tree = new();
    Node parent = new("Parent");
    tree.Add(parent);

    Node subParent1 = new("SubParent1") {
      ParentId = parent.Id,
    };
    tree.Add(subParent1);
    Node subParent2 = new("SubParent2") {
      ParentId = parent.Id,
    };
    tree.Add(subParent2);

    Node child1 = new("Child1") {
      ParentId = subParent1.Id,
    };
    tree.Add(child1);
    Node child2 = new("Child2") {
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
    Node parent = new("Parent");
    tree.Add(parent);

    Node subParent1 = new("SubParent1") {
      ParentId = parent.Id,
    };
    tree.Add(subParent1);
    Node subParent2 = new("SubParent2") {
      ParentId = parent.Id,
    };
    tree.Add(subParent2);

    Node child1 = new("Child1") {
      ParentId = subParent1.Id,
    };
    tree.Add(child1);
    Node child2 = new("Child2") {
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
    Node parent = new("Parent");
    tree.Add(parent);

    Node subParent1 = new("SubParent1") {
      ParentId = parent.Id,
    };
    tree.Add(subParent1);
    Node subParent2 = new("SubParent2") {
      ParentId = parent.Id,
    };
    tree.Add(subParent2);

    Node child1 = new("Child1") {
      ParentId = subParent1.Id,
    };
    tree.Add(child1);
    Node child2 = new("Child2") {
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
    Node node = new("Child") {
      ParentId = Guid.NewGuid()
    };

    bool result = tree.Add(node);

    Assert.True(result);
    Assert.True(tree.FlatTree.ContainsKey(node.ParentId));
    Assert.True(tree.FlatTree.ContainsKey(node.Id));
    Assert.Contains(node, tree.FlatTree[node.ParentId].Children);
  }

  [Fact]
  public void MaximumSize() {
    HierarchyTree tree = new();

    for (int i = 0; i < tree.MaxNodes; i++) {
      tree.Add(new Node($"Node {i}"));
    }

    Assert.False(tree.Add(new Node($"Node {tree.MaxNodes}")));
  }

  [Fact]
  public void UnableToAddFalseParent() {
    HierarchyTree tree = new();

    for (int i = 0; i < tree.MaxNodes - 1; i++) {
      tree.Add(new Node($"Node {i}"));
    }

    Node child = new("Child") {
      ParentId = Guid.NewGuid()
    };

    Assert.False(tree.Add(child));
  }

  [Fact]
  public void CleanTree_RemovesFalseParentsAndOrphans() {
    HierarchyTree tree = new();

    Node parent = new("Parent");
    Node child1 = new("Child1") {
      ParentId = parent.Id
    };

    Node orphan1 = new("Orphan1") {
      ParentId = Guid.NewGuid()
    };
    Node orphan2 = new("Orphan2") {
      ParentId = Guid.NewGuid()
    };
    Node orphan3 = new("Orphan3") {
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
    HierarchyTree tree = new();
    Node root = new("Root");
    Node child = new("Child") {
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
    HierarchyTree tree = new();

    Node root1 = new("Root1");
    Node root2 = new("Root2");

    Node parent11 = new("Parent1.1") {
      ParentId = root1.Id
    };
    Node parent21 = new("Parent2.1") {
      ParentId = root2.Id
    };

    Node child111 = new("Child1.1.1") {
      ParentId = parent11.Id
    };
    Node child112 = new("Child1.1.2") {
      ParentId = parent11.Id
    };
    Node child221 = new("Child2.2.1") {
      ParentId = parent21.Id
    };
    Node child222 = new("Child2.2.2") {
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
    Assert.True(tree.Contains(root1.Name));
    Assert.True(tree.Contains(root2));
    Assert.True(tree.Contains(parent11.Name));
    Assert.True(tree.Contains(parent21));
    Assert.True(tree.Contains(child111.Name));
    Assert.True(tree.Contains(child112));
    Assert.True(tree.Contains(child221.Name));
    Assert.True(tree.Contains(child222));

    tree.Clear();

    Assert.Equal(0, tree.Count);
    Assert.False(tree.Contains(root1));
    Assert.False(tree.Contains(root2.Name));
    Assert.False(tree.Contains(parent11));
    Assert.False(tree.Contains(parent21.Name));
    Assert.False(tree.Contains(child111));
    Assert.False(tree.Contains(child112.Name));
    Assert.False(tree.Contains(child221));
    Assert.False(tree.Contains(child222.Name));
  }

  [Fact]
  public void GetNode() {
    HierarchyTree tree = new();

    Node root1 = new("Root1");
    Node root2 = new("Root2");

    Node parent11 = new("Parent1.1") {
      ParentId = root1.Id
    };
    Node parent21 = new("Parent2.1") {
      ParentId = root2.Id
    };

    Node child111 = new("Child1.1.1") {
      ParentId = parent11.Id
    };
    Node child112 = new("Child1.1.2") {
      ParentId = parent11.Id
    };
    Node child221 = new("Child2.2.1") {
      ParentId = parent21.Id
    };
    Node child222 = new("Child2.2.2") {
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

    Assert.Equal(root1, tree.GetNodeById(root1.Id));
    Assert.Equal(root2, tree.GetNodeByName(root2.Name));
    Assert.Equal(parent11, tree.GetNodeById(parent11.Id));
    Assert.Equal(parent21, tree.GetNodeByName(parent21.Name));
    Assert.Equal(child111, tree.GetNodeById(child111.Id));
    Assert.Equal(child112, tree.GetNodeByName(child112.Name));
    Assert.Equal(child221, tree.GetNodeById(child221.Id));
    Assert.Equal(child222, tree.GetNodeByName(child222.Name));
  }
}