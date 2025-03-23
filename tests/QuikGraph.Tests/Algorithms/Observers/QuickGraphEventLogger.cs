namespace QuikGraph.Tests.Algorithms.Observers;
using System;

public class QuickGraphEventLogger<TVertex, TEdge>
    where TEdge : IEdge<TVertex>
{
    private readonly IVertexAndEdgeListGraph<TVertex, TEdge> _graph;

    public QuickGraphEventLogger(IVertexAndEdgeListGraph<TVertex, TEdge> graph)
    {
        _graph = graph ?? throw new ArgumentNullException(nameof(graph));

        Console.WriteLine("Initializing QuickGraphEventSubscriber...");
        LogGraphDetails();

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        Console.WriteLine("Subscribing to graph events...");

        if (_graph is IMutableVertexAndEdgeListGraph<TVertex, TEdge> mutableGraph)
        {
            mutableGraph.VertexAdded += OnVertexAdded;
            mutableGraph.VertexRemoved += OnVertexRemoved;
            mutableGraph.EdgeAdded += OnEdgeAdded;
            mutableGraph.EdgeRemoved += OnEdgeRemoved;

            Console.WriteLine("Subscribed to Vertex and Edge events in IMutableVertexAndEdgeListGraph.");
        }

        if (_graph is IMutableEdgeListGraph<TVertex, TEdge> edgeListGraph)
        {
            edgeListGraph.EdgeAdded += OnEdgeAdded;
            edgeListGraph.EdgeRemoved += OnEdgeRemoved;

            Console.WriteLine("Subscribed to Edge events in IMutableEdgeListGraph.");
        }
    }

    public void UnsubscribeFromEvents()
    {
        Console.WriteLine("Unsubscribing from graph events...");

        if (_graph is IMutableVertexAndEdgeListGraph<TVertex, TEdge> mutableGraph)
        {
            mutableGraph.VertexAdded -= OnVertexAdded;
            mutableGraph.VertexRemoved -= OnVertexRemoved;
            mutableGraph.EdgeAdded -= OnEdgeAdded;
            mutableGraph.EdgeRemoved -= OnEdgeRemoved;

            Console.WriteLine("Unsubscribed from Vertex and Edge events in IMutableVertexAndEdgeListGraph.");
        }

        if (_graph is IMutableEdgeListGraph<TVertex, TEdge> edgeListGraph)
        {
            edgeListGraph.EdgeAdded -= OnEdgeAdded;
            edgeListGraph.EdgeRemoved -= OnEdgeRemoved;

            Console.WriteLine("Unsubscribed from Edge events in IMutableEdgeListGraph.");
        }
    }

    private void LogGraphDetails()
    {
        Console.WriteLine("Graph Details:");
        Console.WriteLine($"Number of vertices: {_graph.VertexCount}");
        Console.WriteLine($"Number of edges: {_graph.EdgeCount}");

        Console.WriteLine("Vertices:");
        foreach (var vertex in _graph.Vertices)
        {
            Console.WriteLine($" - {vertex}");
        }

        Console.WriteLine("Edges:");
        foreach (var edge in _graph.Edges)
        {
            Console.WriteLine($" - {edge.Source} -> {edge.Target}");
        }
    }

    // Event Handlers with Logging
    private void OnVertexAdded(TVertex vertex)
    {
        Console.WriteLine($"[Event: VertexAdded] A new vertex was added: {vertex}");
        LogGraphDetails();
    }

    private void OnVertexRemoved(TVertex vertex)
    {
        Console.WriteLine($"[Event: VertexRemoved] A vertex was removed: {vertex}");
        LogGraphDetails();
    }

    private void OnEdgeAdded(TEdge edge)
    {
        Console.WriteLine($"[Event: EdgeAdded] A new edge was added: {edge.Source} -> {edge.Target}");
        Console.WriteLine($"Edge Details: Source={edge.Source}, Target={edge.Target}");
        LogGraphDetails();
    }

    private void OnEdgeRemoved(TEdge edge)
    {
        Console.WriteLine($"[Event: EdgeRemoved] An edge was removed: {edge.Source} -> {edge.Target}");
        Console.WriteLine($"Edge Details: Source={edge.Source}, Target={edge.Target}");
        LogGraphDetails();
    }
}
