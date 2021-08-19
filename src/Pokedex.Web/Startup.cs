using System;
using System.Linq;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Pokedex.Application;
using Pokedex.Application.Common.Interfaces;
using Pokedex.Application.ViewModels;
using Pokedex.Domain.Entities;
using Pokedex.Infrastructure.Services;
using Pokedex.Web.Filters;

namespace Pokedex.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddApplication();
            services.AddHttpContextAccessor();
            services.AddControllers(options =>
                {
                    options.Filters.Add(new ApiExceptionFilterAttribute());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                }).ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

            services.AddScoped<IPokemonService, PokemonService>();

            
            services.AddHttpClient(Constants.POKEMON_API_CLIENT_NAME, c =>
            {
                var baseUrl = Configuration.GetValue<string>("PokemonApiUrl");
                c.BaseAddress = new Uri(baseUrl);
            });
            
            services.AddHealthChecks();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pokedex.Web", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pokedex.Web v1"));
            }
            
            TypeAdapterConfig<PokemonSpec, PokemonSpecVm>
                .NewConfig()
                .Map(dest => dest.isLegendary, src => src.is_legendary)
                .Map(dest => dest.habitat, src => $"{src.habitat.name}")
                .Map(dest => dest.description, src => $"{src.flavor_text_entries.FirstOrDefault().flavor_text}");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseHealthChecks("/health");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
