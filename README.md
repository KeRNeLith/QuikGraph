|              |                                                                                                                                                                                                                                                                                                                                                                                                       |     |
| ------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --- |
| **Build**    | [![AppVeyor Build Status](https://ci.appveyor.com/api/projects/status/github/KeRNeLith/QuikGraph?branch=master&svg=true)](https://ci.appveyor.com/project/KeRNeLith/quikgraph)                                                                                                                                                                                                                        |     |
| **Coverage** | <sup>Coveralls</sup> [![Coverage Status](https://coveralls.io/repos/github/KeRNeLith/QuikGraph/badge.svg?branch=master)](https://coveralls.io/github/KeRNeLith/QuikGraph?branch=master) <sup>SonarQube</sup> [![SonarCloud Coverage](https://sonarcloud.io/api/project_badges/measure?project=quikgraph&metric=coverage)](https://sonarcloud.io/component_measures/metric/coverage/list?id=quikgraph) |     |
| **Quality**  | [![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=quikgraph&metric=alert_status)](https://sonarcloud.io/dashboard?id=quikgraph)                                                                                                                                                                                                                                               |     |
| **Nugets**   | [![Nuget Status](https://img.shields.io/nuget/v/quikgraph.svg)](https://www.nuget.org/packages/QuikGraph) QuikGraph                                                                                                                                                                                                                                                                                   |     |
|              | [![Nuget Status](https://img.shields.io/nuget/v/quikgraph.serialization.svg)](https://www.nuget.org/packages/QuikGraph.Serialization) QuikGraph.Serialization                                                                                                                                                                                                                                         |     |
|              | [![Nuget Status](https://img.shields.io/nuget/v/quikgraph.graphviz.svg)](https://www.nuget.org/packages/QuikGraph.Graphviz) QuikGraph.Graphviz                                                                                                                                                                                                                                                        |     |
|              | [![Nuget Status](https://img.shields.io/nuget/v/quikgraph.data.svg)](https://www.nuget.org/packages/QuikGraph.Data) QuikGraph.Data                                                                                                                                                                                                                                                                    |     |
|              | [![Nuget Status](https://img.shields.io/nuget/v/quikgraph.msagl.svg)](https://www.nuget.org/packages/QuikGraph.MSAGL) QuikGraph.MSAGL                                                                                                                                                                                                                                                                 |     |
|              | [![Nuget Status](https://img.shields.io/nuget/v/quikgraph.petri.svg)](https://www.nuget.org/packages/QuikGraph.Petri) QuikGraph.Petri                                                                                                                                                                                                                                                                 |     |
| **License**  | MS-PL                                                                                                                                                                                                                                                                                                                                                                                                 |     |

# QuikGraph

## What is **QuikGraph**?

QuikGraph provides generic directed/undirected graph data structures and algorithms for .NET.

QuikGraph comes with algorithms such as depth first search, breath first search, A* search, 
shortest path, k-shortest path, maximum flow, minimum spanning tree, etc.

*QuikGraph was originally created by Jonathan "Peli" de Halleux in 2003 and named QuickGraph.*

It was then updated to become YC.QuickGraph.

**This version** of QuickGraph, renamed **QuikGraph**, is a fork of YC.QuickGraph, 
and I tried to clean the library to provide it as a clean NuGet packages using modern C# development (.NET Core).

The library has been cleaned and improved. It comes with a lot of fixes for issues of the original library. 
It also has a wide support of .NET targets to make it compatible with the most development environments.

**[Getting started with QuikGraph](https://github.com/KeRNeLith/QuikGraph/wiki)**

Detailed Information can be found in the Library [[src/QuikGraph/ReadMe|ReadMe]] 
and the NDoc of the individual Algorithms.

---

## Targets

- [![.NET Standard](https://img.shields.io/badge/.NET%20Standard-%3E%3D%201.3-blue.svg)](#)
- [![.NET Core](https://img.shields.io/badge/.NET%20Core-%3E%3D%201.0-blue.svg)](#)
- [![.NET Framework](https://img.shields.io/badge/.NET%20Framework-%3E%3D%203.5-blue.svg)](#)
- Works under [Unity 3D](https://github.com/KeRNeLith/QuikGraph/wiki/Unity3D-Integration)

Supports Source Link (use dedicated symbol package)

To get it working you need to:
- Uncheck option "Enable Just My Code"
- Add the NuGet symbol server (*https://symbols.nuget.org/download/symbols*)
- Check option "Enable Source Link support"

---

## Contributing

### Build

* Clone this repository.
* Open QuikGraph.sln.

### Notes

The library get rid of PEX that was previously used for unit tests and now uses NUnit3 (not published).

I would be very pleased to receive pull requests to further **test**, **improve** or add new features to the library.

---

## Usage

### Packages

QuikGraph is available on [NuGet](https://www.nuget.org) in several modules.

- [QuikGraph](https://www.nuget.org/packages/QuikGraph) (Core)
- [QuikGraph.Serialization](https://www.nuget.org/packages/QuikGraph.Serialization)
- [QuikGraph.Graphviz](https://www.nuget.org/packages/QuikGraph.Graphviz)
- [QuikGraph.Data](https://www.nuget.org/packages/QuikGraph.Data)
- [QuikGraph.MSAGL](https://www.nuget.org/packages/QuikGraph.MSAGL)
- [QuikGraph.Petri](https://www.nuget.org/packages/QuikGraph.Petri)

### Where to go next?

* [Wiki](https://github.com/KeRNeLith/QuikGraph/wiki)
* [Documentation](https://kernelith.github.io/QuikGraph/)
* [External Information](https://quickgraph.codeplex.com/documentation) (The website was closed)

---

## Maintainer(s)

[![](https://github.com/KeRNeLith.png?size=50)](https://github.com/KeRNeLith)

## Contributor(s)

This project exists thanks to all the people who have contributed to the code base.

[<img src="https://github.com/jnyrup.png?size=50" width="50px;" alt=""/>](https://github.com/jnyrup)
[<img src="https://github.com/SimonTC.png?size=50" width="50px;" alt=""/>](https://github.com/SimonTC)
[<img src="https://github.com/tuwuhs.png?size=50" width="50px;" alt=""/>](https://github.com/tuwuhs)
[<img src="https://github.com/gropax.png?size=50" width="50px;" alt=""/>](https://github.com/gropax)
[<img src="https://github.com/Yannike.png?size=50" width="50px;" alt=""/>](https://github.com/Yannike)

---