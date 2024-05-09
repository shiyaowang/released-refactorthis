using RefactorThis.Persistence.Models;
using RefactorThis.Persistence.Services;
using System;

namespace RefactorThis.Domain
{
    public class InvoiceService : IInvoiceService
    {
        private readonly InvoiceRepository _invoiceRepository;

        public InvoiceService(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
        }

        public string ProcessPayment(Payment payment)
        {
            var invoice = _invoiceRepository.GetInvoice(payment.Reference) ?? throw new InvalidOperationException("There is no invoice matching this payment");
            var responseMessage = ProcessInvoicePayment(invoice, payment);

            _invoiceRepository.SaveInvoice(invoice);

            return responseMessage;
        }

        private string ProcessInvoicePayment(Invoice invoice, Payment payment)
        {
            var remainingAmount = invoice.Amount - invoice.AmountPaid;
            var taxRate = invoice.Type == InvoiceType.Commercial ? 0.14m : 0;

            if (remainingAmount == 0)
            {
                if (invoice.Payments.Count == 0)
                {
                    return "No payment needed";
                }
                else
                {
                    throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
                }
            }

            if (payment.Amount > remainingAmount)
            {
                return "The payment is greater than the invoice amount";
            }

            invoice.AmountPaid += payment.Amount;
            invoice.TaxAmount += payment.Amount * taxRate;
            invoice.Payments.Add(payment);

            if (invoice.AmountPaid == invoice.Amount)
            {
                return "Invoice is now fully paid";
            }
            else if (invoice.AmountPaid < invoice.Amount)
            {
                return "Invoice is now partially paid";
            }

            throw new InvalidOperationException("Invalid invoice state after processing payment");
        }
    }
}
