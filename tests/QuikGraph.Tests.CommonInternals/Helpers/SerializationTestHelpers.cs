#if SUPPORTS_SERIALIZATION
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;

namespace QuikGraph.Tests
{
    /// <summary>
    /// Test helpers related to serialization.
    /// </summary>
    internal static class SerializationTestHelpers
    {
        [Pure]
        [NotNull]
        public static T SerializeAndDeserialize<T>([NotNull] T @object)
        {
            // Round-trip the exception: Serialize and de-serialize with a BinaryFormatter
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                // "Save" object state
                bf.Serialize(ms, @object);

                // Re-use the same stream for de-serialization
                ms.Seek(0, 0);

                // Replace the original exception with de-serialized one
                return (T)bf.Deserialize(ms);
            }
        }
    }
}
#endif