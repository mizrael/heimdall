using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore;
using SimpleInjector.Integration.AspNetCore.Mvc;
using LibCore.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Reflection;

namespace Heimdall.API
{
    public class Startup
    {
        private Container _container = new Container();

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                 .AddMvcOptions(o => { o.Filters.Add(new LibCore.Web.Filters.ExceptionFilter()); });

            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(_container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(_container));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc().UseSimpleInjectorAspNetRequestScoping(_container);

            RegisterMapping();
        }

        private void InitContainer(IApplicationBuilder app)
        {
            _container.Options.DefaultScopedLifestyle = new AspNetRequestLifestyle();

            _container.RegisterMvcControllers(app);
            _container.RegisterMvcViewComponents(app);

            RegisterDb();

            _container.RegisterMediator();
            _container.RegisterMediatorHandlers(GetAssemblies());

            _container.Register<LibCore.Web.Services.IPinger, LibCore.Web.Services.Pinger>();

            _container.Verify();
        }

        private void RegisterDb()
        {
            _container.Register<LibCore.Mongo.IDbFactory, LibCore.Mongo.DbFactory>();
            _container.Register<LibCore.Mongo.IRepositoryFactory, LibCore.Mongo.RepositoryFactory>();

            var connStr = Configuration["ConnectionStrings:heimdall"];
            _container.Register<Mongo.Infrastructure.IDbContext>(() =>
            {
                var factory = _container.GetInstance<LibCore.Mongo.IRepositoryFactory>();

                return new Mongo.Infrastructure.DbContext(factory, connStr);
            });
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IMediator).GetTypeInfo().Assembly;
            yield return typeof(Mongo.Queries.Handlers.FindServiceHandler).GetTypeInfo().Assembly;
        }

        private void RegisterMapping()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Mongo.Infrastructure.Entities.ServiceEndpoint, Models.ServiceEndpoints>();

                Mongo.Infrastructure.MapperConfiguration.Register(cfg);
            });
        }
    }
}
