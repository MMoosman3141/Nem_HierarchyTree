# Nem_HierarchyTree

Nem_HierarchyTree is a C# library for representing and manipulating hierarchical tree structures using bitwise operations for efficient node management. It is designed for scenarios where unique node identification, fast containment checks, and compact representation are required.

## Features
- Hierarchical tree structure with support for parent and child nodes
- Unique node identification using GUIDs and bit flags
- Efficient node addition, removal, and lookup
- Flat dictionary for fast access to any node
- Support for cleaning orphaned and false parent nodes
- Serialization support via custom JSON converter
- .NET 8 and C# 12 compatible

## Requirements
- .NET 8.0 or later
- C# 12.0 language features

## Known Limitations
- Neither the tree nor nodes are thread-safe.
- Performance degrades with very large trees.  The default size limit is 2,000 nodes.
- Nodes must have unique contents (no duplicates allowed).

## Getting Started
Add the NuGet package or reference the project in your solution. All types are in the `Nem_HierarchyTree` namespace.

## Installation

Install via NuGet Package Manager:

Or via .NET CLI:
```bash
dotnet add package Nem_HierarchyTree --version 2.0.0
```

## Basic Usage

### Creating a Tree and Adding Nodes
```csharp
using Nem_HierarchyTree;

HierarchyTree<string> tree = new HierarchyTree<string>();

Node<string> parent = new Node<string>("Parent");
tree.Add(parent);

Node<string> child = new Node<string>("Child") { ParentId = parent.Id };
tree.Add(child);
```

### Adding Nodes with TryAdd
```csharp
Node<string> another = new Node<string>("Another");
Node<string> addedNode;
if (tree.TryAdd(another, out addedNode)) {
    // addedNode is the node added to the tree
}
```

### Removing Nodes
```csharp
// Remove a node and all its children
tree.Remove(parent); // returns a list of removed nodes

// Or use TryRemove
List<Node<string>> removedNodes;
if (tree.TryRemove(child, out removedNodes)) {
    // removedNodes contains the nodes that were removed
}
```

### Enumerating Nodes (Pre-order Traversal)
```csharp
foreach (Node<string> node in tree) {
    Console.WriteLine($"Node: {node.Contents}, Id: {node.Id}");
}
```

### Containment Checks
```csharp
// By node instance
bool exists = tree.Contains(parent);

// By node Id
Guid id = parent.Id;
bool existsById = tree.Contains(id);

// By node contents (string)
bool existsByName = tree.Contains("Parent");

// Check if a node is a descendant of another
bool isDescendant = parent.Contains(child); // true
```

### Accessing Nodes
```csharp
// By Id
Node<string> foundById = tree[parent.Id];

// By contents
Node<string> foundByContents = tree["Child"];
```

### Cleaning and Clearing the Tree
```csharp
// Remove false parents and orphaned nodes
tree.CleanTree();

// Remove all nodes and reset the tree
tree.Clear();
```

### Serialization and Deserialization
```csharp
using System.Text.Json;

// Serialize
tree.Add(new Node<string>("Root"));
string json = HierarchyTree<string>.SerializeJson(tree);

// Deserialize
HierarchyTree<string> deserializedTree = HierarchyTree<string>.DeserializeJson(json);
```

### Configuring Tree Size Limits
You can configure the maximum allowed size of the tree by setting the `MaxSize` property. The default value is 2000 nodes. To change the limit, set this property before adding nodes:

```csharp
tree.MaxSize = 5000; // Set max tree size to 5000 nodes
```

## Node<T> API Highlights
- `Id`: Unique identifier (Guid)
- `Contents`: The value stored in the node
- `ParentId`: Guid of the parent node (Guid.Empty for root)
- `Children`: Read-only list of child nodes
- `ChildCount`: Number of children
- `Contains(Node<T>)`: Checks if this node contains another node using bitwise operations

## HierarchyTree<T> API Highlights
- `Add(Node<T>)`: Add a node (throws on error)
- `TryAdd(Node<T>, out Node<T>)`: Try to add a node
- `Remove(Node<T>)`: Remove a node and its children
- `TryRemove(Node<T>, out List<Node<T>>)`
- `Contains(Node<T> | Guid | string)`: Check for node existence
- `GetNode(Guid | T)`: Retrieve node by Id or contents
- `Roots`: List of root nodes
- `CleanTree()`: Remove false parents and orphans
- `Clear()`: Remove all nodes
- `SerializeJson`, `DeserializeJson`: JSON (de)serialization

## License
Copyright (c) 2025, Mark Moosman

This project is licensed under the MIT License. See the [LICENSE.txt](LICENSE.txt) file for details.
