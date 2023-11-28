using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public interface IAppModule<TSelf> where TSelf : class, IAppModule<TSelf>, new()
{
    abstract static void Add(IServiceCollection services, IConfiguration configuration, TSelf module);
}
