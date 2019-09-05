namespace Messenger.Server
{
    using System;

    using Messenger.DAL.Services;
    using Messenger.Server.Middlewares;
    using Messenger.Server.Services;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseMvc();

            app.UseWebSockets();

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.Map("/real-time-messages", a =>
            {
                var handler = serviceProvider.GetService<WebSocketService>();
                a.UseMiddleware<WebSocketMiddleware>(handler);
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Messenger API V1"));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddLogging(c => c.AddConsole());

            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Info
            {
                Version = "v1",
                Title = "Messenger API",
                Description = "ASP.NET Core Web API"
            }));

            services.AddScoped<IMessagesService, MessagesService>();
            services.AddSingleton<WebSocketService>();
        }
    }
}