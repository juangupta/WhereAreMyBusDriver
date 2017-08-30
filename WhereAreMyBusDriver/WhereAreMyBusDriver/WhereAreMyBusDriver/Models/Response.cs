namespace WhereAreMyBusDriver.Models
{
    public class Response
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public object Result { get; set; }

        public string Extra { get; set; }
    }
}
