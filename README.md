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

## Example
```csharp
HierarchyTree tree = new();

Node root = new("RootNode");
tree.Add(root);

Node child = new("ChildNode") {
  ParentId = root.Id
};
tree.Add(child);
```

## License
This project is licensed under the MIT License.
