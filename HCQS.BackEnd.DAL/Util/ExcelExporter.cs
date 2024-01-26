using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace HCQS.BackEnd.DAL.Util
{
    public class ExcelExporter
    {
         


        public static void ExportToExcel(List<string> headers, List<List<string>> data, IEnumerable<int> rowsToColor,string filePath)
        {
            if (headers == null || data == null || headers.Count == 0 || data.Count == 0)
            {
                Console.WriteLine("Invalid input data.");
                return;
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
                    for (int j = 0; j < rowData.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1].Value = rowData[j];
                    }
                }

               if(rowsToColor != null)
                {
                    foreach(int i in rowsToColor)
                    {
                        worksheet.Rows[i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Rows[i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                    }
                }

                // Lưu file Excel
                FileInfo excelFile = new FileInfo(filePath);
                excelPackage.SaveAs(excelFile);

                Console.WriteLine($"Export successful. File saved at: {filePath}");
            }
        }

       public static string GetDownloadsPath(string fileName)
        {
            string downloadsPath = null;

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders"))
                {
                    if (key != null)
                    {
                        // Retrieve the Downloads path from the registry
                        downloadsPath = key.GetValue("{374DE290-123F-4565-9164-39C4925E467B}") as string;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return Path.Combine(downloadsPath, "(Error colored)" + fileName);
        }
    }
}