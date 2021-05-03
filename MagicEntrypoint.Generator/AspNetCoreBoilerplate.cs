using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MagicEntrypoint.Generator
{
    [Generator]
    public class AspNetCoreBoilerplate : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new EndpointsReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // generate program.cs
            // todo: figure out how to get namespace
            var @namespace = "MagicEntrypoint.Web"; 
            
            var program = $@"
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace {@namespace}
{{
    public class Program
    {{
        public static void Main(string[] args)
        {{
            CreateHostBuilder(args).Build().Run();
        }}

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {{ webBuilder.UseStartup<Startup>(); }});
    }}
}}";
            context.AddSource("aspnet.core.boilerplate.program", SourceText.From(program, Encoding.UTF8));

            // add endpoint
            var endpoint = $@"
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace {@namespace}
{{
    public interface IEndpoint
    {{
        Task ExecuteAsync(HttpContext context);
    }}
}}";
            
            context.AddSource("aspnet.core.boilerplate.endpoint", SourceText.From(endpoint, Encoding.UTF8));

            // generate startup.cs
            
            //todo: get endpoints
            var syntaxReceiver = (EndpointsReceiver)context.SyntaxReceiver;
            var endpoints = syntaxReceiver!
                .Endpoints
                .Select(c => $"services.AddScoped<{c.ClassName}>();\n")
                .Aggregate("", (a,i) => a + i);

            var maps = syntaxReceiver!
                .Endpoints
                .Select(c => @$"                    
                    endpoints.MapMethods(""{c.Path}"",
                        new [] {{ {c.AllMethods} }}, 
                        async context =>
                        {{
                            var endpoint = context.RequestServices.GetRequiredService<{c.ClassName}>();
                            await endpoint.ExecuteAsync(context);
                        }});
                ")
                .Aggregate("", (a,i) => a + i);
                ;

            var startup = $@"
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;

namespace MagicEntrypoint.Web
{{
        public class Startup
        {{
            // This method gets called by the runtime. Use this method to add services to the container.
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
            public void ConfigureServices(IServiceCollection services)
            {{
                // map endpoints into services                
                {endpoints}
            }}

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {{
                if (env.IsDevelopment())
                {{
                    app.UseDeveloperExceptionPage();
                }}

                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {{
                    /* entry point: {context.Compilation.GetEntryPoint(CancellationToken.None)?.Name} */
                    {maps}
                }});
            }}
        }}
}}
";
            context.AddSource("aspnet.core.boilerplate.startup", SourceText.From(startup, Encoding.UTF8));
        }
    }

    public class EndpointsReceiver : ISyntaxReceiver
    {
        public List<EndpointInfo> Endpoints { get; set; }
            = new List<EndpointInfo>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Business logic to decide what we're interested in goes here
            if (syntaxNode is ClassDeclarationSyntax cds &&
                cds.BaseList?.GetText().ToString().Contains("IEndpoint") == true)
            {
                // I cheated, I know, but this 
                // is where we'd get the information
                Endpoints.Add(new EndpointInfo  {
                    ClassName = cds.Identifier.ValueText,
                    Methods = new List<string> { "Get" },
                    Path = "/"
                });
            }
        }

        public struct EndpointInfo
        {
            public string ClassName { get; set; }
            public List<string> Methods { get; set; }
            public string Path { get; set; }

            public string AllMethods => string.Join(", ", Methods.Select(m => $@"HttpMethods.{m}"));
        }
    }
}