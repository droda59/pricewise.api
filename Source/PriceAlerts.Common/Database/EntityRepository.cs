using MongoDB.Driver;

namespace PriceAlerts.Common.Database
{
    public abstract class EntityRepository<TDocument>
    {
        protected EntityRepository()
        {
            var client = new MongoClient("mongodb://admin:admin@ds159050.mlab.com:59050/pricealerts");
            var database = client.GetDatabase("pricealerts");

            this.Collection = database.GetCollection<TDocument>(typeof(TDocument).Name.ToLower());
        }

        protected IMongoCollection<TDocument> Collection { get; }
    }
}