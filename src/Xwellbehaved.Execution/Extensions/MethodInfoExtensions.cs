using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Xwellbehaved.Execution.Extensions
{
    using Validation;
    using Xunit.Sdk;

    // TODO: TBD: can and probably should comment in Xml comments...
    internal static class MethodInfoExtensions
    {
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Propagating sync method parameter name.")]
        public static async Task InvokeAsync(this MethodInfo method, object obj, object[] arguments)
        {
            method = method.RequiresNotNull(nameof(method));

            var parameterTypes = method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
            Reflector.ConvertArguments(arguments, parameterTypes);

            if (method.Invoke(obj, arguments) is Task task)
            {
                await task;
            }
        }
    }
}
