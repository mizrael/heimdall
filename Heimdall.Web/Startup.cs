using LibCore.Web.ErrorHandling;
using LibCore.Web.ErrorHandling.Builders;
using LibCore.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection;

namespace Heimdall.Web
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfigureOptions<MvcOptions>>(new LibCore.Web.Services.ConfigureMvcOptions(_container));
            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(_container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(_container));
            services.UseSimpleInjectorAspNetRequestScoping(_container);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseBrowserLink();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}

            InitContainer(app);

            app.UseStaticFiles().UseMvc();
        }

        private void InitContainer(IApplicationBuilder app)
        {
            _container.Options.DefaultScopedLifestyle = new SimpleInjector.Lifestyles.AsyncScopedLifestyle();
            
            _container.Register<Proxies.IServicesProxy>(() =>
            {
                var userServiceUrl = Configuration["Services:heimdall"];
                var apiClient = new LibCore.Web.HTTP.ApiClient(userServiceUrl);
                return new Proxies.ServicesProxy(apiClient);
            });

            _container.RegisterErrorFilter();

            _container.RegisterMvcControllers(app);
            _container.RegisterMvcViewComponents(app);

            //RegisterDb();

            //_container.RegisterMediator();
            //_container.RegisterMediatorHandlers(GetAssemblies());

            _container.Verify();
        }
    }
}
