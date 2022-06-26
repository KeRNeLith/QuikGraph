using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace QuikGraph.Serialization.Tests
{
    #region Test classes

    public sealed class TestEdge : Edge<TestVertex>
    {
        public TestEdge(
            [NotNull] TestVertex source,
            [NotNull] TestVertex target,
            [NotNull] string id)
            : base(source, target)
        {
            ID = id;
        }

        public TestEdge(
            [NotNull] TestVertex source,
            [NotNull] TestVertex target,
            [NotNull] string id,
            string @string,
            int @int,
            long @long,
            float @float,
            double @double,
            bool @bool)
            : this(source, target, id)
        {
            String = @string;
            Int = @int;
            Long = @long;
            Float = @float;
            Double = @double;
            Bool = @bool;
        }

        [NotNull]
        public string ID { get; }

        [XmlAttribute("e_string")]
        [DefaultValue("defaultString")]
        public string String { get; set; }

        [XmlAttribute("e_int")]
        [DefaultValue(1)]
        public int Int { get; set; }

        [XmlAttribute("e_long")]
        [DefaultValue(2L)]
        public long Long { get; set; }

        [XmlAttribute("e_double")]
        public double Double { get; set; }

        [XmlAttribute("e_bool")]
        public bool Bool { get; set; }

        [XmlAttribute("e_float")]
        public float Float { get; set; }
    }

    public class EquatableTestEdge : EquatableEdge<EquatableTestVertex>, IEquatable<EquatableTestEdge>
    {
        public EquatableTestEdge(
            [NotNull] EquatableTestVertex source,
            [NotNull] EquatableTestVertex target,
            [NotNull] string id)
            : base(source, target)
        {
            ID = id;
        }

        [NotNull]
        public string ID { get; }

        [XmlAttribute("e_string")]
        [DefaultValue("defaultString")]
        public string String { get; set; }

        [XmlAttribute("e_int")]
        [DefaultValue(1)]
        public int Int { get; set; }

        [XmlAttribute("e_long")]
        [DefaultValue(2L)]
        public long Long { get; set; }

        [XmlAttribute("e_double")]
        public double Double { get; set; }

        [XmlAttribute("e_bool")]
        public bool Bool { get; set; }

        [XmlAttribute("e_float")]
        public float Float { get; set; }

        public virtual bool DataContentEquals([NotNull] EquatableTestEdge other)
        {
            return GetType() == other.GetType()
                   && string.Equals(ID, other.ID)
                   && string.Equals(String, other.String)
                   && Int == other.Int
                   && Long == other.Long
                   && Double.Equals(other.Double)
                   && Bool == other.Bool
                   && Float.Equals(other.Float);
        }

        public bool Equals(EquatableTestEdge other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return base.Equals(other) && DataContentEquals(other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EquatableTestEdge);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = ID.GetHashCode();
                hashCode = (hashCode * 397) ^ (String != null ? String.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Int;
                hashCode = (hashCode * 397) ^ Long.GetHashCode();
                hashCode = (hashCode * 397) ^ Double.GetHashCode();
                hashCode = (hashCode * 397) ^ Bool.GetHashCode();
                hashCode = (hashCode * 397) ^ Float.GetHashCode();
                return hashCode;
            }
        }
    }

    public sealed class EquatableAdditionalDataTestEdge : EquatableTestEdge, IEquatable<EquatableAdditionalDataTestEdge>
    {
        public EquatableAdditionalDataTestEdge(
            [NotNull] EquatableTestVertex source,
            [NotNull] EquatableTestVertex target,
            [NotNull] string id,
            double data)
            : base(source, target, id)
        {
            Data = data;
        }

        public double Data { get; }

        public override bool DataContentEquals(EquatableTestEdge other)
        {
            return base.DataContentEquals(other) && Data.Equals(((EquatableAdditionalDataTestEdge)other).Data);
        }

        public bool Equals(EquatableAdditionalDataTestEdge other)
        {
            return base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EquatableAdditionalDataTestEdge);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = ID.GetHashCode();
                hashCode = (hashCode * 397) ^ (Data.GetHashCode());
                return hashCode;
            }
        }
    }

    public sealed class TestEdgeNoSetter : Edge<TestVertex>
    {
        public TestEdgeNoSetter(
            [NotNull] TestVertex source,
            [NotNull] TestVertex target,
            [NotNull] string id)
            : base(source, target)
        {
            ID = id;
        }

        [NotNull]
        public string ID { get; }

        [XmlAttribute("e_string")]
        public string String { get; }
    }

    #endregion
}