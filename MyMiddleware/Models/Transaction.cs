 
    public class Transaction
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
 
