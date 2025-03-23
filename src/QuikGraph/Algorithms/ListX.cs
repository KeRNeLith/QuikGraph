using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;

/// <summary> Extension Methods for Lists </summary>
public static class ListX
{
    private static void Swap<TVertex>([NotNull, ItemNotNull] IList<TVertex> vertices, int indexA, int indexB)
    {
        TVertex tmp = vertices[indexA];
        vertices[indexA] = vertices[indexB];
        vertices[indexB] = tmp;
    }

    /// <summary> Recursively generates all Permutation of the <paramref name="vertices"/> </summary>
    public static List<List<TVertex>> GetAllPermutations<TVertex>([NotNull, ItemNotNull] this IList<TVertex> vertices)
        => GetPermutations_(vertices, 0, vertices.Count - 1).Select(p => p.ToList()).ToList();

    /// <summary> Recursively generates all Permutation of the <paramref name="vertices"/> </summary>
    public static IEnumerable<IList<TVertex>> GetPermutations<TVertex>([NotNull, ItemNotNull] this IList<TVertex> vertices)
        => GetPermutations_(vertices, 0, vertices.Count - 1);

    static IEnumerable<IList<TVertex>> GetPermutations_<TVertex>(
        [NotNull, ItemNotNull] IList<TVertex> vertices,
        int recursionDepth,
        int maxDepth)
    {
        for (int i = recursionDepth; i <= maxDepth; ++i)
        {
            Swap(vertices, recursionDepth, i);
            if (recursionDepth == maxDepth)
            {
                yield return vertices;
            }
            else
            {
                foreach(var permutation in GetPermutations_(vertices, recursionDepth + 1, maxDepth))
                {
                    yield return permutation;
                }

            }
            Swap(vertices, recursionDepth, i);
        }
    }


}