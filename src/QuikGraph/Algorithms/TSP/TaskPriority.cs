using System;

namespace QuikGraph.Algorithms.TSP
{
    /// <summary> Compares <see cref="_cost"/> first and then negative <seealso cref="_pathSize"/> </summary>
    public sealed class TaskPriority : IComparable<TaskPriority>, IEquatable<TaskPriority>
    {
        private readonly double _cost;
        private readonly int _pathSize;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public TaskPriority(double cost, int pathSize)
        {
            _cost = cost;
            _pathSize = pathSize;
        }

        /// <inheritdoc />
        public int CompareTo(TaskPriority other)
        {
            if (other is null)
                return 1;

            int costCompare = _cost.CompareTo(other._cost);
            if (costCompare != 0)
                return costCompare;

            return -_pathSize.CompareTo(other._pathSize);
        }

        #region IComparable<T> Operators

        public static bool operator <(TaskPriority left, TaskPriority right) => left.CompareTo(right) < 0;

        public static bool operator <=(TaskPriority left, TaskPriority right) => left.CompareTo(right) <= 0;

        public static bool operator >(TaskPriority left, TaskPriority right) => left.CompareTo(right) > 0;

        public static bool operator >=(TaskPriority left, TaskPriority right) => left.CompareTo(right) >= 0;

        #endregion
        #region Equality

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as TaskPriority);
        
        public bool Equals(TaskPriority other)
            => other != null && _cost.Equals(other._cost) && _pathSize == other._pathSize;

        public static bool operator ==(TaskPriority priority1, TaskPriority priority2)
        {
            if (priority1 is null)
                return priority2 is null;
            if (priority2 is null)
                return false;
            return priority1.Equals(priority2);
        }

        public static bool operator !=(TaskPriority priority1, TaskPriority priority2) => !(priority1 == priority2);

        /// <inheritdoc />
        public override int GetHashCode() => (_cost.GetHashCode() * 397) ^ _pathSize;

        #endregion
    }
}