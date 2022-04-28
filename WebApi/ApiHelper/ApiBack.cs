namespace WebApi
{
    public class TApi_Result
    {
        /// <summary>
        /// 返回状态值 200代表成功
        /// </summary>
        public int code = 200;

        /// <summary>
        /// 返回信息
        /// </summary>
        public string message = "";
    }

    public class TApi_Result_Data<T> : TApi_Result where T : class, new()
    {
        public T data = new T();
    }

    public class TApi_Result_List<T> : TApi_Result where T : class, new()
    {
        public List<T> data = new List<T>();
    }
}