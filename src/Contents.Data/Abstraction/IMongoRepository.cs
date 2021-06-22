using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Contents.Data.Entity;
using MongoDB.Driver;

namespace Contents.Data.Abstraction
{
    public interface IMongoRepository<TDocument> where TDocument : IDocument
    {
        Task<IEnumerable<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression);

        TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression);

        void InsertOne(TDocument document);

        Task InsertOneAsync(TDocument document);

        void InsertMany(ICollection<TDocument> documents);

        Task InsertManyAsync(ICollection<TDocument> documents);

        ReplaceOneResult ReplaceOne(Expression<Func<TDocument, bool>> filterExpression, TDocument document);

        Task<ReplaceOneResult> ReplaceOneAsync(Expression<Func<TDocument, bool>> filterExpression, TDocument document);

        TDocument FindOneAndReplace(Expression<Func<TDocument, bool>> filterExpression, TDocument replacement);

        TResult Aggregate<TResult>(Expression<Func<TDocument, bool>> filterExpression, string from, string localField, string foreignField, string @as);
    }
}
