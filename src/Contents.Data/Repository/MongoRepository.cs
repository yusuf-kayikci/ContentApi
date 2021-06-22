using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Contents.Common.Config;
using Contents.Data.Abstraction;
using Contents.Data.Attributes;
using Contents.Data.Entity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Contents.Data.Repository
{
    public class MongoRepository<TDocument> : IMongoRepository<TDocument>
        where TDocument : IDocument
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<TDocument> _collection;
        private readonly string _collectionName;
        public MongoRepository(IMongoDbSettings settings)
        {
            _collectionName = GetCollectionName(typeof(TDocument));
            _database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            _collection = _database.GetCollection<TDocument>(_collectionName);
        }

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
        }

        public async Task<IEnumerable<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression)
        {
            var documents = await _collection.FindAsync(filterExpression);
            return await documents.ToListAsync();
        }

        public TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).FirstOrDefault();
        }

        public void InsertMany(ICollection<TDocument> documents)
        {
            _collection.InsertMany(documents);
        }

        public async Task InsertManyAsync(ICollection<TDocument> documents)
        {            
            await _collection.InsertManyAsync(documents);
        }

        public void InsertOne(TDocument document)
        {
            _collection.InsertOne(document);
        }

        public async Task InsertOneAsync(TDocument document)
        {
            await _collection.InsertOneAsync(document);
        }

        public ReplaceOneResult ReplaceOne(
            Expression<Func<TDocument, bool>> filterExpression, 
            TDocument document)
        {
            return _collection.ReplaceOne(filterExpression, document);
        }

        public async Task<ReplaceOneResult> ReplaceOneAsync(
            Expression<Func<TDocument, bool>> filterExpression, 
            TDocument document)
        {
            return await _collection.ReplaceOneAsync(filterExpression, document);
        }

        public TDocument FindOneAndReplace(
            Expression<Func<TDocument, bool>> filterExpression, 
            TDocument replacement)
        {
            return _collection.FindOneAndReplace(filterExpression, replacement);
        }

        public TResult Aggregate<TResult>(
            Expression<Func<TDocument, bool>> filterExpression, 
            string from, 
            string localField, 
            string foreignField, 
            string @as)
        {
            BsonDocument lookupResult = _collection
                .Aggregate()
                .Match(filterExpression)
                .Lookup(from, localField, foreignField, @as)
                .FirstOrDefault();

            if (lookupResult == null)
            {
                return default(TResult);
            }

            var serializedItem = BsonSerializer.Deserialize<TResult>(lookupResult);

            return serializedItem;
        }
    }
}
