using TestCaseBuilder.BusinessObjects;
using TestCaseBuilder.DAO;
using TestCaseBuilder.ExternalDependencies;

namespace TestCaseBuilder
{
    public class MyService : IMyService
    {
        private readonly IDbContext _db;
        private readonly IUserSessionService _userSession;
        private readonly IDocumentsSettingService _documentsSettings;
        private readonly ICustomerFetchService _customerFetch;
        private readonly IFileStorageService _fileStorage;
        private readonly IDocumentTypeConfigService _documentTypeConfig;
        private readonly ICustomerHistoryService _customerHistory;
        private readonly IHashCalculator _hashCalculator;

        public MyService(
            IDbContext db,
            IUserSessionService userSession,
            IDocumentsSettingService documentsSettings,
            ICustomerFetchService customerFetch,
            IFileStorageService fileStorage,
            IDocumentTypeConfigService documentTypeConfig,
            ICustomerHistoryService customerHistory,
            IHashCalculator hashCalculator
            )
        {
            _db = db;
            _userSession = userSession;
            _documentsSettings = documentsSettings;
            _customerFetch = customerFetch;
            _fileStorage = fileStorage;
            _documentTypeConfig = documentTypeConfig;
            _customerHistory = customerHistory;
            _hashCalculator = hashCalculator;
        }

        public UploadDocumentForCustomerResult UploadDocumentForCustomer(string customerCode, string documentFileName, byte[] document, Guid token)
        {
            UploadDocumentForCustomerResult result = new UploadDocumentForCustomerResult();

            ValidateParametersDocumentToUploadOrThrow(customerCode, documentFileName, document);

            documentFileName = documentFileName.Replace(" ", "_");
            (result.User, result.Error) = _userSession.CheckSession(token);
            if (result.HasError) return result;

            CheckIfExtensionFileIsAllowed(documentFileName, result);
            if (result.HasError) return result;

            CheckIfDocumentSizeExceedsMaxFileSize(document, result);
            if (result.HasError) return result;

            CheckIfCustomerExistsAndAccessCustomerAllowed(customerCode, result);
            if (result.HasError) return result;

            DocumentTypeConfig documentTypeConfig = _documentTypeConfig.GetByArea(result.Customer.AreaId);

            CheckIfDocumentTypeIsAllowedAfterCustomerClosing(result, documentTypeConfig);
            if (result.HasError) return result;

            CheckUploadIsAuthorizedForThisDocumentType(result, documentTypeConfig);
            if (result.HasError) return result;

            string md5Signature = CheckIfDocumentDontAlreadyExistsOnCustomer(result, document);
            if (result.HasError) return result;

            UploadDocumentAndAddToCustomer(documentFileName, document, md5Signature, result, documentTypeConfig);

            return result;
        }

        private void UploadDocumentAndAddToCustomer(string documentFileName,
                                                   byte[] document,
                                                   string md5Signature,
                                                   UploadDocumentForCustomerResult result,
                                                   DocumentTypeConfig documentTypeCpConfig)
        {
            Guid idOfFileOnStorage = UploadDocumentToStorage(documentFileName, document);
            result.NewDocument = AddDocumentToGivenCustomer(result.User, result.Customer, documentTypeCpConfig, idOfFileOnStorage, md5Signature);
            result.UrlDocumentFile = $"/Files/{idOfFileOnStorage}";
        }

        private void CheckIfDocumentSizeExceedsMaxFileSize(byte[] document, UploadDocumentForCustomerResult result)
        {
            var maxDocSize = _documentsSettings.GetMaxFileSize();
            if (document.Length > maxDocSize)
            {
                result.Error = MyServiceErrors.DocumentSizeExceedsTheMaxAllowed;
            }
        }

        private void CheckIfExtensionFileIsAllowed(string documentFileName, UploadDocumentForCustomerResult result)
        {
            var allowedFileExtensions = _documentsSettings.GetAllowedFileExtensions();
            var documentFileExtension = Path.GetExtension(documentFileName).Replace(".", "").ToLowerInvariant();
            if (!allowedFileExtensions.Any() || !allowedFileExtensions.Contains(documentFileExtension))
            {
                result.Error = MyServiceErrors.InvoiceFormatNotAllowed;
            }
        }

        private void CheckIfCustomerExistsAndAccessCustomerAllowed(string customerCode, UploadDocumentForCustomerResult result)
        {
            customerCode = customerCode.Trim();
            result.Customer = _customerFetch.GetCustomerByCustomerNumber(customerCode);
            result.CustomersWithoutSig = _customerFetch.GetCustomersWithoutSigByCustomerNumber(customerCode).ToList();
            result.Error = CheckAccessCustomer(result.Customer, result.User);
        }

        private void CheckIfDocumentTypeIsAllowedAfterCustomerClosing(UploadDocumentForCustomerResult result, DocumentTypeConfig documentTypeConfig)
        {
            if (result.Customer.ContractIsClosed && !documentTypeConfig.CanBeAddedAfterContractClosed)
                result.Error = MyServiceErrors.DocumentTypeNotAllowedAfterContractClosing;
        }

        private void CheckUploadIsAuthorizedForThisDocumentType(UploadDocumentForCustomerResult result, DocumentTypeConfig documentTypeConfig)
        {
            if (!documentTypeConfig.ApiDocumentsUploadAllowed)
                result.Error = MyServiceErrors.UploadNotAllowedThroughAPI;
        }

        private string CheckIfDocumentDontAlreadyExistsOnCustomer(UploadDocumentForCustomerResult result, byte[] document)
        {
            string md5Document = _hashCalculator.CalculateHash(new MemoryStream(document));
            ILookup<string, Guid> md5Documents = result.CustomersWithoutSig
                                                          .SelectMany(m => m.Documents.Where(x => !string.IsNullOrEmpty(x.Md5Signature)))
                                                          .ToLookup(x => x.Md5Signature, x => x.Id);
            if (md5Documents.Any(m => m.Key == md5Document))
                result.Error = MyServiceErrors.DocumentAlreadyExists;

            return md5Document;
        }

        private Guid UploadDocumentToStorage(string documentFileName, byte[] document)
        {
            var uploadedDocument = _fileStorage.Create(documentFileName);
            Guid idOfFileOnStorage = uploadedDocument.Id;
            var uploadStream = new MemoryStream(document);
            _fileStorage.Upload(uploadedDocument, uploadStream);
            return idOfFileOnStorage;
        }

        private CustomerDocument AddDocumentToGivenCustomer(
            User user,
            Customer customer,
            DocumentTypeConfig documentTypeCpConfig,
            Guid idOfFileOnStorage,
            string md5Signature)
        {
            CustomerDocument newInvoiceOrCreditNoteDocument = new CustomerDocument
            {
                FileId = idOfFileOnStorage,
                Customer = customer,
                Date = DateTime.UtcNow,
                ExternalAccess = documentTypeCpConfig.ExternalAccess,
                Md5Signature = md5Signature
            };
            customer.Documents.Add(newInvoiceOrCreditNoteDocument);
            _customerHistory.UpdateCustomer(customer.CustomerCodeWithoutSig + "_" + customer.CustomerSig, user.Login, CustomerHistoryEvent.CustomerUpdate);
            _db.SaveChanges();
            return newInvoiceOrCreditNoteDocument;
        }

        private static void ValidateParametersDocumentToUploadOrThrow(string customerCode, string documentFileName, byte[] document)
        {
            if (string.IsNullOrWhiteSpace(customerCode))
            {
                throw new ArgumentNullException(nameof(customerCode), $"{nameof(customerCode)} is required");
            }

            if (string.IsNullOrWhiteSpace(documentFileName))
            {
                throw new ArgumentNullException(nameof(documentFileName), $"{nameof(documentFileName)} is required");
            }

            if (document == null || document.Length == 0)
            {
                throw new ArgumentNullException(nameof(document), $"{nameof(document)} is required");
            }
        }

        private MyServiceErrors CheckAccessCustomer(Customer customer, User user)
        {
            var areaIds = user.Areas.Select(n => n.Id).Distinct().ToList();
            if (customer == null || !areaIds.Contains(customer.AreaId))
            {
                return MyServiceErrors.CustomerNotFound;
            }

            return MyServiceErrors.None;
        }
    }
}
