using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data
{
    public class DbInitializer
    {
        public static async Task InitDB(WebApplication app)
        {
            await DB.InitAsync("SearchDb", MongoClientSettings
    .FromConnectionString(app.Configuration.GetConnectionString("MongoDBConnection")));

            await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Year, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();


            var count = await DB.CountAsync<Item>();


            using var Scope = app.Services.CreateScope();

            var AuctionServiceHttpClient = Scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

            var items = await AuctionServiceHttpClient.GetItemsForSearchDb();

            Console.WriteLine("Items from AuctionService: " + items.Count);

            if (items.Count > 0)
            {
                await DB.SaveAsync(items);
            }
        }
    }
}
