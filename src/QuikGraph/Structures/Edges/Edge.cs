using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph.Constants;

namespace QuikGraph
{
    /// <inheritdoc cref="Create{TVertex}(TVertex,TVertex)"/>
    public static class Edge
    {
        /// <summary> Returns an empty Edge-Set </summary>
        public static IEnumerable<TEdge> Empty<TEdge>() => Enumerable.Empty<TEdge>(); //null; //

        /// <summary> Creates a new <see cref="Edge{TVertex}"/> from <paramref name="source"/> to <paramref name="target"/> </summary>
        /// <remarks>
        /// A Factory Method allows to centrally change the Types of Edges to create.
        /// It can also be registered with an IoC Container to dynamically create new Edges. 
        /// </remarks>
        public static IEdge<TVertex> Create<TVertex>(TVertex source, TVertex target)
            => new Edge<TVertex>(source, target);

        /// <inheritdoc cref="Create{TVertex}(TVertex,TVertex)"/>
        public static IUndirectedEdge<TVertex> CreateUndirected<TVertex>(TVertex source, TVertex target)
            => new UndirectedEdge<TVertex>(source, target);

        /// <inheritdoc cref="Create{TVertex}(TVertex,TVertex)"/>
        public static IEdge<TVertex> Create<TVertex>(IEdge<TVertex> edge)// => edge.Clone();
            => new Edge<TVertex>(edge.Source, edge.Target);
    }

    /// <summary>
    /// The default <see cref="IEdge{TVertex}"/> implementation (directed edge).
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("{" + nameof(Source) + "}->{" + nameof(Target) + "}")]
    public class Edge<TVertex> : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new <see cref="Edge{TVertex}"/> class.
        /// </summary>
        protected internal Edge([NotNull] TVertex source, [NotNull] TVertex target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            Source = source;
            Target = target;
        }

        /// <inheritdoc />
        public TVertex Source { get; }

        /// <inheritdoc />
        public TVertex Target { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(EdgeFormats.String, Source, Target);
        }
    }
}