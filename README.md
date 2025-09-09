# Num_HierarchyTree

Nem__HierarchyTree is a C# library for representing and manipulating hierarchical tree structures using bitwise operations for efficient node management. It is designed for scenarios where unique node identification, fast containment checks, and compact representation are required.

## Features
- Hierarchical tree structure with support for parent and child nodes
- Unique node identification using GUIDs and bit flags
- Efficient node addition, removal, and lookup
- Flat dictionary for fast access to any node
- Support for cleaning orphaned and false parent nodes
- Serialization support via custom JSON converter
- .NET 8 and C# 12 compatible

## Usage
Create a `HierarchyTree` and add `Node` objects to build your tree. Nodes can be added as roots or as children of other nodes. The library ensures node name uniqueness and manages bit flags for each node.

## Example crating a tree with a parent and a single child node
```csharp
HierarchyTree tree = new();

Node parent = new("Parent");
tree.Add(parent);

Node child = new("Child") {
  ParentId = parent.Id
};
tree.Add(child);
```

## Example to check if a node is part of a parent node
```csharp
HierarchyTree tree = new();

Node parent1 = new("Parent1");
tree.Add(parent);

Node parent2 = new("Parent2");
tree.Add(parent2);

Node child1 = new("child1") {
  ParentId = parent1.Id
};
tree.Add(child1);

Node child2 = new("child2") {
  ParentId = parent2.Id
};
tree.Add(child2);

parent1.Contains(child1); //returns true
parent2.Contains(child2); //returns true
parent1.Contains(child2); //returns false
parent2.Contains(child1); //returns false
```

## Example serialization and deserialization
```csharp
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

// Adding out of order is intentional to show ability to handle mixed order of additions when creating a tree
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
```


## License
This project is licensed under the MIT License.
