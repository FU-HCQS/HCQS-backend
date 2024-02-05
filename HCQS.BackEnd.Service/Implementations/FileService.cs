using DinkToPdf.Contracts;
using Firebase.Auth;
using Firebase.Storage;
using HCQS.BackEnd.Common.ConfigurationModel;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Reflection;

namespace HCQS.BackEnd.Service.Implementations
{
    public class FileService : GenericBackendService, IFileService
    {
        private readonly IConverter _pdfConverter;
        private AppActionResult _result;
        private FirebaseConfiguration _firebaseConfiguration;

        public FileService(IConverter pdfConverter, IServiceProvider serviceProvider) : base(serviceProvider)
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
                bool isRecordTemplate = false;
                for (int i = 0; i < properties.Length; i++)
                {
                    if (sheetName.Contains("Template") && properties[i].Name.Equals("Id"))
                    {
                        worksheet.Cells[1, i + 1].Value = "No";
                        isRecordTemplate = true;
                    }
                    else worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

                int row = 2;

                if (isRecordTemplate)
                {
                    for (int i = row; i <= dataList.Count() + 1; i++)
                    {
                        worksheet.Cells[i, 1].Value = i - 1;
                    }
                }

                int j = isRecordTemplate ? 1 : 0;

                foreach (T item in dataList)
                {
                    for (; j < properties.Length; j++)
                    {
                        worksheet.Cells[row, j + 1].Value = properties[j].GetValue(item);
                    }
                    row++;
                }

                var excelBytes = package.GetAsByteArray();

                return new FileContentResult(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = sheetName + ".xlsx"
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

                    string destinationPath = $"{pathFileName}";

                    var task = new FirebaseStorage(
                    _firebaseConfiguration.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(destinationPath)
                    .PutAsync(stream);
                    var downloadUrl = await task;

                    if (task != null)
                    {
                        _result.Result.Data = downloadUrl;
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

        public async Task<AppActionResult> DeleteImageFromFirebase(string pathFileName)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(_firebaseConfiguration.ApiKey));

                var account = await auth.SignInWithEmailAndPasswordAsync(_firebaseConfiguration.AuthEmail, _firebaseConfiguration.AuthPassword);
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

        public IActionResult ReturnErrorColored<T>(List<string> headers, List<List<string>> data, Dictionary<int, string> rowsToColor, string filename)
        {
            if (headers == null || data == null || headers.Count == 0 || data.Count == 0)
            {
                Console.WriteLine("Invalid input data.");
                return null;
            }

            // Tạo một package Excel mới
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                // Tạo một Sheet trong Excel
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("DataSheet");

                // Ghi header vào sheet
                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Ghi dữ liệu vào sheet
                for (int i = 0; i < data.Count; i++)
                {
                    List<string> rowData = data[i];
                    int j = 0;
                    for (; j < rowData.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1].Value = rowData[j];
                    }
                    if (rowsToColor.ContainsKey(i + 2))
                    {
                        worksheet.Cells[i + 2, j + 1].Value = rowsToColor[i + 2];
                    }
                }

                if (rowsToColor != null)
                {
                    foreach (int i in rowsToColor.Keys)
                    {
                        worksheet.Rows[i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Rows[i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                    }
                }
                if(filename.Contains("(ErrorColor)"))
                    return new FileContentResult(excelPackage.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = filename
                    };
                return new FileContentResult(excelPackage.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"(ErrorColor){filename}.xlsx"
                };
            }
        }
    }
}