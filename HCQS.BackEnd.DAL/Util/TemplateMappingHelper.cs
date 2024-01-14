namespace HCQS.BackEnd.DAL.Util
{
    public class TemplateMappingHelper
    {
//        public string GetTemplateRemindSchedule(List<Schedule> schedules)
//        {
//            string body = @"
//<!DOCTYPE html>
//<html lang=""en"">
//<head>
//  <meta charset=""UTF-8"">
//  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
//  <style>
//    body {
//      font-family: Arial, sans-serif;
//      margin: 20px;
//    }

//    .schedule-container {
//      border: 1px solid #ccc;
//      margin-bottom: 20px;
//      padding: 10px;
//    }

//    .schedule-header {
//      font-size: 18px;
//      font-weight: bold;
//    }

//    .schedule-details {
//      margin-top: 10px;
//    }

//    .schedule-item {
//      margin-bottom: 10px;
//    }

//    .date-label {
//      font-weight: bold;
//    }
//  </style>
//  <title>Training Schedules</title>
//</head>
//<body>

//<h1>Training Schedules Today</h1>

//  <!-- Schedule 1 -->
//";
//            foreach (var item in schedules)
//            {
//                body += @"
//  <div class=""schedule-container"">
//    <div class=""schedule-header"">Schedule 1</div>
//    <div class=""schedule-details"">
//      <div class=""schedule-item"">
//        <span class=""date-label"">Date:</span> 2023-01-06
//      </div>
//      <div class=""schedule-item"">
//        <span class=""date-label"">Time:</span> 13H00 - 15H00
//      </div>
//      <div class=""schedule-item"">
//        <span class=""date-label"">Room:</span> A01
//      </div>
//    </div>
//  </div>
//";
//            }
//            body += @"

//</body>
//</html>
//";
//            return body;
//        }

//        public string GetTemplateRemindPayment(Subscription subscription, string urlPayment)
//        {
//            string body = @"
//<!DOCTYPE html>
//<html lang=""en"">
//<head>
//    <meta charset=""UTF-8"">
//    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
//    <title>Payment Reminder</title>
//    <style>
//        body {
//            font-family: Arial, sans-serif;
//            background-color: #f4f4f4;
//            margin: 0;
//            padding: 0;
//            display: flex;
//            justify-content: center;
//            align-items: center;
//            height: 100vh;
//        }

//        .reminder-container {
//            background-color: #fff;
//            border-radius: 8px;
//            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
//            padding: 20px;
//            max-width: 400px;
//            width: 100%;
//            text-align: center;
//        }

//        h1 {
//            color: #333;
//        }

//        p {
//            color: #666;
//        }

//        .reminder-button {
//            background-color: #007BFF;
//            color: #fff;
//            padding: 10px 20px;
//            border: none;
//            border-radius: 4px;
//            cursor: pointer;
//            font-size: 16px;
//            margin-top: 15px;
//        }
//    </style>
//</head>
//<body>
//    <div class=""reminder-container"">
//        <h1>Payment Reminder</h1>
//        <p>Your payment is due soon. Please make sure to submit your payment on time.</p>
//        <p>Class: " + subscription.Class.ClassName + @" </p>
//        <p>Total:" + subscription.Total + @" </p>
//        <a href="" " + urlPayment + @" "" class=""reminder-button"" style=""text-decoration: none;"">Pay Now</a>
//    </div>
//</body>
//</html>
//";
//            return body;
//        }
    }
}