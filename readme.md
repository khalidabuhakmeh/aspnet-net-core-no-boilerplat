# ASP.NET Core No Boilerplate

## The General Idea

Web developers think about web applications with regard to endpoints being the common unit of work. When starting with a new project, we might want to start with the endpoint and layer new functionality on later. We'll start with defining an endpoint, then move on to configuration and middleware.

We have three building blocks: `Endpoints`, `Services`, and `Middleware`.

**Note: This sample is a rough proof-of-concept, and requires more work to be made complete.**

## Adding An Endpoint

To add an endpoint, we need to implement a type that implements `IEndpoint`.

```c#
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class HelloWorld : MagicEntrypoint.Web.IEndpoint
{
    [Route("/"), HttpGet]
    public async Task ExecuteAsync(HttpContext context)
    {
        await context.Response.WriteAsync("Hello World!");
    }
}
```

We could also assume defaults based on conventions. `GET` and the name of class is the path.

```c#
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

// GET /helloworld
public class HelloWorld : MagicEntrypoint.Web.IEndpoint
{
    public async Task ExecuteAsync(HttpContext context)
    {
        await context.Response.WriteAsync("Hello World!");
    }
}
```

## Configuring Middleware

We define middleware like we normally would, but we would then define a class of `Configuration` in our project with a method of `Configure`.

```c#
public class Configuration {
    
    // can add dependencies in a constructor

     public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        // do more configuration
        // called after routing is added
        // and before routing        
     }
}
```

## Adding Services

By default, each endpoint is added to the `IServiceCollection` and opts into dependency injection. We can add more services by defining a `Services` class in our project with a `Configure` method.

```c#
public class Services {
    public void Configure(IServiceCollection services) {
        // add more services
    }
}
```

## Warning

This is a proof of concept, and the source generator code is nowhere near complete.

## Help

If you'd like to chat about this idea, and you have proficient skills with source generators, please contact me on Twitter (https://twitter.com/buhakmeh).

## License

The MIT License (MIT)
Copyright © 2021 Khalid Abuhakmeh

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

