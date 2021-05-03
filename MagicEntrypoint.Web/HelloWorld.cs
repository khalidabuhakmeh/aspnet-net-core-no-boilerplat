using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class HelloWorld : MagicEntrypoint.Web.IEndpoint
{
    public async Task ExecuteAsync(HttpContext context)
    {
        await context.Response.WriteAsync("Hello World!");
    }
}
