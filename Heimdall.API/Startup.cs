using LibCore.CQRS.Extensions;
using LibCore.Web.ErrorHandling.Builders;
using LibCore.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection;

namespace Heimdall.API
{
    public class Startup
    {
        private Container _container = null;

        public Startup(IHostingEnvironment env)
        {
            _container = new Container();

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
            services.AddSingleton<IConfigureOptions<MvcOptions>>(new LibCore.Web.Services.ConfigureMvcOptions(_container));
            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(_container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(_container));
            services.UseSimpleInjectorAspNetRequestScoping(_container);

            services.AddCors((opt) =>
            {
                opt.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            // after an update, this causes all the routes to give 404 error.
            //services.AddApiVersioning(o =>
            //{
            //    o.AssumeDefaultVersionWhenUnspecified = true;
            //    o.DefaultApiVersion = new ApiVersion(1, 0);
            //    o.ApiVersionReader = new HeaderApiVersionReader("api-version");
            //});

            services.AddMvcCore()
                .AddJsonFormatters()
                .AddApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                     new Swashbuckle.AspNetCore.Swagger.Info
                     {
                         Title = "Heimdall API - V1",
                         Version = "v1"
                     }
                  );

                var filePath = System.IO.Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Heimdall.API.xml");
                c.IncludeXmlComments(filePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            RegisterMapping();

            InitContainer(app);

            app.UseCors("CorsPolicy")
               .UseMvc()
               .UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint("/swagger/v1/swagger.json", "Heimdall API V1");
               });
        }

        private void InitContainer(IApplicationBuilder app)
        {
            var assemblies = GetAssemblies();

            _container.Options.DefaultScopedLifestyle = new SimpleInjector.Lifestyles.AsyncScopedLifestyle();

            _container.RegisterMvcControllers(app);
            //_container.RegisterMvcViewComponents(app);

            _container.RegisterErrorFilter();

            RegisterDb();

            _container.RegisterMediator();
            _container.RegisterHandlers(assemblies);

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

            var analyticsConnStr = Configuration["ConnectionStrings:heimdall_analytics"];
            _container.Register<Analytics.Mongo.Infrastructure.IAnalyticsDbContext>(() =>
            {
                var factory = _container.GetInstance<LibCore.Mongo.IRepositoryFactory>();

                return new Analytics.Mongo.Infrastructure.AnalyticsDbContext(factory, analyticsConnStr);
            });
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IMediator).GetTypeInfo().Assembly;
            yield return typeof(ValidationApiErrorInfoBuilder).GetTypeInfo().Assembly;
            yield return typeof(Mongo.Queries.Handlers.FindServiceHandler).GetTypeInfo().Assembly;
            yield return typeof(Analytics.Mongo.Events.Handlers.ServiceRefreshedHandler).GetTypeInfo().Assembly;
        }

        private void RegisterMapping()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                Mongo.Infrastructure.MapperConfiguration.Register(cfg);
            });
        }
    }
}
