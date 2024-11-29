using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
IConfigurationRoot root = builder.Build();

string? redisConnectionString = root.GetValue<string>("ConnectionStrings:CacheForRedis");

if (string.IsNullOrEmpty(redisConnectionString))
{
    throw new ArgumentNullException(nameof(redisConnectionString));
}

void SetItem(IDatabase database, string key) {
    var product = database.StringGet(key);

    Console.WriteLine($"Product: {product}");

    if (!product.HasValue)
    {
        database.StringSet(key, "New Product", TimeSpan.FromSeconds(60));

        Console.WriteLine("Setting product value...");
    }

    product = database.StringGet(key);

    Console.WriteLine($"Product: {product}");
}

using (var redis = ConnectionMultiplexer.Connect(redisConnectionString))
{
    var database = redis.GetDatabase();

    string key = "product:123";

    SetItem(database, key);
}
