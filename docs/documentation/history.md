# Library history

## Overview

Below is some summarized information about the history of this library in order to have a better idea of where it comes from.

- [QuickGraph](#quickgraph)
- [YC.QuickGraph](#ycquickgraph)
- [QuikGraph](#quikgraph) <= Here we are

## QuickGraph

### General information

This is the original library supporting .NET Framework 4.0, Silverlight 4.0, Windows Phone 7, Windows 8 Metro Apps, XBox 360.

QuickGraph was originally created by Jonathan "Peli" de Halleux in 2003.

Initially the library was providing generic directed/undirected graph data structures and algorithms for .NET.
QuickGraph was coming with algorithms such as depth first seach, breath first search, A* search, shortest path, k-shortest path, maximum flow, minimum spanning tree, least common ancestors, etc...
QuickGraph was supporting [MSAGL](https://www.microsoft.com/en-us/research/project/microsoft-automatic-graph-layout), [GLEE](https://en.wikipedia.org/wiki/Microsoft_Automatic_Graph_Layout), and [Graphviz](https://www.graphviz.org) to render the graphs, serialization to [GraphML](http://graphml.graphdrawing.org), etc...

The library was also using [Code Contracts](https://www.microsoft.com/en-us/research/project/code-contracts/?from=http%3A%2F%2Fresearch.microsoft.com%2Fcontracts).

Note that the library was widely used such as in those projects:
- [Reflector.Graph Addin](https://archive.codeplex.com/?p=reflectoraddins)
- [Graph#, layout algorithms](https://archive.codeplex.com/?p=graphsharp)
- [Jolt.Net, a backing store for a generic finite state machine implementation](https://archive.codeplex.com/?p=jolt)
- [JSL StyleCop, Custom rules for Microsoft's StyleCop utility](https://archive.codeplex.com/?p=jslstylecop)
- [NDepend, codebase macro analysis](https://www.ndepend.com)
- [Dependency Viewer](https://archive.codeplex.com/?p=dependencyvisualizer)

### Versions

- QuickGraph 3.6. Portable Class Library support.
- QuickGraph 3.3.51106.0 available on nuget, no more support for .NET 2.0.
- QuickGraph 3.3 (updated) added Code Contracts reference assemblies
- QuickGraph 3.3 adds new graph data structures based on delegates
- QuickGraph 3.2 (bis) supporting Silveright
- QuickGraph 3.2 started using Code Contracts.
- QuickGraph 3.1 brings a Fibonacci Heap and support for 2.0 is back.
- QuickGraph 3.0 takes advantage of extension methods to simplify tasks.
- QuickGraph 2.0 introduced support for generic graph data structures
- The original QuickGraph for .net 1.0 was posted on CodeProject in 8 Dec 2003. It was time to do a refresh and make the graph generic.

The design of QuickGraph is inspired from the [Boost Graph Library](https://www.boost.org/doc/libs/1_68_0/libs/graph/doc/index.html).

## YC.QuickGraph

### General information

The YC.QuickGraph is a library that took sources of the original QuickGraph and put them on NuGet as package.

The goal was to continue the development (as described [there](https://github.com/YaccConstructor/QuickGraph/issues/173)).

It kept the majority of legacy code from QuickGraph and was applied some fixes.

### Versions

- YC.QuickGraph 3.7.5-deta Fixes, clean, no more DotParser.
- YC.QuickGraph 3.7.4 Some new algorithms, fixes and clean.
- YC.QuickGraph 3.7.3 Fixes.
- YC.QuickGraph 3.7.2 N/A.
- YC.QuickGraph 3.7.1 Some new algorithms, add DotParser and remove portable class library support.

## QuikGraph

### General information

QuikGraph library was built based on YC.QuickGraph (fork) that is itself based on the original QuickGraph.

The goal of this library was to clean and provide a more stable version of QuickGraph, which also follows new C# development standards.

As a consequence a massive clean and unit testing was added to the library.

Note that for now the cleaned library only concern the QuickGraph **core**, not its legacy adapters. This in order to better split features provided by NuGet packages.

This version of QuickGraph also get rid of:
- Code Contracts in favor of JetBrains contract annotations and standard asserts.
- PEX in favor of NUnit3 for unit testing.

### Versions

- See QuikGraph [repository tags](https://github.com/KeRNeLith/QuikGraph/releases) or NuGet [package page](https://www.nuget.org/packages/QuikGraph).