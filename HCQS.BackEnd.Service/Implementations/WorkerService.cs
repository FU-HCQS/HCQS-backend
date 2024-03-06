using AutoMapper;
using Hangfire;
using HCQS.BackEnd.Common.Util;
using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Implementations;
using HCQS.BackEnd.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Service.Implementations
{
    public class WorkerService:GenericBackendService
    {
        private BackEndLogger _logger;
        private IUnitOfWork _unitOfWork;
        private IContractService _contractService;
        private IContractProgressPaymentService _contractProgressPaymentService;
        private IMapper _mapper;
        public WorkerService(BackEndLogger logger, IUnitOfWork unitOfWork, IContractService contractService, IContractProgressPaymentService contractProgressPaymentService, IMapper mapper, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _contractService = contractService;
            _contractProgressPaymentService = contractProgressPaymentService;
            _mapper = mapper;
        }

        public async Task Start()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            RecurringJob.AddOrUpdate(() => _contractService.UpadateContractStatus(), Cron.DayInterval(1), vietnamTimeZone);
            RecurringJob.AddOrUpdate(() => _contractProgressPaymentService.SendPaymentRemindEmail(), Utility.ConvertToCronExpression(8, 30), vietnamTimeZone);
            //RecurringJob.AddOrUpdate(() => CheckSendEmail(), Cron.MinuteInterval(4), vietnamTimeZone);

        }

        public async Task CheckSendEmail()
        {
            try
            {
                 var emailService = Resolve<IEmailService>();
                 emailService.SendEmail("phamkhang12378@gmail.com",
                                "test ne",
                                "Hello");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, this);
            }
            Task.CompletedTask.Wait();
        }
    }

}
