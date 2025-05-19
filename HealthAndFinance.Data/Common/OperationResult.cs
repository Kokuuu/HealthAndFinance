using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthAndFinance.Data.Common
{
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static OperationResult CreateSuccess(string message) => new()
        {
            IsSuccess = true,
            Message = message
        };

        public static OperationResult CreateFailure(string message) => new()
        {
            IsSuccess = false,
            Message = message
        };
    }
}
