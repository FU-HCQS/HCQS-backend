using HCQS.BackEnd.Common.Dto;

namespace HCQS.BackEnd.Service.Implementations
{
    public class GenericBackendService
    {
        private IServiceProvider _serviceProvider;

        public GenericBackendService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Resolve<T>()
        {
            return (T)_serviceProvider.GetService(typeof(T));
        }

        public AppActionResult BuildAppActionResultSuccess(AppActionResult result, string messageSuccess)
        {
            List<string?> messages = new List<string?> { messageSuccess };

            return new AppActionResult
            {
                IsSuccess = true,
                Messages = messages.Any() ? messages : null,
                Result = result.Result
            };
        }

        public AppActionResult BuildAppActionResultError(AppActionResult result, string messageError, bool exception = false)
        {
            List<string?> errors;
            if (exception)
            {
                errors = new List<string?>() { messageError };

                return new AppActionResult
                {
                    IsSuccess = false,
                    Messages = errors,
                    Result = null
                };
            }
            else
            {
                errors = new List<string?>(result.Messages) { messageError };
                return new AppActionResult
                {
                    IsSuccess = false,
                    Messages = errors.Any() ? errors : null,
                    Result = result.Result
                };
            }
        }

        public bool BuildAppActionResultIsError(AppActionResult result)
        {
            return !result.IsSuccess ? true : false;
        }
    }
}