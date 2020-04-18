﻿using Billing.API.Data;
using Billing.API.Exceptions.Invoices;
using Billing.API.Models;
using Billing.API.Services.Repositories.Implementations;
using Billing.API.Services.Services.Implementations;
using Billing.API.ViewModels;
using Billing.Tests.FakeImplementations;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shared.Extensions.HttpClientHandler;
using Shared.Utiliies.CloudStorage.Shared.Models.BaseResult;
using Shared.Utilities.Billing.Billingo.Services.Implementations;
using Shared.Utilities.Billing.Shared.Services.Abstractions;
using Shared.Utilities.Billing.Shared.ViewModels;
using Shared.Utilities.CloudStorage.Shared.Services.Abstractions;
using Shared.Utilities.EmailSender.Core.Services.Abstractions;
using Shared.Utilities.EmailSender.Shared.Services.Abstractions;
using Shared.Utilities.EmailSender.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Billing.Tests.API.Services.InvoiceServiceTests
{
    public class CreateMethodTests
    {


        [Fact]
        public async void Create_ShouldBeOkay()
        {
            //Arrange
            var dbOptions = FakeDbCreatorFactory.CreateDbOptions(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            await FakeDbCreatorFactory.InitDbContext(dbOptions);
            var dbContext = new InvoicesDbContext(dbOptions);

            var expectedSystemID = "test";
            var expectedUserID = "kriszw";

            var userInvoiceRepo = new UserInvoiceRepository(dbContext);
            var repo = new InvoiceRepository(dbContext, userInvoiceRepo);

            var model = await CreateModel(dbContext, expectedUserID);
            var service = CreateInvoiceService(repo, model, expectedSystemID);

            //Act
            await service.Create(model);
            var actual = await repo.GetInvoceByID(model.Invoice.ID.GetValueOrDefault());

            //Assert
            Assert.Equal(expectedSystemID, actual.BillingSystemInvoiceID);
            Assert.False(actual.Canceled);
            Assert.NotNull(actual.FileID);
        }

        [Fact]
        public async void Create_ShouldThrowInvoiceNotFoundException()
        {
            //Arrange
            var dbOptions = FakeDbCreatorFactory.CreateDbOptions(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            await FakeDbCreatorFactory.InitDbContext(dbOptions);
            var dbContext = new InvoicesDbContext(dbOptions);

            var expectedSystemID = "test";
            var expectedUserID = "kriszw";

            var userInvoiceRepo = new UserInvoiceRepository(dbContext);
            var repo = new InvoiceRepository(dbContext, userInvoiceRepo);

            var model = await CreateModel(dbContext, expectedUserID);
            var service = CreateInvoiceService(repo, model, expectedSystemID);

            model.Invoice.ID = 100;

            //Act
            var createTask = service.Create(model);

            //Assert
            await Assert.ThrowsAsync<InvoiceNotFoundByIDException>(() => createTask);
        }



        private static async Task<InvoiceCreationViewModel> CreateModel(InvoicesDbContext dbContext, string userID)
        {
            var invoice = await dbContext.Invoices.AsNoTracking().Include(i=> i.File).Include(i=> i.Items).FirstOrDefaultAsync();
            return new InvoiceCreationViewModel() { UserID = userID, IsCanceledInvoice = false, Invoice = invoice };
        }

        private static InvoiceService CreateInvoiceService(InvoiceRepository repo, InvoiceCreationViewModel model, string expectedSystemID)
        {
            var mockedApiFileManager = new Mock<IBillingAPIFileManager>();
            mockedApiFileManager.Setup(bFile => bFile.DownloadInvoice(expectedSystemID)).ReturnsAsync(default(Stream));

            var mockedBillingService = new Mock<IBillingService>();
            mockedBillingService
                .Setup(bService => bService.CreateInvoiceAsync(default))
                .ReturnsAsync(new InvoiceCreationResultViewModel() { BillingSystemID = expectedSystemID });

            var mockedStorageService = new Mock<IStorageService>();
            mockedStorageService
                .Setup(sService => sService.UploadWithContainerExtension(model.UserID, model.Invoice.OrderID.ToString(), default(Stream)))
                .ReturnsAsync(new CloudStorageUploadResult(true, $"{model.Invoice.OrderID}-szamla.pdf"));

            var mockedEmailSender = new Mock<IEmailSenderService>();
            mockedEmailSender
                .Setup(service => service.SendMail(CreateEmailViewModel(model)))
                .ReturnsAsync(new EmailSendResult());

            return new InvoiceService(repo, mockedApiFileManager.Object, mockedBillingService.Object,
                mockedStorageService.Object, mockedEmailSender.Object);
        }

        private static EmailSendModelWithAttachmentsViewModel CreateEmailViewModel(InvoiceCreationViewModel model)
        {
            return new EmailSendModelWithAttachmentsViewModel()
            {
                Subject = $"#{model.Invoice.OrderID} rendelés e-számlája",
                From = new EmailAddress("billing@kwsoft.dev", $"E-Számlázás EzG"),
                To = new List<EmailAddress>() { new EmailAddress(model.Invoice.UserEmail, $"{model.Invoice.LastName} {model.Invoice.FirstName} ") },
                Attachments = new List<AttachmentViewModel>()
                    {
                        new AttachmentViewModel()
                        {
                            FileName = $"{model.Invoice.OrderID}-szamla.pdf",
                            FileStream = new MemoryStream(),
                            ContentType = "application/pdf"
                        }
                    },
                Body = "",
            };
        }
    }
}
