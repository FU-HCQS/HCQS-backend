using DinkToPdf;
using DinkToPdf.Contracts;
using Firebase.Auth;
using Firebase.Storage;
using HCQS.BackEnd.Common.ConfigurationModel;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using RestSharp;
using System.Net;
using System.Reflection;

namespace HCQS.BackEnd.Service.Implementations
{
    public class FileService : GenericBackendService, IFileService
    {
        private readonly IConverter _pdfConverter;
        private AppActionResult _result;
        private FirebaseConfiguration _firebaseConfiguration;

        public FileService(IConverter pdfConverter,  IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _pdfConverter = pdfConverter;
            _result = new();
            _firebaseConfiguration = Resolve<FirebaseConfiguration>();
        }

      

        public IActionResult ConvertDataToExcel()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells["A1"].Value = "Ticket name";
                worksheet.Cells["B1"].Value = "Ticket issue";

                byte[] excelBytes = package.GetAsByteArray();

                return new FileContentResult(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "template.xlsx"
                };
            }
        }

        public ActionResult<List<List<string>>> UploadExcel(IFormFile file)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;

                        List<List<string>> data = new List<List<string>>();

                        for (int row = 1; row <= rowCount; row++)
                        {
                            List<string> rowData = new List<string>();
                            for (int col = 1; col <= colCount; col++)
                            {
                                object cellValue = worksheet.Cells[row, col].Value;
                                rowData.Add(cellValue != null ? cellValue.ToString() : string.Empty);
                            }
                            data.Add(rowData);
                        }

                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public IActionResult GenerateExcelContent<T>(IEnumerable<T> dataList, string sheetName)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);

                PropertyInfo[] properties = typeof(T).GetProperties();

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

                int row = 2;
                foreach (T item in dataList)
                {
                    for (int j = 0; j < properties.Length; j++)
                    {
                        worksheet.Cells[row, j + 1].Value = properties[j].GetValue(item);
                    }
                    row++;
                }

                var excelBytes = package.GetAsByteArray();

                return new FileContentResult(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "template.xlsx"
                };
            }
        }

        public IActionResult GenerateTemplateExcel<T>(T dataList)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(nameof(T));

                PropertyInfo[] properties = typeof(T).GetProperties();

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

                string a = typeof(T).ToString();
                string[] name = a.Split(".");
                var excelBytes = package.GetAsByteArray();
                var nameExcel = name[name.Length - 1];
                if (nameExcel.Contains("Dto"))
                {
                    nameExcel = nameExcel.Substring(0, nameExcel.Length - 3);
                }
                return new FileContentResult(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"TemplateImport{nameExcel}.xlsx"
                };
            }
        }

        public async Task<AppActionResult> UploadImageToFirebase(IFormFile file, string pathFileName)
        {
            bool isValid = true;
            if (file == null || file.Length == 0)
            {
                isValid = false;
                _result.Messages.Add("The file is empty");
            }
            if (isValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var stream = new MemoryStream(memoryStream.ToArray());
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseConfiguration.ApiKey));

                    var account = await auth.SignInWithEmailAndPasswordAsync(_firebaseConfiguration.AuthEmail, _firebaseConfiguration.AuthPassword);
                    var cancellation = new CancellationTokenSource();

                    string destinationPath = $"{pathFileName}";

                    var task = new FirebaseStorage(
                    _firebaseConfiguration.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(destinationPath)
                    .PutAsync(stream, cancellation.Token);
                    if (task != null)
                    {
                        _result.Result.Data = await GetUrlImageFromFirebase(pathFileName);
                    }
                    else
                    {
                        _result.IsSuccess = false;
                        _result.Messages.Add("Upload failed");
                    }
                }
            }
            return _result;
        }

        public async Task<string> GetUrlImageFromFirebase(string pathFileName)
        {
            string[] a = pathFileName.Split("/");
            pathFileName = $"{a[0]}%2F{a[1]}";
            string api = $"https://firebasestorage.googleapis.com/v0/b/yogacenter-44949.appspot.com/o?name={pathFileName}";
            if (string.IsNullOrEmpty(pathFileName))
            {
                return string.Empty;
            }
            else
            {
                var client = new RestClient();
                var request = new RestRequest(api, Method.Get);
                RestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JObject jmessage = JObject.Parse(response.Content);
                    string downloadToken = jmessage.GetValue("downloadTokens").ToString();
                    return $"https://firebasestorage.googleapis.com/v0/b/{_firebaseConfiguration.Bucket}/o/{pathFileName}?alt=media&token={downloadToken}";
                }
            }

            return string.Empty;
        }

        public async Task<AppActionResult> DeleteImageFromFirebase(string pathFileName)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseConfiguration.ApiKey));

                var account = await auth.SignInWithEmailAndPasswordAsync(_firebaseConfiguration.AuthEmail, _firebaseConfiguration.AuthPassword);
                var cancellation = new CancellationTokenSource();

                var storage = new FirebaseStorage(
             _firebaseConfiguration.Bucket,
             new FirebaseStorageOptions
             {
                 AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                 ThrowOnCancel = true
             });
                await storage
                    .Child(pathFileName)
                    .DeleteAsync();
                _result.Messages.Add("Delete image successful");
            }
            catch (FirebaseStorageException ex)
            {
                _result.Messages.Add($"Error deleting image: {ex.Message}");
            }
            return _result;
        }
    }
}