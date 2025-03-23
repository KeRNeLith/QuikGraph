#if SUPPORTS_SERIALIZATION || SUPPORTS_CLONEABLE
using System;
#endif
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace QuikGraph.Petri
{
    /// <summary>
    /// High Level Petri Graph.
    /// </summary>
    /// <typeparam name="TToken">Token type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    public sealed class PetriNet<TToken> : IMutablePetriNet<TToken>
#if SUPPORTS_CLONEABLE
        , ICloneable
#endif
    {
        /// <summary>
        /// Initializes a new <see cref="PetriNet{Token}"/> class.
        /// </summary>
        public PetriNet()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        private PetriNet([NotNull] PetriNet<TToken> other)
        {
            Debug.Assert(other != null);

            _places.AddRange(other._places);
            _transitions.AddRange(other._transitions);
            _arcs.AddRange(other._arcs);
            _graph = new PetriGraph<TToken>();
            _graph.AddVertexRange(other._graph.Vertices);
            _graph.AddEdgeRange(other._graph.Edges);
        }

        #region IPetriNet<TToken>

        [NotNull, ItemNotNull]
        private readonly List<IPlace<TToken>> _places = new List<IPlace<TToken>>();

        /// <inheritdoc />
        public IEnumerable<IPlace<TToken>> Places => _places.AsEnumerable();

        [NotNull, ItemNotNull]
        private readonly List<ITransition<TToken>> _transitions = new List<ITransition<TToken>>();

        /// <inheritdoc />
        public IEnumerable<ITransition<TToken>> Transitions => _transitions.AsEnumerable();

        [NotNull, ItemNotNull]
        private readonly List<IArc<TToken>> _arcs = new List<IArc<TToken>>();

        /// <inheritdoc />
        public IEnumerable<IArc<TToken>> Arcs => _arcs.AsEnumerable();

        [NotNull]
        private readonly PetriGraph<TToken> _graph = new PetriGraph<TToken>();

        /// <inheritdoc />
        public IReadOnlyPetriGraph<TToken> Graph => _graph;

        #endregion

        #region IMutablePetriNet<TToken>

        /// <inheritdoc />
        public IPlace<TToken> AddPlace(string name)
        {
            IPlace<TToken> place = new Place<TToken>(name);
            _places.Add(place);
            _graph.AddVertex(place);
            return place;
        }

        /// <inheritdoc />
        public ITransition<TToken> AddTransition(string name)
        {
            ITransition<TToken> transition = new Transition<TToken>(name);
            _transitions.Add(transition);
            _graph.AddVertex(transition);
            return transition;
        }

        /// <inheritdoc />
        public IArc<TToken> AddArc(IPlace<TToken> place, ITransition<TToken> transition)
        {
            IArc<TToken> arc = new Arc<TToken>(place, transition);
            _arcs.Add(arc);
            _graph.AddEdge(arc);
            return arc;
        }

        /// <inheritdoc />
        public IArc<TToken> AddArc(ITransition<TToken> transition, IPlace<TToken> place)
        {
            IArc<TToken> arc = new Arc<TToken>(transition, place);
            _arcs.Add(arc);
            _graph.AddEdge(arc);
            return arc;
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Clones this <see cref="PetriNet{TToken}"/>.
        /// </summary>
        [Pure]
        [NotNull]
        public PetriNet<TToken> Clone()
        {
            return new PetriNet<TToken>(this);
        }

#if SUPPORTS_CLONEABLE
        /// <inheritdoc />
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("-----------------------------------------------");
            
            builder.AppendLine($"Places ({_places.Count})");
            foreach (IPlace<TToken> place in _places)
            {
                builder.AppendLine($"\t{place.ToStringWithMarking()}");
                builder.AppendLine();
            }

            builder.AppendLine($"Transitions ({_transitions.Count})");
            foreach (ITransition<TToken> transition in _transitions)
            {
                builder.AppendLine($"\t{transition}");
                builder.AppendLine();
            }

            builder.AppendLine("Arcs");
            foreach (IArc<TToken> arc in _arcs)
            {
                builder.AppendLine($"\t{arc}");
            }

            return builder.ToString();
        }
    }
}