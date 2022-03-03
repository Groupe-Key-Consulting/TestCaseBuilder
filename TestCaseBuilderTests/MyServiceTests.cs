using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCaseBuilder;
using TestCaseBuilder.BusinessObjects;
using TestCaseBuilder.DAO;
using TestCaseBuilder.ExternalDependencies;
using TestCaseBuilderTests.TestImplementations;

namespace TestCaseBuilderTests
{
    [TestFixture]
    internal class MyServiceTests
    {
        private TestCaseBuilder CurrentTestCase;

        [SetUp]
        public void BeforeEachTest()
        {
            CurrentTestCase = new TestCaseBuilder();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Throw_ArgumentNullException_When_CustomerNumber_Is_Null()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerCode = null;
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerCode, null);
            var myService = CurrentTestCase.GetServiceTested();

            Action callWithNullCustomerNumber = () => myService.UploadDocumentForCustomer(
                customerCode,
                documentFileName,
                new byte[documentSize],
                Guid.NewGuid());

            callWithNullCustomerNumber.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("customerCode");
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Throw_ArgumentNullException_When_CustomerNumber_Is_Whitespace()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "    ";
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerNumber, null);
            var myService = CurrentTestCase.GetServiceTested();

            Action callWithNullCustomerNumber = () => myService.UploadDocumentForCustomer(
                customerNumber, documentFileName, new byte[documentSize], Guid.NewGuid());

            callWithNullCustomerNumber.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("customerCode");
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Throw_ArgumentNullException_When_DocumentFileName_Is_Null()
        {
            const string documentFileName = null;
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerNumber, null);
            var myService = CurrentTestCase.GetServiceTested();

            Action callWithNullDocumentFileName = () => myService.UploadDocumentForCustomer(
                customerNumber, documentFileName, new byte[documentSize], Guid.NewGuid());

            callWithNullDocumentFileName.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("documentFileName");
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Throw_ArgumentNullException_When_Document_Is_Null()
        {
            byte[] documentData = null;
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerNumber, null);
            var myService = CurrentTestCase.GetServiceTested();

            Action callWithNullDocument = () => myService.UploadDocumentForCustomer(
                customerNumber, "Invoice.pdf", documentData, Guid.NewGuid());

            callWithNullDocument.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("document");
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Return_SessionIsNotValid_When_Session_Is_Not_Valid()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerNumber, null)
                           .But.User_Session_Is_Not_Valid();
            var myService = CurrentTestCase.GetServiceTested();

            var result = myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            result.Error.Should().Be(MyServiceErrors.SessionIsNotValid);
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Return_FormatNotAllowed_When_FileExtension_Is_Not_In_Allowed_Extensions()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerNumber, null)
                           .But.File_Extension_Is_Not_Allowed(documentFileName);
            var myService = CurrentTestCase.GetServiceTested();

            var result = myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            result.Error.Should().Be(MyServiceErrors.InvoiceFormatNotAllowed);
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Return_DocumentTypeNotAllowedAfterContractClosing_When_Contract_Is_Closed_And_Upload_Is_Not_Allowed()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerNumber, null)
                           .But.Documents_Are_Not_Allowed_After_Contract_Closing_And_Contract_Is_Closed();
            var myService = CurrentTestCase.GetServiceTested();

            var result = myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            result.Error.Should().Be(MyServiceErrors.DocumentTypeNotAllowedAfterContractClosing);
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Return_FormatNotAllowed_When_No_FileExtension_Is_Allowed()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerNumber, null)
                           .But.No_File_Extension_Is_Allowed();
            var myService = CurrentTestCase.GetServiceTested();

            var result = myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            result.Error.Should().Be(MyServiceErrors.InvoiceFormatNotAllowed);
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Return_DocumentSizeExceedsTheMaxAllowed_When_Document_Is_Bigger_Than_MaxSize()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerNumber, null)
                           .But.Document_Size_Is_Bigger_Than_Maximum_File_Size(documentSize);
            var myService = CurrentTestCase.GetServiceTested();

            var result = myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            result.Error.Should().Be(MyServiceErrors.DocumentSizeExceedsTheMaxAllowed);
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [TestCase("?")]
        [TestCase("no underscore")]
        [TestCase("_0000")]
        [TestCase("AbrahamLincoln_0000")]
        public void UploadDocumentForCustomer_Should_Return_CustomerNotFound_When_Given_CustomerNumber_Is_Malformed(string customerNumber)
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerNumber, null)
                .But.No_Customer_Exist_With_This_Number(customerNumber);
            var myService = CurrentTestCase.GetServiceTested();

            var result = myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            result.Error.Should().Be(MyServiceErrors.CustomerNotFound);
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Return_CustomerNotFound_When_No_Customer_Exists_With_Given_CustomerNumber()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerNumber, null)
                .But.No_Customer_Exist_With_This_Number(customerNumber);
            var myService = CurrentTestCase.GetServiceTested();

            var result = myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            result.Error.Should().Be(MyServiceErrors.CustomerNotFound);
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Return_CustomerNotFound_When_User_Has_No_Access_To_Customer_Area()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerNumber, null)
                           .But.User_Does_Not_Have_Access_To_The_Area_Of_The_Customer(customerNumber);
            var myService = CurrentTestCase.GetServiceTested();

            var result = myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            result.Error.Should().Be(MyServiceErrors.CustomerNotFound);
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Return_UploadNotAllowedThroughAPI_When_Upload_Through_API_Is_Not_Allowed()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerNumber, null)
                           .But.Upload_Is_Not_Allowed_On_API_For_This_Document_Type();
            var myService = CurrentTestCase.GetServiceTested();

            var result = myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            result.Error.Should().Be(MyServiceErrors.UploadNotAllowedThroughAPI);
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Return_DocumentAlreadyExists_When_Same_Document_Is_Already_Uploaded_For_Customer()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            
            Guid expectedFileId = Guid.NewGuid();
            const string md5Signature = "597FF3F1CA5EBFD58CD7598D363810AF";

            CurrentTestCase.Is.SessionService_Returns_No_Error()
                           .And.File_Extension_Is_Allowed_For_Name(documentFileName)
                           .And.User_Has_Access_To_The_Area_Of_The_Customer(customerNumber)
                           .And.Maximum_File_Size_Is_At_Least(documentSize)
                           .And.DocumentType_Has_Valid_Configuration_On_Same_Area_As_Customer()
                           .And.FileStorage_Will_CreateFile_With_Id(expectedFileId)
                           .But.Customer_Already_Has_A_Document_With_Same_Md5Signature(md5Signature);
            var myService = CurrentTestCase.GetServiceTested();

            var result = myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize], Guid.NewGuid());

            result.Error.Should().Be(MyServiceErrors.DocumentAlreadyExists);
            CurrentTestCase.Assert_No_Upload_And_No_Save_Was_Done();
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Return_No_Error_When_Process_Is_Successful()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            
            Guid expectedFileId = Guid.NewGuid();
            CurrentTestCase.Is.SessionService_Returns_No_Error()
                           .And.File_Extension_Is_Allowed_For_Name(documentFileName)
                           .And.User_Has_Access_To_The_Area_Of_The_Customer(customerNumber)
                           .And.Maximum_File_Size_Is_At_Least(documentSize)
                           .And.DocumentType_Has_Valid_Configuration_On_Same_Area_As_Customer()
                           .And.FileStorage_Will_CreateFile_With_Id(expectedFileId);
            var myService = CurrentTestCase.GetServiceTested();

            var result = myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            result.Error.Should().Be(MyServiceErrors.None);
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Create_And_Upload_File_When_Process_Is_Successful()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            
            Guid expectedFileId = Guid.NewGuid();
            CurrentTestCase.Is.SessionService_Returns_No_Error()
                           .And.File_Extension_Is_Allowed_For_Name(documentFileName)
                           .And.User_Has_Access_To_The_Area_Of_The_Customer(customerNumber)
                           .And.Maximum_File_Size_Is_At_Least(documentSize)
                           .And.DocumentType_Has_Valid_Configuration_On_Same_Area_As_Customer()
                           .And.FileStorage_Will_CreateFile_With_Id(expectedFileId);
            var myService = CurrentTestCase.GetServiceTested();

            myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            CurrentTestCase.MockFileStorage.Verify(x => x.Create(It.IsAny<string>()), Times.Once);
            CurrentTestCase.MockFileStorage.Verify(x => x.Upload(It.IsAny<IFile>(), It.IsAny<Stream>()), Times.Once);
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Add_Document_To_Customer_When_Process_Is_Successful()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            
            const string md5Signature = "597FF3F1CA5EBFD58CD7598D363810AF";
            Guid expectedFileId = Guid.NewGuid();
            CurrentTestCase.Is.SessionService_Returns_No_Error()
                           .And.File_Extension_Is_Allowed_For_Name(documentFileName)
                           .And.User_Has_Access_To_The_Area_Of_The_Customer(customerNumber)
                           .And.Maximum_File_Size_Is_At_Least(documentSize)
                           .And.DocumentType_Has_Valid_Configuration_On_Same_Area_As_Customer()
                           .And.FileStorage_Will_CreateFile_With_Id(expectedFileId)
                           .And.Setup_Md5_Hash_For_Document(md5Signature);
            var myService = CurrentTestCase.GetServiceTested();

            myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            CurrentTestCase.Customer.Documents.Should().HaveCount(1);
            CustomerDocument addedDocument = CurrentTestCase.Customer.Documents.First();
            addedDocument.Customer.Should().Be(CurrentTestCase.Customer);
            addedDocument.FileId.Should().Be(expectedFileId);
            addedDocument.Date.Should().BeCloseTo(DateTime.UtcNow, 6000.Milliseconds());
            addedDocument.Md5Signature.Should().Be(md5Signature);
            CurrentTestCase.MockDbContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Set_Document_Accessibility_Based_On_Config_When_Process_Is_Successful()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            
            Guid expectedFileId = Guid.NewGuid();
            bool externalAccess = TestContext.CurrentContext.Random.NextBool();
            CurrentTestCase.Is.SessionService_Returns_No_Error()
                           .And.File_Extension_Is_Allowed_For_Name(documentFileName)
                           .And.User_Has_Access_To_The_Area_Of_The_Customer(customerNumber)
                           .And.Maximum_File_Size_Is_At_Least(documentSize)
                           .And.DocumentType_Has_Valid_Configuration_On_Same_Area_As_Customer()
                           .And.DocumentType_CP_Accessibility_Config_Is(externalAccess)
                           .And.FileStorage_Will_CreateFile_With_Id(expectedFileId);
            var myService = CurrentTestCase.GetServiceTested();

            myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            CurrentTestCase.Customer.Documents.Should().HaveCount(1);
            CustomerDocument addedDocument = CurrentTestCase.Customer.Documents.First();
            addedDocument.ExternalAccess.Should().Be(externalAccess);
            CurrentTestCase.MockDbContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Return_DocumentUrl_When_Proccess_Is_Successful()
        {
            const string documentFileName = "Invoice.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            
            Guid expectedFileId = Guid.NewGuid();
            CurrentTestCase.Is.SessionService_Returns_No_Error()
                           .And.File_Extension_Is_Allowed_For_Name(documentFileName)
                           .And.User_Has_Access_To_The_Area_Of_The_Customer(customerNumber)
                           .And.Maximum_File_Size_Is_At_Least(documentSize)
                           .And.DocumentType_Has_Valid_Configuration_On_Same_Area_As_Customer()
                           .And.FileStorage_Will_CreateFile_With_Id(expectedFileId);
            var myService = CurrentTestCase.GetServiceTested();

            var result = myService.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize],
                Guid.NewGuid());

            result.UrlDocumentFile.Should().Be($"/Files/{expectedFileId}");
        }

        [Test]
        public void UploadDocumentForCustomer_Should_Update_CustomerHistory_When_Process_Is_Successful()
        {
            const string documentFileName = "fake.pdf";
            const int documentSize = 20;
            const string customerNumber = "AbrahamLincoln_0000";
            const string userLogin = "utest";
            CurrentTestCase.Is.Normally_Successful_Case_For_Parameters(documentFileName, documentSize, customerNumber, null)
                           .And.User_Login_Is(userLogin);
            var serviceTested = CurrentTestCase.GetServiceTested();

            serviceTested.UploadDocumentForCustomer(customerNumber, documentFileName, new byte[documentSize], Guid.NewGuid());

            CurrentTestCase.MockCustomerHistory.Verify(m => m.UpdateCustomer(customerNumber, userLogin, CustomerHistoryEvent.CustomerUpdate), Times.Once);
        }

        private class TestCaseBuilder
        {
            private readonly Mock<IDbContext> _mockDbContext;
            private readonly Mock<IUserSessionService> _userSessionService;
            private readonly Mock<IDocumentsSettingService> _documentsSettingService;
            private readonly Mock<ICustomerFetchService> _customerFetchService;
            private readonly Mock<IFileStorageService> _fileStorageService;
            private readonly Mock<IDocumentTypeConfigService> _documentTypeConfigService;
            private readonly Mock<ICustomerHistoryService> _customerHistoryService;
            private readonly Mock<IDocumentGenericTypesService> _documentGenericTypeService;
            private readonly Mock<IHashCalculator> _md5HashCalculatorService;
            private readonly User _user;
            private Customer _customer;
            private DocumentTypeConfig _documentTypeConfig;
            private int InitialCountOfCustomerDocuments = 0;

            public Customer Customer => _customer;
            public Mock<ICustomerHistoryService> MockCustomerHistory => _customerHistoryService;
            public Mock<IDbContext> MockDbContext => _mockDbContext;
            public Mock<IFileStorageService> MockFileStorage => _fileStorageService;
            public DocumentTypeConfig DocumentTypeConfig => _documentTypeConfig;
            public TestCaseBuilder Is => this;
            public TestCaseBuilder And => this;
            public TestCaseBuilder But => this;

            public TestCaseBuilder()
            {
                _mockDbContext = new Mock<IDbContext>();
                _userSessionService = new Mock<IUserSessionService>();
                _documentsSettingService = new Mock<IDocumentsSettingService>();
                _customerFetchService = new Mock<ICustomerFetchService>();
                _fileStorageService = new Mock<IFileStorageService>();
                _documentTypeConfigService = new Mock<IDocumentTypeConfigService>();
                _customerHistoryService = new Mock<ICustomerHistoryService>();
                _documentGenericTypeService = new Mock<IDocumentGenericTypesService>();
                _md5HashCalculatorService = new Mock<IHashCalculator>();
                _user = new User();
            }

            public IMyService GetServiceTested()
            {
                return new MyService(_mockDbContext.Object, _userSessionService.Object,
                    _documentsSettingService.Object, _customerFetchService.Object,
                    _fileStorageService.Object, _documentTypeConfigService.Object,
                    _customerHistoryService.Object, _md5HashCalculatorService.Object);
            }

            public TestCaseBuilder Normally_Successful_Case_For_Parameters(string documentFileName, int documentSize, string customerCode, Guid? documentId)
            {
                SessionService_Returns_No_Error()
                    .And.File_Extension_Is_Allowed_For_Name(documentFileName)
                    .And.User_Has_Access_To_The_Area_Of_The_Customer(customerCode)
                    .And.Maximum_File_Size_Is_At_Least(documentSize)
                    .And.DocumentType_Has_Valid_Configuration_On_Same_Area_As_Customer()
                    .And.FileStorage_Will_CreateFile_With_Id(Guid.NewGuid());
                return this;
            }

            #region Success cases (Ands)

            public TestCaseBuilder SessionService_Returns_No_Error()
            {
                var sessionValid = MyServiceErrors.None;
                _user.Login = "TestUser";
                _userSessionService.Setup(m => m.CheckSession(It.IsAny<Guid>()))
                    .Returns((_user, sessionValid));
                return this;
            }

            public TestCaseBuilder File_Extension_Is_Allowed_For_Name(string documentFileName)
            {
                string extension = Path.GetExtension(documentFileName ?? "").Replace(".", "").ToLowerInvariant();
                Setup_SettingsService_Returns_Allowed_File_Extensions(new[] { extension });
                return this;
            }

            public void Setup_SettingsService_Returns_Allowed_File_Extensions(string[] extensions)
            {
                _documentsSettingService.Setup(m => m.GetAllowedFileExtensions()).Returns(extensions);
            }

            public TestCaseBuilder User_Has_Access_To_The_Area_Of_The_Customer(string customerNumber)
            {
                var areaIdAccessibleByCallingUser = Guid.NewGuid();
                User_Has_Access_To_AreaId(areaIdAccessibleByCallingUser);
                Customer_Is_Linked_To_AreaId(customerNumber, areaIdAccessibleByCallingUser);
                return this;
            }

            public TestCaseBuilder User_Has_Access_To_AreaId(Guid areaId)
            {
                _user.Areas = new List<Area>
                {
                    new Area { Id = areaId }
                };
                return this;
            }

            public TestCaseBuilder Customer_Is_Linked_To_AreaId(string customerNumber, Guid areaId)
            {
                Setup_CustomerService_Returns_Customer_For_Number(customerNumber);
                _customer.AreaId = areaId;
                return this;
            }

            public TestCaseBuilder Setup_CustomerService_Returns_Customer_For_Number(string customerNumber)
            {
                _customer = new Customer();
                if (customerNumber == null)
                    customerNumber = "AbrahamLincoln_0000";
                var lastIndexOfUnderscore = customerNumber.LastIndexOf("_", StringComparison.Ordinal);
                if (lastIndexOfUnderscore <= 0 || lastIndexOfUnderscore >= customerNumber.Length - 2) return null;
                _customer.CustomerCodeWithoutSig = customerNumber.Substring(0, lastIndexOfUnderscore);
                _customer.CustomerSig = customerNumber.Substring(lastIndexOfUnderscore + 1);
                _customerFetchService.Setup(m => m.GetCustomerByCustomerNumber(customerNumber)).Returns(_customer);

                List<Customer> customersWithoutSig = new List<Customer> { _customer, new Customer(), new Customer() };
                _customerFetchService.Setup(m => m.GetCustomersWithoutSigByCustomerNumber(customerNumber)).Returns(customersWithoutSig.AsQueryable());
                return this;
            }

            public TestCaseBuilder Maximum_File_Size_Is_At_Least(int maxFileSize)
            {
                _documentsSettingService.Setup(m => m.GetMaxFileSize()).Returns(maxFileSize);
                return this;
            }

            public TestCaseBuilder DocumentType_Has_Valid_Configuration_On_Same_Area_As_Customer()
            {
                _documentTypeConfig = new DocumentTypeConfig
                {
                    AreaId = _customer.AreaId,
                    ExternalAccess = true,
                    CanBeAddedAfterContractClosed = true,
                    ApiDocumentsUploadAllowed = true,
                };
                _documentTypeConfigService
                    .Setup(m => m.GetByArea(It.IsAny<Guid>()))
                    .Returns(_documentTypeConfig);
                return this;
            }

            public TestCaseBuilder DocumentType_CP_Accessibility_Config_Is(bool externalAccess)
            {
                _documentTypeConfig.ExternalAccess = externalAccess;
                return this;
            }

            public TestCaseBuilder FileStorage_Will_CreateFile_With_Id(Guid id)
            {
                IFile file = new InMemoryFile { Id = id };
                _fileStorageService
                    .Setup(m => m.Create(It.IsAny<string>()))
                    .Returns(file);
                return this;
            }

            public TestCaseBuilder User_Login_Is(string userLogin)
            {
                _user.Login = userLogin;
                return this;
            }

            #endregion

            #region Error cases (Buts)

            public TestCaseBuilder User_Session_Is_Not_Valid()
            {
                var sessionInvalid = MyServiceErrors.SessionIsNotValid;
                var antifogTest = new User
                {
                    Login = "AntifogTest"
                };
                _userSessionService.Setup(m => m.CheckSession(It.IsAny<Guid>()))
                    .Returns((antifogTest, sessionInvalid));
                return this;
            }

            public TestCaseBuilder User_Does_Not_Have_Access_To_The_Area_Of_The_Customer(string customerNumber)
            {
                var areaIdAccessibleByCallingUser = Guid.NewGuid();
                var otherAreaId = Guid.NewGuid();
                User_Has_Access_To_AreaId(areaIdAccessibleByCallingUser);
                Customer_Is_Linked_To_AreaId(customerNumber, otherAreaId);
                return this;
            }

            public TestCaseBuilder No_Customer_Exist_With_This_Number(string customerNumber)
            {
                _customerFetchService.Setup(m => m.GetCustomerByCustomerNumber(customerNumber)).Returns((Customer)null);
                return this;
            }

            public TestCaseBuilder Document_Size_Is_Bigger_Than_Maximum_File_Size(int documentSize)
            {
                _documentsSettingService.Setup(m => m.GetMaxFileSize()).Returns(documentSize - 1);
                return this;
            }

            public TestCaseBuilder No_File_Extension_Is_Allowed()
            {
                Setup_SettingsService_Returns_Allowed_File_Extensions(new string[] { });
                return this;
            }

            public TestCaseBuilder File_Extension_Is_Not_Allowed(string documentFileName)
            {
                string fileExtension = Path.GetExtension(documentFileName).Replace(".", "").ToLowerInvariant();
                Setup_SettingsService_Returns_Allowed_File_Extensions(new string[] { "not" + fileExtension });
                return this;
            }

            public TestCaseBuilder Documents_Are_Not_Allowed_After_Contract_Closing_And_Contract_Is_Closed()
            {
                _customer.ContractIsClosed = true;
                _documentTypeConfig.CanBeAddedAfterContractClosed = false;
                return this;
            }

            public TestCaseBuilder Customer_Already_Has_A_Document_Of_Type(Guid documentGenericTypeId, Guid documentId)
            {
                _customer.Documents.Add(new CustomerDocument
                {
                    Id = documentId,
                });
                InitialCountOfCustomerDocuments = 1;
                return this;
            }

            public TestCaseBuilder Upload_Is_Not_Allowed_On_API_For_This_Document_Type()
            {
                _documentTypeConfig.ApiDocumentsUploadAllowed = false;
                return this;
            }

            public TestCaseBuilder Assert_No_Upload_And_No_Save_Was_Done()
            {
                _fileStorageService.Verify(m => m.Upload(It.IsAny<IFile>(), It.IsAny<Stream>()), Times.Never);
                _customer.Documents.Should().HaveCount(InitialCountOfCustomerDocuments);
                _mockDbContext.Verify(m => m.SaveChanges(), Times.Never);
                return this;
            }

            public TestCaseBuilder Setup_Md5_Hash_For_Document(string md5Signature)
            {
                _md5HashCalculatorService.Setup(x => x.CalculateHash(It.IsAny<Stream>())).Returns(md5Signature);
                return this;
            }

            public TestCaseBuilder Customer_Already_Has_A_Document_With_Same_Md5Signature(string md5Signature)
            {
                Setup_Md5_Hash_For_Document(md5Signature);
                _customer.Documents.Add(new CustomerDocument
                {
                    Id = Guid.NewGuid(),
                    Md5Signature = md5Signature
                });
                InitialCountOfCustomerDocuments = 1;
                return this;
            }

            #endregion
        }
    }
}
