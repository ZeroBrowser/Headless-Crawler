using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ZeroBrowser.Crawler.Api.HostedService;
using ZeroBrowser.Crawler.Common.Frontier;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;
using ZeroBrowser.Crawler.Frontier;
using ZeroBrowser.Crawler.Puppeteer;

namespace ZeroBrowser.Crawler.Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IWebHostEnvironment env)
        {
            _hostingEnvironment = env;

            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.            
            //services.AddDbContextPool<CrawlerContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDbContext<CrawlerContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient, ServiceLifetime.Transient);

            //services.AddHealthChecks().AddDbContextCheck<CrawlerContext>();

            //services.AddScoped<ICrawler, Core.Crawler>();
            services.AddSingleton<IHeadlessBrowserService, HeadlessBrowserService>();
            services.AddSingleton<IBackgroundUrlQueue, BackgroundUrlQueue>();
            services.AddSingleton<FrontierState>();

            //services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            //services.AddTransient<IRepository, SQLiteRepository>();

            services.AddSingleton<IManageHeadlessBrowser, ManageHeadlessBrowser>();
            services.AddSingleton<IUrlChannel, UrlChannel>();

            services.AddSingleton<IFrontier, Frontier.Frontier>();

            services.Configure<CrawlerOptions>(Configuration.GetSection(CrawlerOptions.Section));

            //services.AddHostedService<ParallelQueuedHostedService>();
            //services.AddHostedService<QueuedHostedService>();

            services.AddHostedService<FrontierUrlQueuedHostedService>();
            services.AddHostedService<ParallelCrawlerHostedService>();



            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ZeroBrowser.Crawler.Api", Version = "v1" });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZeroBrowser.Crawler.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {

                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();


                });
            });
        }
    }
}
