using System;
using System.IO;
using System.Reflection;

namespace Xwellbehaved.Infrastructure
{
    // TODO: TBD: can and probably should comment in Xml comments...
    internal static class AssemblyExtensions
    {
        public static string GetLocalCodeBase(this Assembly assembly)
        {
            if (!assembly.CodeBase.StartsWith("file:///", StringComparison.OrdinalIgnoreCase))
            {
                var message = $"Code base {assembly.CodeBase} in wrong format; must start with 'file:///' (case-insensitive).";

                throw new ArgumentException(message, "assembly");
            }

            var codeBase = assembly.CodeBase.Substring(8);
            return Path.DirectorySeparatorChar == '/'
                ? "/" + codeBase
                : codeBase.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
