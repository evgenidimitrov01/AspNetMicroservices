using Catalog.API.Constants;
using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>(ApplicationConstants.ConnectionString));
            var database = client.GetDatabase(configuration.GetValue<string>(ApplicationConstants.DatabaseName));

            Products = database.GetCollection<Product>(configuration.GetValue<string>(ApplicationConstants.CollectionName));
            CatalogContextSeed.SeedData(Products);
        }

        public IMongoCollection<Product> Products { get; }
    }
}
