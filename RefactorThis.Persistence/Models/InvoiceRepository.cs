using RefactorThis.Persistence.Services;
using System.Collections.Generic;

namespace RefactorThis.Persistence.Models
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly List<Invoice> _invoices;

        public InvoiceRepository()
        {
            _invoices = new List<Invoice>();
        }

        public void SaveInvoice(Invoice invoice)
        {
            // Save the invoice to the database
            // Implementation omitted for brevity
        }

        public Invoice GetInvoice(string reference)
        {
            return _invoices.Find(invoice => invoice.Reference == reference);
        }

        public void AddInvoice(Invoice invoice)
        {
            _invoices.Add(invoice);
        }
    }
}