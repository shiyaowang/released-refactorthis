using RefactorThis.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Persistence.Services
{
    public interface IInvoiceService
    {
        string ProcessPayment(Payment payment);
    }
}
