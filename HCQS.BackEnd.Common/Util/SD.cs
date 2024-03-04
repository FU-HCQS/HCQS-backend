using System.Globalization;

namespace HCQS.BackEnd.Common.Util
{
    public class SD
    {
        public static int MAX_RECORD_PER_PAGE = short.MaxValue;

        public class ResponseMessage
        {
            public static string CREATE_SUCCESSFUL = "CREATE_SUCCESSFULLY";
            public static string UPDATE_SUCCESSFUL = "UPDATE_SUCCESSFULLY";
            public static string DELETE_SUCCESSFUL = "DELETE_SUCCESSFULLY";
            public static string CREATE_FAILED = "CREATE_FAILED";
            public static string UPDATE_FAILED = "UPDATE_FAILED";
            public static string DELETE_FAILED = "DELETE_FAILED";
            public static string LOGIN_FAILED = "LOGIN_FAILED";
        }

        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }

        public static IEnumerable<WeekForYear> PrintWeeksForYear(int year)
        {
            List<WeekForYear> weekForYears = new List<WeekForYear>();
            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = startDate.AddDays(6);

            CultureInfo cultureInfo = CultureInfo.CurrentCulture;

            Console.WriteLine($"Week 1: {startDate.ToString("d", cultureInfo)} - {endDate.ToString("d", cultureInfo)}");
            weekForYears.Add(new WeekForYear { WeekIndex = 1, Timeline = new() { StartDate = startDate.ToString("d", cultureInfo), EndDate = endDate.ToString("d", cultureInfo) } });

            for (int week = 2; week < 53; week++)
            {
                startDate = endDate.AddDays(1);
                endDate = startDate.AddDays(6);

                Console.WriteLine($"Week {week}: {startDate.ToString("d", cultureInfo)} - {endDate.ToString("d", cultureInfo)}");
                weekForYears.Add(new WeekForYear { WeekIndex = week, Timeline = new() { StartDate = startDate.ToString("d", cultureInfo), EndDate = endDate.ToString("d", cultureInfo) } });
            }
            return weekForYears;
        }

        public class SubjectMail
        {
            public static string VERIFY_ACCOUNT = "[THANK YOU] WELCOME TO LOVE HOUSE. PLEASE VERIFY ACCOUNT";
            public static string WELCOME = "[THANK YOU] WELCOME TO LOVE HOUSE";
            public static string REMIND_PAYMENT = "REMIND PAYMENT";
            public static string PASSCODE_FORGOT_PASSWORD = "[LOVE HOUSE] PASSCODE FORGOT PASSWORD";
            public static string SIGN_CONTRACT_VERIFICATION_CODE = "[LOVE HOUSE] You are in the process of completing contract procedures".ToUpper();
        }

        public class WeekForYear
        {
            public int WeekIndex { get; set; }
            public TimelineDto Timeline { get; set; }

            public class TimelineDto
            {
                public string StartDate { get; set; }
                public string EndDate { get; set; }
            }
        }

        public class FirebasePathName
        {
            public static string NEWS_PREFIX = "news/";
            public static string BLOG_PREFIX = "blog/";
            public static string SAMPLE_HOUSE_PREFIX = "sample-house/";
        }

        public class ExcelHeaders
        {
            public static List<string> SUPPLIER_QUOTATION_DETAIL = new List<string> { "No", "MaterialName", "Unit", "MOQ", "Price" };
            public static List<string> EXPORT_PRICE_DETAIL = new List<string> { "No", "MaterialName", "Price" };
            public static List<string> IMPORT_INVENTORY = new List<string> { "No", "MaterialName", "SupplierName", "Quantity" };
            public static List<string> SUPPLIER = new List<string> { "No", "SupplierName", "Type" };
        }

        public class EnumType
        {
            public static Dictionary<string, int> MaterialUnit = new Dictionary<string, int> { { "KG", 0 }, { "M3", 1 }, { "BAR", 2 }, { "ITEM", 3 }, { "Kg", 0 }, { "m3", 1 }, { "Bar", 2 }, { "Item", 3 }, { "kg", 0 }, { "bar", 2 }, { "item", 3 } };
            public static Dictionary<string, int> SupplierType = new Dictionary<string, int> { { "ConstructionMaterialsSupplier", 0 }, { "FurnitureSupplier", 1 }, { "Both", 2 }, { "constructionMaterialsSupplier", 0 }, { "furnitureSupplier", 1 }, { "both", 2 }, { "CONSTRUCTIONMATERIALSSUPPLIER", 0 }, { "FURNITURESUPPLIER", 1 }, { "BOTH", 2 } };
            public static Dictionary<string, int> MaterialType = new Dictionary<string, int> { { "RawMaterials", 0 }, { "Furniture", 1 }, { "rawMaterials", 0 }, { "furniture", 1 }, { "RAWMATERIALS", 0 }, { "FURNITURE", 1 } };
            public static Dictionary<Enum, string> ContractStatus = new Dictionary<Enum, string> { { DAL.Models.Contract.Status.NEW, "New" }, { DAL.Models.Contract.Status.ACTIVE, "Active" }, { DAL.Models.Contract.Status.IN_ACTIVE, "Inactive" } };
            public static Dictionary<Enum, string> ProjectStatus = new Dictionary<Enum, string> { { DAL.Models.Project.ProjectStatus.Pending, "Pending" }, { DAL.Models.Project.ProjectStatus.Processing, "Processing" }, { DAL.Models.Project.ProjectStatus.UnderConstruction, "UnderConstruction" }, { DAL.Models.Project.ProjectStatus.Closed, "Closed" } };
            public static Dictionary<Enum, string> ProjectConstructionType = new Dictionary<Enum, string> { { DAL.Models.Project.ProjectConstructionType.RoughConstruction, "Rough construction" }, { DAL.Models.Project.ProjectConstructionType.CompleteConstruction, "Complete construction" } };
        }
    }
}