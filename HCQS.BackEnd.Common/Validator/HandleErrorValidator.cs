using FluentValidation.Results;
using HCQS.BackEnd.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.Common.Validator
{
    public class HandleErrorValidator
    {
        public AppActionResult HandleError(ValidationResult result)
        {
            if (!result.IsValid) {
                List<string> errorMessage = new List<string>();
                foreach(var error in result.Errors)
                {
                    errorMessage.Add(error.ErrorMessage);
                }
                return new AppActionResult
                {
                    IsSuccess = false,
                    Messages = errorMessage,
                    Result = null
                };
            }
            else
            {
                return new AppActionResult();
            }
        }
    }
}
