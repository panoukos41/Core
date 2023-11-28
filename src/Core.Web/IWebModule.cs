using Microsoft.AspNetCore.Builder;

namespace Core;

public interface IWebModule<TSelf> where TSelf : class, IWebModule<TSelf>, new()
{
    abstract static void Add(WebApplicationBuilder builder, TSelf module);

    abstract static void Use(WebApplication app);
}
