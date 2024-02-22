using AutoMapper;
using HCQS.BackEnd.Common.ConfigurationModel;
using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Models;
using HCQS.BackEnd.Service.Contracts;
using HCQS.BackEnd.Service.UtilityService.Payment.PaymentLibrary;
using HCQS.BackEnd.Service.UtilityService.Payment.PaymentRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Transactions;

namespace HCQS.BackEnd.Service.Implementations
{
    public class PaymentService : GenericBackendService, IPaymentService
    {
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IPaymentRepository _paymentRepository;
        private readonly IConfiguration _configuration;
        private MomoConfiguration _momoConfiguration;
        private VNPayConfiguration _vnPayConfiguration;

        public PaymentService(IConfiguration configuration, BackEndLogger backEndLogger, IUnitOfWork unitOfWork, IMapper mapper, IPaymentRepository paymentRepository, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _configuration = configuration;
            _logger = backEndLogger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentRepository = paymentRepository;
            _momoConfiguration = Resolve<MomoConfiguration>();
            _vnPayConfiguration = Resolve<VNPayConfiguration>();
        }

        public async Task<AppActionResult> CreatePaymentUrlMomo(Guid paymentId)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var accountRepository = Resolve<IAccountRepository>();
                var contractRepository = Resolve<IContractRepository>();
                var projectRepository = Resolve<IProjectRepository>();

                var contractProgressPaymentRepository = Resolve<IContractProgressPaymentRepository>();
                var paymentDb = await _paymentRepository.GetByExpression(s => s.Id == paymentId);
                var contractProgressPaymentDb = await contractProgressPaymentRepository.GetByExpression(c => c.PaymentId == paymentId);
                var contractId = contractProgressPaymentDb.ContractId;
                var contractDb = await contractRepository.GetById(contractId);
                var project = await projectRepository.GetById(contractDb.ProjectId);
                var accountDb = await accountRepository.GetById(project.AccountId);

                if (paymentDb == null || contractProgressPaymentDb == null || contractDb == null || accountDb == null)
                {
                    result = BuildAppActionResultError(result, $"The payment with id {paymentId} is not existed");
                }
                if (contractDb == null || contractDb.ContractStatus != Contract.Status.ACTIVE)
                {
                    result = BuildAppActionResultError(result, $"The contract with id {paymentId} is not existed or is not signed");

                }
                else if (paymentDb != null && (paymentDb.Price > 5000000 || paymentDb.Price < 1000))
                {
                    result = BuildAppActionResultError(result, $"Momo only supports amounts from 1000 to 5000000");
                }
                if (!BuildAppActionResultIsError(result))
                {
                    PaymentInformationRequest momo = new PaymentInformationRequest
                    {
                        AccountID = accountDb.Id,
                        Amount = paymentDb.Price,
                        CustomerName = $"{accountDb.FirstName} {accountDb.LastName}",
                        OrderID = paymentId.ToString()
                    };

                    string endpoint = _momoConfiguration.Api;
                    string partnerCode = _momoConfiguration.PartnerCode;
                    string accessKey = _momoConfiguration.AccessKey;
                    string secretkey = _momoConfiguration.Secretkey;
                    string orderInfo = $"Khach hang: {momo.CustomerName} thanh toan hoa don {momo.OrderID}";
                    string redirectUrl = $"{_momoConfiguration.RedirectUrl}/{momo.OrderID}";
                    string ipnUrl = _momoConfiguration.IPNUrl;
                    //  string ipnUrl = "https://webhook.site/3399b42a-eee3-4e2d-8925-c2f893737de9";

                    string requestType = "captureWallet";

                    string amount = momo.Amount.ToString();
                    string orderId = Guid.NewGuid().ToString();
                    string requestId = Guid.NewGuid().ToString();
                    string extraData = momo.OrderID.ToString();

                    //Before sign HMAC SHA256 signature
                    string rawHash = "accessKey=" + accessKey +
                        "&amount=" + amount +
                        "&extraData=" + extraData +
                        "&ipnUrl=" + ipnUrl +
                        "&orderId=" + orderId +
                        "&orderInfo=" + orderInfo +
                        "&partnerCode=" + partnerCode +
                        "&redirectUrl=" + redirectUrl +
                        "&requestId=" + requestId +
                        "&requestType=" + requestType
                        ;

                    MomoSecurity crypto = new MomoSecurity();
                    //sign signature SHA256
                    string signature = crypto.signSHA256(rawHash, secretkey);

                    //build body json request
                    JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Test" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "en" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }
                };

                    var client = new RestClient();

                    var request = new RestRequest(endpoint, Method.Post);
                    request.AddJsonBody(message.ToString());
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        JObject jmessage = JObject.Parse(response.Content);
                        result.Result.Data = jmessage.GetValue("payUrl").ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
            }

            return result;
        }

        public async Task<AppActionResult> CreatePaymentUrlVNPay(Guid paymentId, HttpContext context)
        {
            AppActionResult result = new AppActionResult();

            try
            {
                var accountRepository = Resolve<IAccountRepository>();
                var contractRepository = Resolve<IContractRepository>();
                var projectRepository = Resolve<IProjectRepository>();
                var contractProgressPaymentRepository = Resolve<IContractProgressPaymentRepository>();
                var paymentDb = await _paymentRepository.GetByExpression(s => s.Id == paymentId);
                var contractProgressPaymentDb = await contractProgressPaymentRepository.GetByExpression(c => c.PaymentId == paymentId);
                var contractId = contractProgressPaymentDb.ContractId;
                var contractDb = await contractRepository.GetById(contractId);
                var project = await projectRepository.GetById(contractDb.ProjectId);
                var accountDb = await accountRepository.GetById(project.AccountId);

                if (paymentDb == null || contractProgressPaymentDb == null || accountDb == null)
                {
                    result = BuildAppActionResultError(result, $"The payment with id {paymentId} is not existed");
                }
                if (contractDb == null || contractDb.ContractStatus != Contract.Status.ACTIVE)
                {
                    result = BuildAppActionResultError(result, $"The contract with id {paymentId} is not existed or is not signed");

                }
                if (!BuildAppActionResultIsError(result))
                {
                    PaymentInformationRequest momo = new PaymentInformationRequest
                    {
                        AccountID = accountDb.Id,
                        Amount = paymentDb.Price,
                        CustomerName = $"{accountDb.FirstName} {accountDb.LastName}",
                        OrderID = paymentId.ToString()
                    };
                    var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
                    var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
                    var pay = new VNPayLibrary();
                    var urlCallBack = $"{_vnPayConfiguration.ReturnUrl}/{paymentId}";

                    pay.AddRequestData("vnp_Version", _vnPayConfiguration.Version);
                    pay.AddRequestData("vnp_Command", _vnPayConfiguration.Command);
                    pay.AddRequestData("vnp_TmnCode", _vnPayConfiguration.TmnCode);
                    pay.AddRequestData("vnp_Amount", (paymentDb.Price * 100).ToString());
                    pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));

                    pay.AddRequestData("vnp_CurrCode", _vnPayConfiguration.CurrCode);
                    pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
                    pay.AddRequestData("vnp_Locale", _vnPayConfiguration.Locale);
                    pay.AddRequestData("vnp_OrderInfo", $"Khach hang: {accountDb.FirstName} {accountDb.LastName} thanh toan hoa don {paymentId}");
                    pay.AddRequestData("vnp_OrderType", "other");

                    pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
                    pay.AddRequestData("vnp_TxnRef", paymentId.ToString());

                    result.Result.Data = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
            }

            return result;
        }

        public async Task<AppActionResult> UpdatePaymentStatus(string paymentId, bool status, int type)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                AppActionResult result = new AppActionResult();
                var paymentResponseRepository = Resolve<IPaymentResponseRepository>();
                var paymentDb = await _paymentRepository.GetByExpression(p => p.Id == Guid.Parse(paymentId));
                try
                {
                    PaymentResponse paymentResponse = new PaymentResponse()
                    {
                        PaymentTypeResponse = type == 0 ? PaymentResponse.PaymentType.VNPAY : PaymentResponse.PaymentType.MOMO,
                        Amount = paymentDb.Price,
                        IsSuccess = status == true ? true : false,
                        OrderInfo = paymentDb.Content,
                        Id = Guid.NewGuid(),
                        PaymentId = Guid.Parse(paymentId)
                    };

                    if (status == true)
                    {
                        paymentDb.PaymentStatus = Payment.Status.Success;
                    }
                    await _paymentRepository.Update(paymentDb);
                    await paymentResponseRepository.Insert(paymentResponse);
                    await _unitOfWork.SaveChangeAsync();
                    if (!BuildAppActionResultIsError(result))
                    {
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    result = BuildAppActionResultError(result, ex.Message);
                    _logger.LogError(ex.Message, this);
                }
                return result;
            }
        }

        public async Task<AppActionResult> GetAllPayment(int pageIndex, int pageSize)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var list = await _paymentRepository.GetAllDataByExpression(null);
                result.Result.Data = DataPresentationHelper.ApplyPaging(list, pageIndex, pageSize);
                result.Result.TotalPage = DataPresentationHelper.CalculateTotalPageSize(list.Count(), pageSize);
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<AppActionResult> GetAllPaymentByContractId(Guid contractId)
        {
            AppActionResult result = new AppActionResult();
            try
            {
                var list = await _paymentRepository.GetAllDataByExpression(a => a.ContractProgressPayment.ContractId == contractId);
            }
            catch (Exception ex)
            {
                result = BuildAppActionResultError(result, ex.Message);
                _logger.LogError(ex.Message, this);
            }
            return result;
        }
    }
}