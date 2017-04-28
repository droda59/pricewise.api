namespace PriceAlerts.PriceCheckJob.Emails
{
    public class ApiResponse<T>
    {
        public bool success = false;

        public string error = null;
        
        public T Data { get; set; }
    }

    public static class ApiTypes
    {
        public class EmailSend
        {
            public string TransactionID { get; set; }

            public string MessageID { get; set; }
        }
    }
}
