using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph.Collections;

namespace QuikGraph.Algorithms.TSP
{
    internal sealed class TasksManager<TVertex, TEdge>
        where TEdge : EquatableEdge<TVertex>
    {
        [NotNull]
        private readonly BinaryHeap<TaskPriority, Task<TVertex, TEdge>> _tasksQueue = new BinaryHeap<TaskPriority, Task<TVertex, TEdge>>();

        /// <summary> Adds the given <paramref name="task"/> into the <see cref="TasksManager{TVertex,TEdge}"/>. </summary>
        public void AddTask([NotNull] Task<TVertex, TEdge> task)
        {
            Debug.Assert(task != null);

            if (task.MinCost < double.PositiveInfinity)
            {
                _tasksQueue.Add(task.Priority, task);
            }
        }

        /// <summary> Gets and removes the task with minimal priority. </summary>
        [Pure]
        [NotNull]
        public Task<TVertex, TEdge> GetTask() => _tasksQueue.RemoveMinimum().Value;

        /// <summary> Checks if there are pending tasks. </summary>
        /// <returns>True if there are pending tasks, false otherwise.</returns>
        [Pure]
        public bool HasTasks() => _tasksQueue.Count > 0;
    }
}