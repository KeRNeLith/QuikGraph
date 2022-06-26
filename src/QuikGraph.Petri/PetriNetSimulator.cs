using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace QuikGraph.Petri
{
    /// <summary>
    /// Petri Net simulator.
    /// </summary>
    /// <typeparam name="TToken">Token type.</typeparam>
    public sealed class PetriNetSimulator<TToken>
    {
        [NotNull]
        private readonly Dictionary<ITransition<TToken>, TransitionBuffer> _transitionBuffers =
            new Dictionary<ITransition<TToken>, TransitionBuffer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PetriNetSimulator{TToken}"/> class.
        /// </summary>
        /// <param name="net">Petri net to simulate.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="net"/> is <see langword="null"/>.</exception>
        public PetriNetSimulator([NotNull] IPetriNet<TToken> net)
        {
            Net = net ?? throw new ArgumentNullException(nameof(net));
        }

        /// <summary>
        /// Petri Net.
        /// </summary>
        [NotNull]
        public IPetriNet<TToken> Net { get; }

        /// <summary>
        /// Initializes simulator.
        /// </summary>
        public void Initialize()
        {
            _transitionBuffers.Clear();
            foreach (ITransition<TToken> transition in Net.Transitions)
            {
                _transitionBuffers.Add(transition, new TransitionBuffer());
            }
        }

        /// <summary>
        /// Simulates a step.
        /// </summary>
        public void SimulateStep()
        {
            // First step, iterate over arcs and gather tokens in transitions
            GatherTransitioningTokens();

            // Second step, see which transition was enabled
            ComputeEnabledTransitions();

            // Third step, iterate over the arcs
            TransferTokens();

            // Step four, clear buffers
            ClearTransitionBuffers();
        }

        private void GatherTransitioningTokens()
        {
            foreach (IArc<TToken> arc in Net.Arcs)
            {
                if (!arc.IsInputArc)
                    continue;

                IList<TToken> tokens = _transitionBuffers[arc.Transition].Tokens;
                // Get annotated tokens
                IList<TToken> annotatedTokens = arc.Annotation.Evaluate(arc.Place.Marking);
                // Add annotated tokens
                foreach (TToken annotatedToken in annotatedTokens)
                {
                    tokens.Add(annotatedToken);
                }
            }
        }

        private void ComputeEnabledTransitions()
        {
            foreach (ITransition<TToken> transition in Net.Transitions)
            {
                // Get buffered tokens
                IList<TToken> tokens = _transitionBuffers[transition].Tokens;
                // Check if enabled, store value
                _transitionBuffers[transition].Enabled = transition.Condition.IsEnabled(tokens);
            }
        }

        private void TransferTokens()
        {
            foreach (IArc<TToken> arc in Net.Arcs)
            {
                if (!_transitionBuffers[arc.Transition].Enabled)
                    continue;

                if (arc.IsInputArc)
                {
                    // Get annotated tokens
                    TToken[] annotatedTokens = arc.Annotation.Evaluate(arc.Place.Marking).ToArray();
                    // Remove annotated comments from source place
                    foreach (TToken annotatedToken in annotatedTokens)
                    {
                        arc.Place.Marking.Remove(annotatedToken);
                    }
                }
                else
                {
                    IList<TToken> tokens = _transitionBuffers[arc.Transition].Tokens;
                    // Get annotated tokens
                    IList<TToken> annotatedTokens = arc.Annotation.Evaluate(tokens);
                    // IList<Token> annotated comments to target place
                    foreach (TToken annotatedToken in annotatedTokens)
                    {
                        arc.Place.Marking.Add(annotatedToken);
                    }
                }
            }
        }

        private void ClearTransitionBuffers()
        {
            foreach (ITransition<TToken> transition in Net.Transitions)
            {
                _transitionBuffers[transition].Tokens.Clear();
                _transitionBuffers[transition].Enabled = false;
            }
        }

        private sealed class TransitionBuffer
        {
            [NotNull, ItemNotNull]
            public IList<TToken> Tokens { get; } = new List<TToken>();

            public bool Enabled { get; set; } = true;
        }
    }
}