namespace Contents.Business.Model
{
    public class BaseResult<T>
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public BaseResult()
        {
            IsSuccess = true;
        }
    }
}
