using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.OrderBot.Bots;
using VirtoCommerce.OrderBot.Bots.Adapters;
using VirtoCommerce.OrderBot.Bots.Dialogs;
using VirtoCommerce.OrderBot.Bots.Middlewares.Injector;
using VirtoCommerce.OrderBot.Infrastructure;

namespace VirtoCommerce.OrderBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            services.AddSingleton<IMiddlewareInjector, MiddlewareInjector>();
            services.AddMiddlewares();

            services.AddSingleton<IStorage, MemoryStorage>();
            services.AddSingleton<UserState>();
            services.AddSingleton<ConversationState>();
            services.AddTransient<IBot, DialogBot<AuthDialog>>();

            services.AddBotServices();

            services.Configure<PlatformEndpointOptions>(Configuration.GetSection("VirtoCommerce:Endpoint"));

            services.AddAutoRestClients();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();

            app.AddMiddlewares();
            app.AddInterceptors();
        }
    }
}
