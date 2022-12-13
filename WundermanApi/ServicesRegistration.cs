using Infrastructure;
using Model;

namespace WundermanApi;

public static class ServicesRegistration
{
    public static IServiceCollection RegisterCustomServices(this IServiceCollection services)
    {
        services.AddSingleton<IFileProcessor, FileProcessor>();
        services.AddSingleton<IDataProcessorService, DataProcessorService>();
        return services;
    }
}