using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public static class MongoDBConfig
    {
        public static void RegisterClassMaps()
        {
            var conventionPack = new ConventionPack();
            conventionPack.Add(new CamelCaseElementNameConvention());
            ConventionRegistry.Register("camelCase", conventionPack, t => true);

            BsonClassMap.RegisterClassMap<Document>(
                x =>
                    {
                        x.AutoMap();
                        x.MapIdMember(y => y.Id)
                            .SetSerializer(new StringSerializer(BsonType.ObjectId))
                            .SetIdGenerator(StringObjectIdGenerator.Instance);
                    });
            
            BsonClassMap.RegisterClassMap<User>(x => x.AutoMap());
            BsonClassMap.RegisterClassMap<MonitoredProduct>(x => x.AutoMap());
        }
    }
}