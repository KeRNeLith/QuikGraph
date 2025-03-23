#if SUPPORTS_SERIALIZATION
using System;
#endif
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace QuikGraph.Graphviz.Dot
{
    /// <summary>
    /// Graphviz record cell collection.
    /// </summary>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class GraphvizRecordCellCollection : Collection<GraphvizRecordCell>
    {
        /// <summary>
        /// Initializes a new <see cref="GraphvizRecordCellCollection"/> class.
        /// </summary>
        public GraphvizRecordCellCollection()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GraphvizRecordCellCollection"/> class.
        /// </summary>
        /// <param name="collection">The collection that is wrapped by the new collection.</param>
        public GraphvizRecordCellCollection([NotNull, ItemNotNull] IList<GraphvizRecordCell> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GraphvizRecordCellCollection"/> class.
        /// </summary>
        /// <param name="collection">The collection that is wrapped by the new collection.</param>
        public GraphvizRecordCellCollection([NotNull, ItemNotNull] GraphvizRecordCellCollection collection)
            : base(collection)
        {
        }
    }
}