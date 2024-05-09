using System.Collections.Generic;

namespace RefactorThis.Persistence.Models
{
    public class Invoice
    {
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TaxAmount { get; set; }
        public List<Payment> Payments { get; set; }
        public InvoiceType Type { get; set; }
        public string Reference { get; set; }

        public void RecordPayment(Payment payment)
        {
            Payments.Add(payment);
        }
    }

    public enum InvoiceType
    {
        Standard,
        Commercial
    }
}