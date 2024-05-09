using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using RefactorThis.Persistence.Models;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
	public class InvoicePaymentProcessorTests
	{
		[Test]
		public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference( )
		{
			var repo = new InvoiceRepository( );

			Invoice invoice = null;
			var paymentProcessor = new InvoiceService( repo );

			var payment = new Payment( );
			var failureMessage = "";

			try
			{
				var result = paymentProcessor.ProcessPayment( payment );
			}
			catch ( InvalidOperationException e )
			{
				failureMessage = e.Message;
			}

			ClassicAssert.AreEqual( "There is no invoice matching this payment", failureMessage );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded( )
		{
			var repo = new InvoiceRepository();

			var invoice = new Invoice()
			{
				Amount = 0,
				AmountPaid = 0,
				Payments = null
			};

			repo.AddInvoice( invoice );

			var paymentProcessor = new InvoiceService( repo );

			var payment = new Payment( );

			var result = paymentProcessor.ProcessPayment( payment );

            ClassicAssert.AreEqual( "no payment needed", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid( )
		{
			var repo = new InvoiceRepository( );

			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 10,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 10
					}
				}
			};
			repo.AddInvoice( invoice );

			var paymentProcessor = new InvoiceService( repo );

			var payment = new Payment( );

			var result = paymentProcessor.ProcessPayment( payment );

            ClassicAssert.AreEqual( "invoice was already fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 5,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 5
					}
				}
			};
			repo.AddInvoice( invoice );

			var paymentProcessor = new InvoiceService( repo );

			var payment = new Payment( )
			{
				Amount = 6
			};

			var result = paymentProcessor.ProcessPayment( payment );

            ClassicAssert.AreEqual( "the payment is greater than the partial amount remaining", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice()
			{
				Amount = 5,
				AmountPaid = 0,
				Payments = new List<Payment>( )
			};
			repo.AddInvoice( invoice );

			var paymentProcessor = new InvoiceService( repo );

			var payment = new Payment( )
			{
				Amount = 6
			};

			var result = paymentProcessor.ProcessPayment( payment );

            ClassicAssert.AreEqual( "the payment is greater than the invoice amount", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 5,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 5
					}
				}
			};
			repo.AddInvoice( invoice );

			var paymentProcessor = new InvoiceService( repo );

			var payment = new Payment( )
			{
				Amount = 5
			};

			var result = paymentProcessor.ProcessPayment( payment );

            ClassicAssert.AreEqual( "final partial payment received, invoice is now fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>( ) { new Payment( ) { Amount = 10 } }
			};
			repo.AddInvoice( invoice );

			var paymentProcessor = new InvoiceService( repo );

			var payment = new Payment( )
			{
				Amount = 10
			};

			var result = paymentProcessor.ProcessPayment( payment );

            ClassicAssert.AreEqual( "invoice was already fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 5,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 5
					}
				}
			};
			repo.AddInvoice( invoice );

			var paymentProcessor = new InvoiceService( repo );

			var payment = new Payment( )
			{
				Amount = 1
			};

			var result = paymentProcessor.ProcessPayment( payment );

            ClassicAssert.AreEqual( "another partial payment received, still not fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount( )
		{
			var repo = new InvoiceRepository( );
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>( )
			};
			repo.AddInvoice( invoice );

			var paymentProcessor = new InvoiceService( repo );

			var payment = new Payment( )
			{
				Amount = 1
			};

			var result = paymentProcessor.ProcessPayment( payment );

            ClassicAssert.AreEqual( "invoice is now partially paid", result );
		}

        [Test]
        public void ProcessPayment_Should_ReturnNoPaymentNeededMessage_When_InvoiceAmountIsZero()
        {
            var repo = new InvoiceRepository();

            var invoice = new Invoice()
            {
                Amount = 0,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            repo.AddInvoice(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment()
            {
                Amount = 0
            };

            var result = paymentProcessor.ProcessPayment(payment);

            ClassicAssert.AreEqual("No payment needed", result);
        }

        [Test]
        public void ProcessPayment_Should_ApplyTaxForCommercialInvoice()
        {
            var repo = new InvoiceRepository();

            var invoice = new Invoice()
            {
                Amount = 100,
                AmountPaid = 0,
                Type = InvoiceType.Commercial,
                Payments = new List<Payment>()
            };
            repo.AddInvoice(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment()
            {
                Amount = 50
            };

            var result = paymentProcessor.ProcessPayment(payment);

            ClassicAssert.AreEqual("final partial payment received, invoice is now fully paid", result);
            ClassicAssert.AreEqual(7, invoice.TaxAmount); // Assuming the tax rate is 0.14 and the payment amount is 50
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_PartialPaymentExceedsRemainingAmount()
        {
            var repo = new InvoiceRepository();

            var invoice = new Invoice()
            {
                Amount = 100,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            repo.AddInvoice(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment()
            {
                Amount = 150 // Exceeds the invoice amount
            };

            ClassicAssert.Throws<InvalidOperationException>(() => paymentProcessor.ProcessPayment(payment));
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_PaymentAmountIsNegative()
        {
            var repo = new InvoiceRepository();

            var invoice = new Invoice()
            {
                Amount = 100,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            repo.AddInvoice(invoice);

            var paymentProcessor = new InvoiceService(repo);

            var payment = new Payment()
            {
                Amount = -50 // Negative payment amount
            };

            ClassicAssert.Throws<InvalidOperationException>(() => paymentProcessor.ProcessPayment(payment));
        }

    }
}