using Microsoft.Extensions.DependencyInjection;

namespace UberQueue.AspNetCore.Configuration
{
    public static class ServiceCollectionPreConfigureExtensions
    {
        public static IServiceCollection PreConfigure<TOptions>(this IServiceCollection services, Action<TOptions> optionsAction)
        {
            services.GetPreConfigureActions<TOptions>().Add(optionsAction);
            return services;
        }

        public static TOptions ExecutePreConfiguredActions<TOptions>(this IServiceCollection services)
            where TOptions : new()
        {
            return services.ExecutePreConfiguredActions(new TOptions());
        }

        public static TOptions ExecutePreConfiguredActions<TOptions>(this IServiceCollection services, TOptions options)
        {
            services.GetPreConfigureActions<TOptions>().Configure(options);
            return options;
        }

        public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
        {
            return (T)services
                .FirstOrDefault(d => d.ServiceType == typeof(T))
                ?.ImplementationInstance;
        }

        public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services, Type type)
        {
            return (T)services
                .FirstOrDefault(d => d.ServiceType == type)
                ?.ImplementationInstance;
        }

        public static PreConfigureActionsHandler<TOptions> GetPreConfigureActions<TOptions>(this IServiceCollection services)
        {
            var actionList = services.GetSingletonInstanceOrNull<PreConfigureActionsHandler<TOptions>>();
            if (actionList == null)
            {
                actionList = new PreConfigureActionsHandler<TOptions>();
                services.AddSingleton(actionList);
            }

            return actionList;
        }
    }
}
