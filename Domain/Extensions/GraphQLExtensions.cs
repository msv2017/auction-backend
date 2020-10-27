using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Interfaces;
using GraphQL;
using GraphQL.Types;

namespace Domain.Extensions
{
    public static class GraphQLExtensions
    {
        public static void AddFieldToQuery<TFrom, TTo>(
            this IGraphQLQueryResolver resolver,
            IObjectGraphType query,
            string name,
            string methodName)
            where TFrom : ObjectGraphType<TTo>
        {
            (query as ObjectGraphType)
                .FieldAsync<ListGraphType<TFrom>>(
                name,
                arguments: resolver.GetQueryArguments(methodName),
                resolve: async context =>
                {
                    return await resolver.InvokeAsync<IEnumerable<TTo>>(context, methodName);
                });
        }

        public static QueryArguments GetQueryArguments(
            this IGraphQLQueryResolver resolver,
            string methodName)
        {
            var type = resolver.GetType();
            var method = type.GetMethod(methodName);
            var arguments = new QueryArguments();

            foreach (var p in method.GetParameters())
            {
                arguments.Add(new QueryArgument(GetGraphQLType(p.ParameterType))
                {
                    Name = p.Name
                });
            }

            return arguments;
        }

        public static Task<T> InvokeAsync<T>(
            this IGraphQLQueryResolver resolver,
            IResolveFieldContext<object> context,
            string methodName)
        {
            var type = resolver.GetType();
            var method = type.GetMethod(methodName);
            var values = new List<object>();

            foreach (var p in method.GetParameters())
            {
                values.Add(context.GetArgument(p.ParameterType, p.Name));
            }

            return (Task<T>)method.Invoke(resolver, values.ToArray());
        }

        public static Type GetGraphQLType(Type type)
        {
            switch (type.Name)
            {
                case nameof(Byte):
                case nameof(Int16):
                case nameof(Int32):
                case nameof(Int64):
                case nameof(Enum): return typeof(IntGraphType);
                case nameof(Double): return typeof(FloatGraphType);
                case nameof(Single): return typeof(FloatGraphType);
                case nameof(Decimal): return typeof(DecimalGraphType);
                case nameof(Boolean): return typeof(BooleanGraphType);
                case nameof(DateTime): return typeof(DateTimeGraphType);
                default: return typeof(StringGraphType);
            }
        }
    }
}
