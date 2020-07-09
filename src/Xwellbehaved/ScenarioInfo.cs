using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Xwellbehaved.Execution
{

#if DEBUG
    using Validation;
#endif

    using Xunit.Abstractions;
    using Xunit.Sdk;

    // TODO: TBD: we can and probably should comment these in Xml fashion...
    public class ScenarioInfo
    {
        private static readonly ITypeInfo objectType = Reflector.Wrap(typeof(object));

        public string ScenarioDisplayName { get; }

        public MethodInfo MethodToRun { get; }

        // #4 MWP 2020-07-09 05:46:25 PM / While we are here also clarify between dataRow and actualArgs
        public List<object> ConvertedActualArgs { get; }

        public ScenarioInfo(IMethodInfo testMethod, object[] actualArgs, string scenarioOutlineDisplayName)
        {
            //Guard.AgainstNullArgument(nameof(testMethod), testMethod);

#if DEBUG
            testMethod = testMethod.RequiresNotNull(nameof(testMethod));
#endif

            // Which, we "do", in DEBUG mode.
#pragma warning disable CA1062 // ...validate parameter 'name' is non-null before using it...
            var parameters = testMethod.GetParameters().ToList();
#pragma warning restore CA1062 // ...validate parameter 'name' is non-null before using it...

            var typeParams = testMethod.GetGenericArguments().ToList();

            ITypeInfo[] typeArgs;
            if (testMethod.IsGenericMethodDefinition)
            {
                typeArgs = typeParams
                    .Select(typeParameter => InferTypeArgument(typeParameter.Name, parameters, actualArgs))
                    .ToArray();

                this.MethodToRun = testMethod.MakeGenericMethod(typeArgs).ToRuntimeMethod();
            }
            else
            {
                typeArgs = Array.Empty<ITypeInfo>();
                this.MethodToRun = testMethod.ToRuntimeMethod();
            }

            var passedArgs = Reflector.ConvertArguments(actualArgs, this.MethodToRun.GetParameters().Select(p => p.ParameterType).ToArray());

            var generatedArguments = GetGeneratedArguments(typeParams, typeArgs, parameters, passedArgs.Length);

            var args = passedArgs
                .Select(value => new Argument(value))
                .Concat(generatedArguments)
                .ToList();

            this.ScenarioDisplayName = GetScenarioDisplayName(scenarioOutlineDisplayName, typeArgs, parameters, args);
            this.ConvertedActualArgs = args.Select(arg => arg.Value).ToList();
        }

        private static ITypeInfo InferTypeArgument(
            string typeParameterName
            , IReadOnlyList<IParameterInfo> parameters
            , IReadOnlyList<object> passedArguments)
        {
            var sawNullValue = false;
            ITypeInfo typeArgument = null;
            for (var index = 0; index < Math.Min(parameters.Count, passedArguments.Count); ++index)
            {
                var parameterType = parameters[index].ParameterType;
                if (parameterType.IsGenericParameter && parameterType.Name == typeParameterName)
                {
                    var passedArgument = passedArguments[index];
                    if (passedArgument == null)
                    {
                        sawNullValue = true;
                    }
                    else if (typeArgument == null)
                    {
                        typeArgument = Reflector.Wrap(passedArgument.GetType());
                    }
                    else if (typeArgument.Name != passedArgument.GetType().FullName)
                    {
                        return objectType;
                    }
                }
            }

            return typeArgument == null || (sawNullValue && typeArgument.IsValueType) ? objectType : typeArgument;
        }

        private static IEnumerable<Argument> GetGeneratedArguments(
            IReadOnlyList<ITypeInfo> typeParameters
            , IReadOnlyList<ITypeInfo> typeArguments
            , IReadOnlyList<IParameterInfo> parameters
            , int passedArgumentsCount)
        {
            for (var missingArgumentIndex = passedArgumentsCount;
                missingArgumentIndex < parameters.Count;
                ++missingArgumentIndex)
            {
                var parameterType = parameters[missingArgumentIndex].ParameterType;
                if (parameterType.IsGenericParameter)
                {
                    ITypeInfo concreteType = null;
                    for (var typeParameterIndex = 0; typeParameterIndex < typeParameters.Count; ++typeParameterIndex)
                    {
                        var typeParameter = typeParameters[typeParameterIndex];
                        if (typeParameter.Name == parameterType.Name)
                        {
                            concreteType = typeArguments[typeParameterIndex];
                            break;
                        }
                    }

                    parameterType = concreteType ??
                        throw new InvalidOperationException(
                            $"The type of parameter \"{parameters[missingArgumentIndex].Name}\" cannot be resolved.");
                }

                yield return new Argument(((IReflectionTypeInfo)parameterType).Type);
            }
        }

        private static string GetScenarioDisplayName(
            string scenarioOutlineDisplayName
            , IReadOnlyList<ITypeInfo> typeArguments
            , IReadOnlyList<IParameterInfo> parameters
            , IReadOnlyList<Argument> arguments)
        {
            var typeArgumentsString = typeArguments.Any()
                ? $"<{string.Join(", ", typeArguments.Select(typeArgument => TypeUtility.ConvertToSimpleTypeName(typeArgument)))}>"
                : string.Empty;

            var parameterAndArgumentTokens = new List<string>();
            int parameterIndex;
            for (parameterIndex = 0; parameterIndex < arguments.Count; parameterIndex++)
            {
                if (arguments[parameterIndex].IsGeneratedDefault)
                {
                    continue;
                }

                parameterAndArgumentTokens.Add(string.Concat(
                    parameterIndex >= parameters.Count ? "???" : parameters[parameterIndex].Name,
                    ": ",
                    arguments[parameterIndex].ToString()));
            }

            for (; parameterIndex < parameters.Count; parameterIndex++)
            {
                parameterAndArgumentTokens.Add(parameters[parameterIndex].Name + ": ???");
            }

            return $"{scenarioOutlineDisplayName}{typeArgumentsString}({string.Join(", ", parameterAndArgumentTokens)})";
        }

        private class Argument
        {
            private static readonly MethodInfo genericFactoryMethod = CreateGenericFactoryMethod();

            public Argument(Type type)
            {
                //Guard.AgainstNullArgument(nameof(type), type);

#if DEBUG
                type = type.RequiresNotNull(nameof(type));
#endif

                this.Value = genericFactoryMethod.MakeGenericMethod(type).Invoke(null, null);
                this.IsGeneratedDefault = true;
            }

            public Argument(object value) => this.Value = value;

            public object Value { get; }

            public bool IsGeneratedDefault { get; }

            public override string ToString() => ArgumentFormatter.Format(this.Value);

            private static MethodInfo CreateGenericFactoryMethod()
            {
                Expression<Func<object>> template = () => CreateDefault<object>();
                return ((MethodCallExpression)template.Body).Method.GetGenericMethodDefinition();
            }

            private static T CreateDefault<T>() => default;
        }
    }
}
