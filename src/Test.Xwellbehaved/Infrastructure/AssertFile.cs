using System;
using System.IO;
using System.Text;

namespace Xwellbehaved.Infrastructure
{
    // TODO: TBD: is a whole other file worth it for purposes of one test?
    /// <summary>
    /// Perofrms File assertions.
    /// </summary>
    public static class AssertFile
    {
        /// <summary>
        /// Verifies whether the contents of the file <paramref name="expectedPath"/> Contain
        /// the <paramref name="actual"/> text, as-is.
        /// </summary>
        /// <param name="expectedPath"></param>
        /// <param name="actual"></param>
        public static void Contains(string expectedPath, string actual)
        {
            var expected = File.ReadAllText(expectedPath);

            // If we are asking "Contains", then check for Contains, not "Exact Match".
            if (expected.IndexOf(actual) < 0)
            {
                var actualPath = Path.Combine(
                    Path.GetDirectoryName(expectedPath),
                    Path.GetFileNameWithoutExtension(expectedPath) + "-actual" + Path.GetExtension(expectedPath));

                File.WriteAllText(actualPath, actual, Encoding.UTF8);

                throw new Exception($"{actualPath} does not contain the contents of {expectedPath}.");
            }
        }
    }
}
