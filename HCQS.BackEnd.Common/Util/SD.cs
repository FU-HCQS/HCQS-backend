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
            public static List<string> SUPPLIER_QUOTATION_DETAIL = new List<string> { "No", "MaterialName", "Unit", "MQO", "Price" };
            public static List<string> EXPORT_PRICE_DETAIL = new List<string> { "No", "MaterialName", "Price", "Date" };
            public static List<string> IMPORT_INVENTORY = new List<string> { "No", "MaterialName", "SupplierName", "Quantity" };
        }
    }
}