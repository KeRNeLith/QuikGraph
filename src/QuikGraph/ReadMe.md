# QuikGraph

This Library defines Interfaces and Algorithms to solve Graph-Problems. 
Graphs are discrete Topologies and many real-world Problems 
can be abstracted down to this Level, which makes the Algorithms widely applicable. 

## Definitions: 

| Term          | Definition                                                                                                                                                                                                                                                                           |
| ------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Graph         | A __graph__ is a collection of Vertices and Edges. <br><br>In this Library it implements a generic Interface `IGraph<TVertex,TEdge>` <br>where `TEdge : IEdge<TVertex> { TVertex Source; TVertex Target; }`                                                                          |
| (dense)       | A __dense graph__  has up to \|Vertices\|²  Edges                                                                                                                                                                                                                                    |
| (sparse)      | A __sparse graph__  fewer Edges than \|Vertices\| * Log(\|Vertices\|)                                                                                                                                                                                                                |
| Vertex        | __Vertices__ (AKA __Nodes__) are simple objects <br>which have names (Identity) and sometimes other properties. <br><br>In this Library Vertices can be any Type with an Identity Relation <br>implemented as `IComparable<TVertex>`.                                                |
| Edge          | An __edge__ is a (directed, i.e. ordered) connection <br>from the __Source__ Vertex to the __Target__ Vertex. <br><br>It is modeled as an `IEdge<TVertex>`<br>                                                                                                                       |
| (reverse)     | The __reverse__ (AKA transpose) Edge runs in the opposite Direction <br>from the __Target__ Vertex to the __Source__ Vertex.<br>`ReversedBidirectionalGraph` wraps a Graph and exposes it by swapping `InEdges` and `OutEdges`.                                                      |
| (adjacent)    | Union of  `InEdges` and `OutEdges`.                                                                                                                                                                                                                                                  |
| (Properties)  | Edges can also have Properties, mostly a `Length` or `Cost`.<br>The Edges impose a discrete Topology on the Graph.                                                                                                                                                                   |
| Direction     | - __Directed__ Graphs (AKA `Digraph`) consist of only directed Edges. <br>- The __reverse__ Graph consists of the same Vertices but __reversed Edges__. <br>- __Undirected__ Graphs can be modeled as the Union of a Graph and its Reverse, <br>  i.e. all Edges can be be traversed |
| Path          | A Path is a contiguous sequence of Edges, <br>i.e. the __Target__ Vertex of the previous Edge is the __Source__ Vertex of the next Edge.                                                                                                                                             |
| (simple)      | A simple path is a path in which no vertex is repeated. <br>                                                                                                                                                                                                                         |
| Cycle         | A Cycle is a simple path, except the first and last Nodes are the same. <br>                                                                                                                                                                                                         |
| Component     | A connected Component of a Graph is a Sub-Graph <br>with a path from every node to every other node in the Component                                                                                                                                                                 |
| Tree          | A graph with no cycles is called a tree.<br>Prove by induction that a tree on V vertices has exactly V - 1 edges.                                                                                                                                                                    |
| Diamond       | A Diamond is                                                                                                                                                                                                                                                                         |
| Forest        | A group of disconnected trees is called a forest.                                                                                                                                                                                                                                    |
| Spanning Tree | A spanning tree of a Component is a sub-graph that <br>contains all the vertices but only enough of the edges to form a tree.                                                                                                                                                        |

## Examples 

### Geographical Navigation 

- The Vertices are geographical Places and 
- the Edge Length is usually the 'Distance' between these Places. 

### Electrical Wiring 

- The Vertices are electronic Components and 
- the Edge Properties can often be neglected, 
  unless for high Frequencies or Currencies 
  where Impedance, Capacity and Resistance become relevant. 

### Finite State-Machines 

Graphs can be used to visualize finite State-Machines with 
- the States as Vertices and 
- the Transitions as Edges. 

## Visualization 
 
One can draw a graph by marking points for the vertices and
drawing lines connecting them for the edges. 

The Placement of the Vertices is usually arbitrary, 
but it considerably reduces Confusion when Crossings of Edges are minimized. 

## Samples 

### Sample Vertices are named `a`,`b`,`c`,... 

### Sample Edges are just ordered Pairs of Vertices: `ab`, `ed`, ... 

### Paths are just Strings of Vertices `abfd`... 

### V = |Nodes| = Number of Vertices 

### E = |Edges| = Number of Edges  

## Data Structures 

There are two primary ways to efficiently represent Graphs in Programs. 
The choice between them determines Storage and Efficiency of Algorithms. 
For most efficient Representation the Vertices are 'numbered', 
which can be achieved by storing the Vertices in a List
and storing the Position in an `Index` Property of each Vertex. 

### Adjacency Matrix for dense Graphs 

Modelled by a 2D Array of `float` or `bool` Values 
representing the Length or (reciprocal) Cost of the Connection. 
The 2 Indices of the Array are determined from the `Index` Property of each Vertex 

This is very efficient both in Access in O(1) and Storage, but only for dense Graphs. 
For large Graphs the O(V²) Storage becomes a Problem. 

### Adjacency List for sparse Graphs 

This is the preferred solution for sparse Graphs, even large ones. 
It is modelled by a jagged, compressed 2D Array of Vertices or Vertex Indices. 
This requires O(E) Storage, but Access is typically O(V Log V). 
The first Index is determined from the `Index` Property of the `Source` Vertex. 
The second Index is usually determined by BiSection of a sorted List or by a Hash-Table. 


### Additional Node Information

Extra information associated with each vertex can be accommodated 
by using auxiliary arrays indexed by vertex number 
(or by making adj an array of records in the adjacency structure representation). 
Extra information associated with each edge can be put in the adjacency list nodes 
(or in an array a of records in the adjacency matrix representation), 
or in auxiliary arrays indexed by edge number (this requires numbering the edges). 


## Algorithms 


### Eulerian_Trail 


In graph theory, an Eulerian __trail__ (or **Eulerian path**) is a trail in a finite graph 
that visits __every Edge exactly once__ (allowing for revisiting vertices). 

Similarly, an Eulerian __circuit__ or Eulerian __cycle__ is an Eulerian trail 
that __starts and ends on the same vertex__. 

> They were first discussed by Leonhard Euler 
> while solving the famous [[#Königsberg Problem]] in 1736. 
> 
> The problem can be stated mathematically like this:
> Given the graph in the image, is it possible to construct a path 
> (or even a cycle; i.e., a path starting and ending on the same vertex) 
> that visits each edge exactly once?
>
> Euler proved that a __necessary condition__ for the existence of Eulerian circuits is 
> that __all vertices in the graph have an even degree__, 
> and stated without proof that connected graphs with all vertices of even degree 
> have an Eulerian circuit. 
> 
> The first complete proof of this latter claim was published 
> posthumously in 1873 by Carl Hierholzer. This is known as Euler's Theorem:
>
> # A connected graph has an __Euler cycle if and only if every vertex has even degree__.
>
> The term Eulerian graph has two common meanings in graph theory. 
> One meaning is a graph with an Eulerian circuit, and 
> the other is a graph with every vertex of even degree. 
> These definitions coincide for connected graphs.
>
> For the existence of Eulerian trails it is necessary that zero or two vertices have an odd degree; 
> this means the Königsberg graph is not Eulerian. 
> If there are no vertices of odd degree, all Eulerian trails are circuits. 
> If there are exactly two vertices of odd degree, 
> all Eulerian trails start at one of them and end at the other. 
> A graph that has an Eulerian trail but not an Eulerian circuit is called __semi-Eulerian__.
>
> [Wikipedia](https://en.wikipedia.org/wiki/Eulerian%20path)

#### Königsberg Problem: 

The Seven Bridges of Königsberg problem asks 
whether you can cross each of the Bridges B1..B7 exactly once: 
![[Konigsberg_bridges.png]]

Here is a more abstract, topological Representation:
```mermaid
graph LR

N--B1---K[Kneiphof]
K--B2---N[North]
N--B3---L[Lohmse]
L--B4---K
K--B5---S[South]
K--B6---S
S--B7---L

```

#### [Haus vom Nikolaus puzzle](https://de.wikipedia.org/wiki/Haus_vom_Nikolaus "de:Haus vom Nikolaus") 

```
  C      C
 / \    / \
B---D  B---D
|\ /|  |\ /|
| X |  | X |
|/ \|  |/ \|
A---E  A---E
        \ /
         F
```


This Puzzle has 2 odd Vertices (A and E at the Bottom), 
therefore you have to start at one of them (e.g. A like 'Anfang') and stop at the other (E like 'Ende'). 
If the House is made symmetric, you can create an Euler Cycle starting at any node. 

```mermaid
graph 

b-->c
f-->a
a-->b
c-->d
e-->c
d-->e
c-->f
```

