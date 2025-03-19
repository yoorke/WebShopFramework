namespace eshop.PaymentProcessor.BE
{
    public class IPSQRCodeRequest
    {
        public string ServiceUrl { get; set; }
        public int QRCodeSize { get; set; }
        public string PayeeAccount { get; set; }
        public string PayeeName { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string PayerName { get; set; }
        public int PaymentCode { get; set; }
        public string Purpose { get; set; }
        public string Model { get; set; }
        public string ReferenceNumber { get; set; }
        public string IPSCode { get; set; }
        public string IPSVersion { get; set; }
        public string IPSCharSet { get; set; }
        public string DecimalSeparator { get; set; }
        public string ImageType { get; set; }
    }
}
