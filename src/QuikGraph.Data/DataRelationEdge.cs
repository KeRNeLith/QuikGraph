using System;
using System.Data;
using JetBrains.Annotations;

namespace QuikGraph.Data
{
    /// <summary>
    /// Represents a relation between <see cref="T:System.Data.DataTable"/>s.
    /// </summary>
    public sealed class DataRelationEdge : IEdge<DataTable>
    {
        /// <summary>
        /// Initializes a new <see cref="DataRelationEdge"/> class.
        /// </summary>
        /// <param name="relation">Data relation.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="relation"/> is <see langword="null"/>.</exception>
        public DataRelationEdge([NotNull] DataRelation relation)
        {
            Relation = relation ?? throw new ArgumentNullException(nameof(relation));
        }

        /// <summary>
        /// Data relation hold by this edge.
        /// </summary>
        public DataRelation Relation { get; }

        /// <inheritdoc />
        public DataTable Source => Relation.ParentTable;

        /// <inheritdoc />
        public DataTable Target => Relation.ChildTable;
    }
}