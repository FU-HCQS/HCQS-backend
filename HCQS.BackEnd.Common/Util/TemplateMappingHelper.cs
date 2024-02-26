using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.Common.Util
{
    public class TemplateMappingHelper
    {
        public static string GetTemplateContract(DateTime createDate, DateTime signDate, Account account, List<ContractProgressPayment> payments, bool isSign)
        {
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

        <p>Hôm nay, ngày " + createDate.Day + @" tháng " + createDate.Month + @" năm " + createDate.Year + @" Tại  Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh.</p>

        <div class=""contract-section"">
            <h3>BÊN CUNG CẤP VẬT TƯ (Bên A)</h3>
            <p>Ông/bà/Công ty: Love House</p>
            <p>Địa chỉ: Tại  Lô E2a-7, Đường D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh.</p>
            <p>Điện thoại: 0366 967 957.</p>
        </div>

        <div class=""contract-section"">
            <h3>BÊN NHẬN CUNG CẤP VẬT TƯ VÀ PHÍ XÂY DỰNG (Bên B)</h3>
            <p>Ông/bà: " + $"{account.FirstName} {account.LastName}" + @"</p>
            <p>Địa chỉ: …………………………………………………………………………………</p>
            <p>Điện thoại: " + account.PhoneNumber + @".</p>
        </div>

        <div class=""contract-section"">
            <p>Hai bên thỏa thuận ký hợp đồng này, trong đó, Bên A cam kết cung cấp vật tư và Bên B cam kết trả phí xây dựng với các điều khoản như sau:</p>
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
            foreach (var payment in payments)
            {
                body += "<p><strong>Đợt" + i + ":</strong> Trước khi bắt đầu công việc - Bên B thanh toán cho Bên A " + payment.Payment.Price + " đồng.</p> ";
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
                <p>Ngày " + createDate.Day + @" tháng " + createDate.Month + @" năm " + createDate.Year + @"</p>
<p> Đã kí </p>
  <p> Công ty Love House </p>
";


            body += @"
                
               
            </div>
            <div>
                <p>ĐẠI DIỆN BÊN B</p>
                 <p>Ngày " + signDate.Day + @" tháng " + signDate.Month + @" năm " + signDate.Year + @"</p>";
            if (isSign)
            {
                body += "<p>Đã kí điện tử</p>";
            }
            else
            {
                body += "<p>Empty !!!!!!!!</p>";
            }
            body += @" <p> " + $"{account.FirstName} {account.LastName}" + @" </p>
               
            </div>
        </div>
    </div>
</body>
</html>
";
            return body;
        }
    }
}