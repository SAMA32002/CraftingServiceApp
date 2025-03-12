namespace CraftingServiceApp.AdminAPI.Helpers
{
    public class ContentContainer<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }

        public ContentContainer(T data, string message = null)
        {
            Data = data;
            Message = message ?? "Operation successful";
        }
    }
}
