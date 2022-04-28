

using Cx.Data;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Cx.Mongo
{
    public class CxMongo_DB<T> where T : iBaseMongo
    {
        private static IConfiguration dbset = ConfigExtensions.GetConfig("~/config/mongodb.json");

        MongoClient? client;
        IMongoDatabase? database;
        public IMongoCollection<T>? collection;
        public CxMongo_DB(string Key)
        {
            string source = Base.GetAppKeyValue("Mongo:Source");
            string dbname = dbset.GetSection(Key + ":database").Value;
            string tbname = dbset.GetSection(Key + ":table").Value;
            try
            {
                MongoUrl connectionString = new MongoUrl(source);
                //client = new MongoClient(new MongoUrl(model.SOURCE));
                client = new MongoClient(connectionString);// MongoClient(model.SOURCE);
                database = client.GetDatabase(dbname);
                collection = database.GetCollection<T>(tbname);
            }
            catch (Exception e) { Logger.Default.Process("system", "CxMongo_DB", "Mongo初始化错误，Key:" + Key + ";message:" + e.Message); }
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> InsertAsync(T entity)
        {
            var flag = ObjectId.GenerateNewId();
            entity._id = flag;
            await collection.InsertOneAsync(entity);
            return entity;
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Insert(T entity)
        {
            var flag = ObjectId.GenerateNewId();
            entity._id = flag;
            collection.InsertOne(entity);
            return entity;
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public async Task<int> Modify(string id, string field, string value)
        {
            var filter = Builders<T>.Filter.Eq("Id", ObjectId.Parse(id));
            var updated = Builders<T>.Update.Set(field, value);
            UpdateResult result = await collection.UpdateOneAsync(filter, updated);
            return result.MatchedCount > 0 ? 1 : 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public async Task<int> ModifyDIY(string keyTitle, string keyValue, string field, string value)
        {
            var filter = Builders<T>.Filter.Eq(keyTitle, keyValue);
            var updated = Builders<T>.Update.Set(field, value);
            UpdateResult result = await collection.UpdateOneAsync(filter, updated);
            return result.MatchedCount > 0 ? 1 : 0;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        public async Task<int> Update(T entity)
        {
            try
            {
                var old = collection.Find(e => e._id.Equals(entity._id)).FirstOrDefault();

                foreach (var prop in entity.GetType().GetProperties())
                {
                    var newValue = prop.GetValue(entity);
                    var oldValue = old.GetType().GetProperty(prop.Name).GetValue(old);
                    if (newValue != null)
                    {
                        if (oldValue == null)
                            oldValue = "";
                        if (!newValue.ToString().Equals(oldValue.ToString()))
                        {
                            old.GetType().GetProperty(prop.Name).SetValue(old, newValue.ToString());
                        }
                    }
                }


                var filter = Builders<T>.Filter.Eq("_id", entity._id);
                ReplaceOneResult result = await collection.ReplaceOneAsync(filter, old);
                return result.MatchedCount > 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                var aaa = ex.Message + ex.StackTrace;
                throw;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        public async Task<bool> Delete(T entity)
        {
            var filter = Builders<T>.Filter.Eq("Id", entity._id);
            DeleteResult result = await collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        public async Task<bool> Delete(string Id)
        {
            var filter = Builders<T>.Filter.Eq("Id", ObjectId.Parse(Id));
            DeleteResult result = await collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
        /// <summary>
        /// 根据id查询一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T QueryOne(string id)
        {
            return collection.Find(a => a._id == ObjectId.Parse(id)).FirstOrDefault();
        }
        /// <summary>
        /// 根据id查询一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T QueryOne(FilterDefinition<T> filter1)
        {
            return collection.Find(filter1).FirstOrDefault();
        }
        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public List<T> QueryAll()
        {
            return collection.Find(null).ToList();
        }

        public List<T> QueryAll(FilterDefinition<T> filter1)
        {
            return collection.Find(filter1).ToList();
        }

        public List<T> QueryAll(FilterDefinition<T> filter1, SortDefinition<T> sort, int TopNum)
        {
            return collection.Find(filter1).Sort(sort).Limit(TopNum).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter1"></param>
        /// <param name="OrderBy"></param>
        /// <param name="SortDesc"></param>
        /// <param name="CuPage"></param>
        /// <param name="PageSize"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public List<T> QueryAll(FilterDefinition<T> filter1, SortDefinition<T> sort, int CuPage, int PageSize, ref long totalcount)
        {
            //collection.Find(m=>m.tit)

            // var task1 = Task.Run(() => { return collection.CountDocuments(filter1); });
            var task1 = collection.CountDocumentsAsync(filter1);
            int Skip = (CuPage - 1) * PageSize;
            var rt = collection.Find(filter1)
                                .Sort(sort)
                                .Skip(Skip).Limit(PageSize).ToList();
            totalcount = task1.Result;
            return rt;
        }

        /// <summary>
        /// 根据条件查询一条数据
        /// </summary>
        /// <param name="express"></param>
        /// <returns></returns>
        public T QueryByFirst(Expression<Func<T, bool>> express)
        {
            return collection.Find(express).FirstOrDefault();
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="list"></param>
        public async void InsertBatch(List<T> list)
        {
            await collection.InsertManyAsync(list);
        }
        /// <summary>
        /// 根据Id批量删除
        /// </summary>
        public async void DeleteBatch(List<ObjectId> list)
        {
            var filter = Builders<T>.Filter.In("_id", list);
            await collection.DeleteManyAsync(filter);
        }
    }
}