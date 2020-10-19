using System;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using AspectCore.DynamicProxy.Parameters;
using Domain.Interfaces;

namespace Domain.Aop
{
    public class CacheAttribute : AbstractInterceptorAttribute
    {
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var returnType = context.GetReturnParameter().Type;
            var hasCache = context.ImplementationMethod.GetCustomAttributes(typeof(CacheAttribute), false).Any();

            if (returnType == typeof(void)
                || returnType == typeof(Task)
                || returnType == typeof(ValueTask)
                || !hasCache)
            {
                await next(context);
                return;
            }

            if (context.IsAsync())
            {
                returnType = returnType.GenericTypeArguments.FirstOrDefault();
            }

            var cacheKey = this.CreateCacheKey(context);

            this.Log("trying to get", cacheKey);

            var cacheService = (ICacheService)context.ServiceProvider.GetService(typeof(ICacheService));

            var cachedResult = cacheService.Get(cacheKey);

            if(cachedResult == null)
            {
                this.Log("key not found", cacheKey);
                await next(context);
                var returnValue = context.IsAsync()
                    ? await context.UnwrapAsyncReturnValue()
                    : context.ReturnValue;

                this.Log("trying to set", cacheKey);
                if(!cacheService.Set(cacheKey, returnValue))
                {
                    this.Log("failed to set", cacheKey);
                }
            }
            else
            {
                this.Log("found key", cacheKey);
                if (context.IsAsync())
                {
                    if (context.ImplementationMethod.ReturnType == typeof(Task<>).MakeGenericType(returnType))
                    {
                        context.ReturnValue = typeof(Task)
                            .GetMethod(nameof(Task.FromResult))
                            .MakeGenericMethod(returnType)
                            .Invoke(null, new[] { cachedResult });
                    }
                    if (context.ImplementationMethod.ReturnType == typeof(ValueTask<>).MakeGenericType(returnType))
                    {
                        context.ReturnValue = Activator
                            .CreateInstance(typeof(ValueTask<>).MakeGenericType(returnType), cachedResult);
                    }
                }
                else
                {
                    context.ReturnValue = cachedResult;
                }
            }
        }

        private string CreateCacheKey(AspectContext context)
        {
            // could use assembly name plus some fixed name from attribute
            // but that only matters for production ready apps

            return context.ImplementationMethod.DeclaringType.FullName +
                "." +
                context.ImplementationMethod.Name +
                "_" +
                string.Join("_", context.Parameters.Select(x => x.ToString()));
        }

        private void Log(string text, params string[] args)
        {
            // naive logging...
            Console.WriteLine($"Cache: {text} {string.Join(", ", args)}");
        }
    }
}
