using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Pokedex.Application.Common.Enums;
using Pokedex.Application.Common.Interfaces;
using Pokedex.Application.ViewModels;
using Pokedex.Domain.Entities;
using Pokedex.Web;

namespace Pokedex.Application.IntegrationTests
{
    public class TestBase
    {
        private IConfigurationRoot _configuration;
        private static IServiceScopeFactory _scopeFactory;
        
        [OneTimeSetUp]
        public void Setup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            _configuration = builder.Build();
            
            var startUp = new Startup(_configuration);
            var services = new ServiceCollection();

            services.AddLogging(config => config.AddConsole());
            
            startUp.ConfigureServices(services);
            services.AddTransient<IHttpContextAccessor>(_ => SetupMockHttpContextAccessor());

            var pokemonService = services.FirstOrDefault(d => d.ServiceType == typeof(IPokemonService));
            services.Remove(pokemonService);
            services.RemoveAll(typeof(ITranslationService));

            services.AddScoped<IPokemonService>(_ => SetupMockPokmonService());
            services.AddScoped<ITranslationService>(_ => SetupMockYodaTranslationService());
            services.AddScoped<ITranslationService>(_ => SetupMockShakespeareTranslationService());
            
            TypeAdapterConfig<PokemonSpec, PokemonSpecVm>
                .NewConfig()
                .Map(dest => dest.isLegendary, src => src.is_legendary)
                .Map(dest => dest.habitat, src => $"{src.habitat.name}")
                .Map(dest => dest.description, src => $"{src.flavor_text_entries.FirstOrDefault().flavor_text}");
            
            _scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();   
        }
        
        private IHttpContextAccessor SetupMockHttpContextAccessor()
        {
            var fakeHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            fakeHttpContextAccessor.Setup(f => f.HttpContext).Returns(httpContext);
            return fakeHttpContextAccessor.Object;
        }

        private IPokemonService SetupMockPokmonService()
        {
            var fakePokemonService = new Mock<IPokemonService>();
            fakePokemonService.Setup(f => f.GetPokemonSpecAsync("mewtwo", CancellationToken.None))
                .ReturnsAsync(new PokemonSpec
                {
                    name = "mewtwo",
                    is_legendary = true,
                    habitat = new Habitat
                    {
                        name = "rare"
                    },
                    flavor_text_entries = new []{new Flavor_Text_Entry(){flavor_text = "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments."}}
                });
            fakePokemonService.Setup(f => f.GetPokemonSpecAsync("wormadam", CancellationToken.None))
                .ReturnsAsync(new PokemonSpec
                {
                    name = "wormadam",
                    is_legendary = false,
                    habitat = new Habitat
                    {
                        name = "rare"
                    },
                    flavor_text_entries = new []{new Flavor_Text_Entry(){flavor_text = "When the bulb on its back grows large, it appears to lose the ability to stand on its hind legs."}}
                });
            fakePokemonService.Setup(f => f.GetPokemonSpecAsync("aaa", CancellationToken.None))
                .ReturnsAsync((PokemonSpec)null);
            return fakePokemonService.Object;
        }

        private ITranslationService SetupMockYodaTranslationService()
        {
            var fakeYodaTranslationService = new Mock<ITranslationService>();
            fakeYodaTranslationService.SetupGet(f => f.TranslationOptions).Returns(TranslationOptions.Yoda);
            fakeYodaTranslationService.Setup(f => f.TranslateAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync("yoda_test");
                
            return fakeYodaTranslationService.Object;
        }
        
        private ITranslationService SetupMockShakespeareTranslationService()
        {
            var fakeYodaTranslationService = new Mock<ITranslationService>();
            fakeYodaTranslationService.SetupGet(f => f.TranslationOptions).Returns(TranslationOptions.Shakespeare);
            fakeYodaTranslationService.Setup(f => f.TranslateAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync("shakespeare_test");
                
            return fakeYodaTranslationService.Object;
        }
        
        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            using var scope = _scopeFactory.CreateScope();

            var mediator = scope.ServiceProvider.GetService<ISender>();

            return await mediator.Send(request);
        }

        public T GetInstance<T>()
        {
            using var scope = _scopeFactory.CreateScope();
            var result = scope.ServiceProvider.GetService<T>();
            return result;
        }
    }
}