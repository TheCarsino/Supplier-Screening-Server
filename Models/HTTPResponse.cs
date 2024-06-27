namespace Supplier_Screening_Server.Models
{
    public class HTTPResponse
    {
        public class ErrorResponse
        {
            public string message { get; set; }
            public int status { get; set; }
            public object details { get; set; }
        }

        public class SuccessResponse
        {
            public string message { get; set; }
            public int status { get; set; }
            public object data { get; set; }
        }
        public class ErrorValidResponse
        {
            public string title { get; set; }
            public int status { get; set; }
            public object errors { get; set; }
        }
    }
}
