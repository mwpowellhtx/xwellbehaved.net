using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Xwellbehaved.Execution.Extensions
{

#if DEBUG
    using Validation;
#endif

    using Xunit.Sdk;

    // TODO: TBD: can and probably should comment in Xml comments...
    internal static class MethodInfoExtensions
    {
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Propagating sync method parameter name.")]
        public static async Task InvokeAsync(this MethodInfo method, object obj, object[] arguments)
        {
            //Guard.AgainstNullArgument(nameof(method), method);

#if DEBUG
            method = method.RequiresNotNull(nameof(method));
#endif

            var parameterTypes = method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
            Reflector.ConvertArguments(arguments, parameterTypes);

            if (method.Invoke(obj, arguments) is Task task)
            {
                await task;
            }
        }
    }
}
