using MongoDB.Bson;

namespace Cx.Mongo
{
    /// <summary> 
    /// mongo基础类
    /// </summary>
    public interface iBaseMongo
    {
        ObjectId _id { get; set; }
    }
    /// <summary> 
    /// mongo基础类
    /// </summary>
    [MongoDB.Bson.Serialization.Attributes.BsonIgnoreExtraElements]
    public abstract class BaseM_Mongo
    {
        [Newtonsoft.Json.JsonIgnore]
        public ObjectId _id { get; set; }
    }
}