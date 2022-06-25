using DotneteLK.Models;
using Nest;

namespace DotneteLK.Extensions
{
    public static class ElasticSearchExtension
    {
        public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["ELKConfigurations:Uri"];
            var defaultIndex = configuration["ELKConfigurations:index"];

            var settings = new ConnectionSettings(new Uri(url))
                                .PrettyJson()
                                .DefaultIndex(defaultIndex);

            AddDefaultMapping(settings);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);

            CreateIndex(client, defaultIndex);
        }

        private static void AddDefaultMapping(ConnectionSettings settings)
        {
            settings.DefaultMappingFor<Product>(
                p => p.Ignore(x => x.Price)
                      // .Ignore(x => x.Id)
                      .Ignore(x => x.Quantity));
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            client.Indices.Create(indexName, i => i.Map<Product>(x => x.AutoMap()));
        }
    }
}
