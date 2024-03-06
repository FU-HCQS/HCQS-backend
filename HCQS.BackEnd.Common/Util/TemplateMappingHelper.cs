using HCQS.BackEnd.Common.Dto.Request;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.Common.Util
{
    public class TemplateMappingHelper
    {
        public static string GetTemplateContract(ContractTemplateDto dto)
        {
            var utility = new Utility();
            string body = @"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>HỢP ĐỒNG CUNG CẤP VẬT TƯ VÀ PHÍ XÂY DỰNG</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            line-height: 1.6;
        }
        .container {
            max-width: 800px;
            margin: 0 auto;
            padding: 20px;
        }
        h1, h2, h3 {
            color: #333;
            text-align: center;
        }
        p {
            margin-bottom: 15px;
        }
        .contract-section {
            margin-bottom: 30px;
        }
        .payment-schedule {
            margin-top: 20px;
        }
        .signature {
            margin-top: 50px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <h1>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</h1>
        <h3>Độc lập - Tự do – Hạnh Phúc</h3>
        <hr>

        <h2>HỢP ĐỒNG CUNG CẤP VẬT TƯ VÀ PHÍ XÂY DỰNG</h2>

        <p>Hôm nay, ngày " + dto.CreateDate.Day + @" tháng " + dto.CreateDate.Month + @" năm " + dto.CreateDate.Year + @" Tại  Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh.</p>

        <div class=""contract-section"">
            <h3>BÊN CUNG CẤP VẬT TƯ (Bên A)</h3>
            <p>Ông/bà/Công ty: Love House</p>
            <p>Địa chỉ: Tại  Lô E2a-7, Đường D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh.</p>
            <p>Điện thoại: 0366 967 957.</p>
        </div>

        <div class=""contract-section"">
            <h3>BÊN NHẬN CUNG CẤP VẬT TƯ VÀ PHÍ XÂY DỰNG (Bên B)</h3>
            <p>Ông/bà: " + $"{dto.Account.FirstName} {dto.Account.LastName}" + @"</p>
            <p>Địa chỉ: " + dto.Project.AddressProject + @" </p>
            <p>Điện thoại: " + dto.Account.PhoneNumber + @".</p>
        </div>

        <div class=""contract-section"">
            <p>Hai bên thỏa thuận ký hợp đồng này, trong đó, Bên A cam kết cung cấp vật tư và Bên B cam kết trả phí xây dựng với các điều khoản như sau:</p>
        </div>
        <div class=""contract-section"">
            <h3>Tổng chi phí dự tính thực hiện thi công:</h3>
            <ul>
                <li>Tổng số tiền vật liệu thô (xi măng, cát, thép...): " + $"{utility.FormatMoney(dto.Contract.MaterialPrice)} Bằng chữ: {utility.TranslateToVietnamese(dto.Contract.MaterialPrice)}" + @"</li>
                <li>Tổng số tiền vật liệu nội thất (bồn vệ sinh, đèn điện ...): " + $"{utility.FormatMoney(dto.Contract.FurniturePrice)} Bằng chữ: {utility.TranslateToVietnamese(dto.Contract.FurniturePrice)}" + @"</li>
                <li>Tổng số tiền nhân công: " + $"{utility.FormatMoney(dto.Contract.LaborPrice)} Bằng chữ: {utility.TranslateToVietnamese(dto.Contract.LaborPrice)}" + @"</li>
            </ul>
            <p>Tất cả số tiền trên là chi phí dự tính có thể sai lệch khoảng 10%. Nếu trong trường hợp sai lệch hơn thì bên A chịu trách nhiệm.</p>
        </div>
        <div class=""contract-section"">
            <h3>Điều 1: Nội dung cung cấp vật tư và phí xây dựng</h3>
            <p>Bên A cam kết cung cấp vật tư xây dựng cho Bên B, bao gồm:</p>
            <ul>
                <li>Vật liệu xây dựng như xi măng, cát, đá, thép,...</li>
                <li>Phụ kiện và thiết bị xây dựng như ống nước, ống điện, cửa, cầu thang,...</li>
                <li>Bất kỳ vật liệu hoặc phụ kiện nào khác được mô tả chi tiết trong bản hợp đồng.</li>
            </ul>
            <p>Bên B cam kết trả phí xây dựng theo đúng điều khoản và số liệu đã thỏa thuận trong hợp đồng.</p>
        </div>

        <div class=""contract-section"">
            <h3>Điều 2: Khoản thanh toán theo đợt</h3>
            <div class=""payment-schedule"">
";
            int i = 1;
            foreach (var progressPayment in dto.ContractProgressPayments)
            {
                body += "<p><strong>Đợt" + i + ":</strong> " + $"(Ngày {utility.FormatDate(progressPayment.Date)})" + " Trước khi bắt đầu công việc - Bên B thanh toán cho Bên A " + $"{utility.FormatMoney(progressPayment.Payment.Price)} Bằng chữ: {utility.TranslateToVietnamese(progressPayment.Payment.Price)}" + " đồng.</p> ";
                i++;
            };

            body += @"
            </div>
        </div>

        <div class=""contract-section"">
            <h3>Điều 3: Trách nhiệm của các bên</h3>
            <p><strong>Trách nhiệm của Bên A:</strong></p>
            <ul>
                <li>Cung cấp vật tư chất lượng và đúng số lượng như đã thỏa thuận.</li>
                <li>Giao hàng đúng thời hạn và địa điểm đã được thống nhất.</li>
                <li>Hỗ trợ và giải quyết mọi vấn đề xuất hiện trong quá trình cung cấp vật tư.</li>
            </ul>
            <p><strong>Trách nhiệm của Bên B:</strong></p>
            <ul>
                <li>Thanh toán đầy đủ và đúng hạn các khoản phí xây dựng và thanh toán theo đợt.</li>
                <li>Bảo quản và sử dụng vật tư một cách đúng đắn, không làm hỏng hoặc lãng phí.</li>
                <li>Thực hiện công việc xây dựng theo đúng thiết kế và tiến độ đã thống nhất.</li>
            </ul>
        </div>

        <div class=""contract-section"">
            <h3>Điều 4: Cam kết</h3>
            <p><strong>Cam kết của Bên A:</strong></p>
            <ul>
                <li>Cam kết cung cấp vật tư theo đúng chất lượng đã thỏa thuận.</li>
                <li>Cam kết hỗ trợ trong quá trình triển khai công việc xây dựng.</li>
                <li>Cam kết tuân thủ mọi điều khoản trong hợp đồng.</li>
            </ul>
            <p><strong>Cam kết của Bên B:</strong></p>
            <ul>
                <li>Cam kết thanh toán đầy đủ và đúng hạn theo đợt đã thỏa thuận.</li>
                <li>Cam kết sử dụng vật tư một cách có hiệu quả và không gây lãng phí.</li>
                <li>Cam kết bảo đảm chất lượng công trình xây dựng.</li>
            </ul>
        </div>

        <div class=""signature"">
            <div>
                <p>ĐẠI DIỆN BÊN A</p>
                <p>Ngày " + dto.CreateDate.Day + @" tháng " + dto.CreateDate.Month + @" năm " + dto.CreateDate.Year + @"</p>
<p> Đã kí </p>
  <p> Công ty Love House </p>
";

            body += @"

            </div>
            <div>
                <p>ĐẠI DIỆN BÊN B</p>
                 <p>Ngày " + dto.SignDate.Day + @" tháng " + dto.SignDate.Month + @" năm " + dto.SignDate.Year + @"</p>";
            if (dto.IsSigned)
            {
                body += "<p>Đã kí điện tử</p>";
            }
            else
            {
                body += "<p>Chưa kí</p>";
            }
            body += @" <p> " + $"{dto.Account.FirstName} {dto.Account.LastName}" + @" </p>

            </div>
        </div>
    </div>
</body>
</html>
";
            return body;
        }

        public enum ContentEmailType
        {
            VERIFICATION_CODE,
            FORGOTPASSWORD,
            CONTRACT_CODE
        }
        public static string GetTemplateOTPEmail(ContentEmailType type, string body, string name)
        {
            string content = "";
            switch (type)
            {
                case ContentEmailType.VERIFICATION_CODE:
                    content = @"
<html>
  <head>
    <style>
      * {
        margin: 0;
        padding: 0;
      }

      body {
        font-family: Arial, sans-serif;
        background-color: #f4f4f4; /* Background color for the entire email */
      }

      .container {
        max-width: 900px;
        margin: 20 auto;
        /* padding: 20px; */
        border-radius: 5px;
        box-shadow: 0px 0px 5px 2px #ccc; /*Add a shadow to the content */
      }

      .header {
        text-align: center;
        background-color: #ffba00; /* Header background color */
        padding: 20px;
      }
      .header-title {
        text-align: left;
        background-color: #2ad65e; /* Header background color */
        padding: 20px;
        color: white;
      }
      .title {
        color: black; /* Text color for the title */
        font-size: 30px;
        font-weight: bold;
      }

      .greeting {
        font-size: 18px;
        margin: 10 5;
      }
      .emailBody {
        margin: 5 5;
      }
      .support {
        font-size: 15px;
        font-style: italic;
        margin: 5 5;
      }

      .mainBody {
        background-color: #ffffff; /* Main content background color */
        padding: 20px;
        /* border-radius: 5px; */
        /* box-shadow: 0px 0px 5px 2px #ccc; Add a shadow to the content */
      }
      .body-content {
        /* display: flex;
        flex-direction: column; */
        border: 1px #fff8ea;
        border-radius: 5px;
        margin: 10 5;
        padding: 10px;
        /* background-color: #fff8ea; */
        box-shadow: 0px 0px 5px 2px #ccc;
      }
      .title-content {
        font-weight: bold;
      }

      u i {
        color: blue;
      }

      .footer {
        font-size: 14px;
        text-align: center;
        background-color: #ffba00; /* Header background color */
        padding: 10px;
        display: flex;
        justify-content: center;
        flex-direction: column;
      }
      .footer-text {
        font-weight: 600;
      }
      .signature {
        text-align: right;
        font-size: 16px;
        margin: 5 5;
      }
    </style>
  </head>
  <body>
    <div class=""container"">
      <div
        style=""
          height: 100px;
          display: flex;
          align-items: center;
          justify-content: center;
          background-color: white;
        ""
      >
        <p
          style=""
            color: #515151;
            text-align: center;
            margin: auto 0;
            font-size: 30px;
          ""
        >
          Love House
        </p>
      </div>
      <div class=""mainBody"">
        <!-- <div class=""header-title"">
        </div> -->
        <h2 class=""emailBody"">Hello " + name + @" ,</h2>
        <p class=""greeting""></p>

        <p class=""emailBody"">
          You are currently registering an account through <b><i>Love House </i></b>.
        </p>
        <p class=""emailBody"">
          Below is your OTP information:
          <b><i> " + body + @"</i></b>
        </p>

        <p class=""emailBody"">
          Please enter the code above into the system to proceed to the next step
          <a href=""https://lovehouse.vercel.app/""
            ><span style=""font-weight: bold; text-transform: uppercase""
              >here</span
            ></a
          >
        </p>
        <p class=""support"">
          Thank you for your interest in the services of <b><i>Love House</i></b
          >, for any inquiries, please contact
          <u><i>qk.backend@gmail.com</i></u> for support
        </p>
        <div class=""signature"">
          <p>Best regards,</p>
          <p>
            <b><i>Love House Team</i></b>
          </p>
        </div>
      </div>
      <div style=""height: 100px"">
        
      </div>
    </div>
  </body>
</html>

";
                    break;

                case ContentEmailType.CONTRACT_CODE:
                    content = @"
<html>
  <head>
    <style>
      * {
        margin: 0;
        padding: 0;
      }

      body {
        font-family: Arial, sans-serif;
        background-color: #f4f4f4; /* Background color for the entire email */
      }

      .container {
        max-width: 900px;
        margin: 20 auto;
        /* padding: 20px; */
        border-radius: 5px;
        box-shadow: 0px 0px 5px 2px #ccc; /*Add a shadow to the content */
      }

      .header {
        text-align: center;
        background-color: #ffba00; /* Header background color */
        padding: 20px;
      }
      .header-title {
        text-align: left;
        background-color: #2ad65e; /* Header background color */
        padding: 20px;
        color: white;
      }
      .title {
        color: black; /* Text color for the title */
        font-size: 30px;
        font-weight: bold;
      }

      .greeting {
        font-size: 18px;
        margin: 10 5;
      }
      .emailBody {
        margin: 5 5;
      }
      .support {
        font-size: 15px;
        font-style: italic;
        margin: 5 5;
      }

      .mainBody {
        background-color: #ffffff; /* Main content background color */
        padding: 20px;
        /* border-radius: 5px; */
        /* box-shadow: 0px 0px 5px 2px #ccc; Add a shadow to the content */
      }
      .body-content {
        /* display: flex;
        flex-direction: column; */
        border: 1px #fff8ea;
        border-radius: 5px;
        margin: 10 5;
        padding: 10px;
        /* background-color: #fff8ea; */
        box-shadow: 0px 0px 5px 2px #ccc;
      }
      .title-content {
        font-weight: bold;
      }

      u i {
        color: blue;
      }

      .footer {
        font-size: 14px;
        text-align: center;
        background-color: #ffba00; /* Header background color */
        padding: 10px;
        display: flex;
        justify-content: center;
        flex-direction: column;
      }
      .footer-text {
        font-weight: 600;
      }
      .signature {
        text-align: right;
        font-size: 16px;
        margin: 5 5;
      }
    </style>
  </head>
  <body>
    <div class=""container"">
      <div
        style=""
          height: 100px;
          display: flex;
          align-items: center;
          justify-content: center;
          background-color: white;
        ""
      >
        <p
          style=""
            color: #515151;
            text-align: center;
            margin: auto 0;
            font-size: 30px;
          ""
        >
          Love House
        </p>
      </div>
      <div class=""mainBody"">
        <!-- <div class=""header-title"">
        </div> -->
        <h2 class=""emailBody"">Hello " + name + @" ,</h2>
        <p class=""greeting""></p>

        <p class=""emailBody"">
          You are in the process of completing contract procedures through <b><i>Love House </i></b>.
        </p>
        <p class=""emailBody"">
          Below is your OTP information:
          <b><i> " + body + @"</i></b>
        </p>

        <p class=""emailBody"">
          Please enter the code above into the system to proceed to the next step
          <a href=""https://lovehouse.vercel.app/""
            ><span style=""font-weight: bold; text-transform: uppercase""
              >here</span
            ></a
          >
        </p>
        <p class=""support"">
          Thank you for your interest in the services of <b><i>Love House</i></b
          >, for any inquiries, please contact
          <u><i>qk.backend@gmail.com</i></u> for support
        </p>
        <div class=""signature"">
          <p>Best regards,</p>
          <p>
            <b><i>Love House Team</i></b>
          </p>
        </div>
      </div>
      <div style=""height: 100px"">
        
      </div>
    </div>
  </body>
</html>

";
                    break;
                case ContentEmailType.FORGOTPASSWORD:
                    content = @"
<html>
  <head>
    <style>
      * {
        margin: 0;
        padding: 0;
      }

      body {
        font-family: Arial, sans-serif;
        background-color: #f4f4f4; /* Background color for the entire email */
      }

      .container {
        max-width: 900px;
        margin: 20 auto;
        /* padding: 20px; */
        border-radius: 5px;
        box-shadow: 0px 0px 5px 2px #ccc; /*Add a shadow to the content */
      }

      .header {
        text-align: center;
        background-color: #ffba00; /* Header background color */
        padding: 20px;
      }
      .header-title {
        text-align: left;
        background-color: #2ad65e; /* Header background color */
        padding: 20px;
        color: white;
      }
      .title {
        color: black; /* Text color for the title */
        font-size: 30px;
        font-weight: bold;
      }

      .greeting {
        font-size: 18px;
        margin: 10 5;
      }
      .emailBody {
        margin: 5 5;
      }
      .support {
        font-size: 15px;
        font-style: italic;
        margin: 5 5;
      }

      .mainBody {
        background-color: #ffffff; /* Main content background color */
        padding: 20px;
        /* border-radius: 5px; */
        /* box-shadow: 0px 0px 5px 2px #ccc; Add a shadow to the content */
      }
      .body-content {
        /* display: flex;
        flex-direction: column; */
        border: 1px #fff8ea;
        border-radius: 5px;
        margin: 10 5;
        padding: 10px;
        /* background-color: #fff8ea; */
        box-shadow: 0px 0px 5px 2px #ccc;
      }
      .title-content {
        font-weight: bold;
      }

      u i {
        color: blue;
      }

      .footer {
        font-size: 14px;
        text-align: center;
        background-color: #ffba00; /* Header background color */
        padding: 10px;
        display: flex;
        justify-content: center;
        flex-direction: column;
      }
      .footer-text {
        font-weight: 600;
      }
      .signature {
        text-align: right;
        font-size: 16px;
        margin: 5 5;
      }
    </style>
  </head>
  <body>
    <div class=""container"">
      <div
        style=""
          height: 100px;
          display: flex;
          align-items: center;
          justify-content: center;
          background-color: white;
        ""
      >
        <p
          style=""
            color: #515151;
            text-align: center;
            margin: auto 0;
            font-size: 30px;
          ""
        >
          Love House
        </p>
      </div>
      <div class=""mainBody"">
        <!-- <div class=""header-title"">
        </div> -->
        <h2 class=""emailBody"">Hello " + name + @" ,</h2>
        <p class=""greeting""></p>

        <p class=""emailBody"">
         You have accidentally forgotten your password through <b><i>Love House </i></b>.
        </p>
        <p class=""emailBody"">
          Below is your OTP information:
          <b><i> " + body + @"</i></b>
        </p>

        <p class=""emailBody"">
          Please enter the code above into the system to proceed to the next step
          <a href=""https://lovehouse.vercel.app/""
            ><span style=""font-weight: bold; text-transform: uppercase""
              >here</span
            ></a
          >
        </p>
        <p class=""support"">
          Thank you for your interest in the services of <b><i>Love House</i></b
          >, for any inquiries, please contact
          <u><i>qk.backend@gmail.com</i></u> for support
        </p>
        <div class=""signature"">
          <p>Best regards,</p>
          <p>
            <b><i>Love House Team</i></b>
          </p>
        </div>
      </div>
      <div style=""height: 100px"">
        
      </div>
    </div>
  </body>
</html>

";
                    break;

            }

            return content;
        }

        public static string GetTemplatePaymentReminder(List<ContractProgressPayment> payment, string name)
        {
            string body = string.Empty;
            var utility = new Utility();
            body = @"
<html>
  <head>
    <style>
      * {
        margin: 0;
        padding: 0;
      }

      body {
        font-family: Arial, sans-serif;
        background-color: #f4f4f4; /* Background color for the entire email */
      }

      .container {
        max-width: 900px;
        margin: 20 auto;
        /* padding: 20px; */
        border-radius: 5px;
        box-shadow: 0px 0px 5px 2px #ccc; /*Add a shadow to the content */
      }

      .header {
        text-align: center;
        background-color: #ffba00; /* Header background color */
        padding: 20px;
      }
      .header-title {
        text-align: left;
        background-color: #2ad65e; /* Header background color */
        padding: 20px;
        color: white;
      }
      .title {
        color: black; /* Text color for the title */
        font-size: 30px;
        font-weight: bold;
      }

      .greeting {
        font-size: 18px;
        margin: 10 5;
      }
      .emailBody {
        margin: 5 5;
      }
      .support {
        font-size: 15px;
        font-style: italic;
        margin: 5 5;
      }

      .mainBody {
        background-color: #ffffff; /* Main content background color */
        padding: 20px;
        /* border-radius: 5px; */
        /* box-shadow: 0px 0px 5px 2px #ccc; Add a shadow to the content */
      }
      .body-content {
        /* display: flex;
        flex-direction: column; */
        border: 1px #fff8ea;
        border-radius: 5px;
        margin: 10 5;
        padding: 10px;
        /* background-color: #fff8ea; */
        box-shadow: 0px 0px 5px 2px #ccc;
      }
      .title-content {
        font-weight: bold;
      }

      u i {
        color: blue;
      }

      .footer {
        font-size: 14px;
        text-align: center;
        background-color: #ffba00; /* Header background color */
        padding: 10px;
        display: flex;
        justify-content: center;
        flex-direction: column;
      }
      .footer-text {
        font-weight: 600;
      }
      .signature {
        text-align: right;
        font-size: 16px;
        margin: 5 5;
      }
    </style>
  </head>
  <body>
    <div class=""container"">
      <div
        style=""
          height: 100px;
          display: flex;
          align-items: center;
          justify-content: center;
          background-color: white;
        ""
      >
        <p
          style=""
            color: #515151;
            text-align: center;
            margin: auto 0;
            font-size: 30px;
          ""
        >
          Love House
        </p>
      </div>
      <div class=""mainBody"">
        <!-- <div class=""header-title"">
        </div> -->
        <h2 class=""emailBody"">Hello " + name + @",</h2>
        <p class=""greeting""></p>

        <p class=""emailBody"">
          You currently have 1 unpaid invoice at <b><i>Love House </i></b>.
        </p>
        <p class=""emailBody"">Below is your information:</p>";
            foreach (ContractProgressPayment item in payment)
            {
                body += @"  <p class=""emailBody""><b>Contract no: </b>" + item.ContractId + @" </p>
        <p class=""""emailBody""""><b>Name: </b>" + item.Name + @" </p>
        <p class=""emailBody""><b>Amount that needs to be paid: </b>" + item.Payment.Price + @" </p>
        <p class=""emailBody"">You need to pay the bill within <b>" + $"{utility.GetCurrentDateTimeInTimeZone().Hour - item.Date.Hour} "+ @"hours</b></p>";
            }


            body += @"   <p class=""emailBody"">
          Please enter into the system to proceed to the next step
          <a href=""https://lovehouse.vercel.app/""
            ><span style=""font-weight: bold; text-transform: uppercase""
              >here</span
            ></a
          >
        </p>
        <p class=""support"">
          Thank you for your interest in the services of <b><i>Love House</i></b
          >, for any inquiries, please contact
          <u><i>qk.backend@gmail.com</i></u> for support
        </p>
        <div class=""signature"">
          <p>Best regards,</p>
          <p>
            <b><i>Love House Team</i></b>
          </p>
        </div>
      </div>
      <div style=""height: 100px""></div>
    </div>
  </body>
</html>

";
            return body;
        }
    }
}