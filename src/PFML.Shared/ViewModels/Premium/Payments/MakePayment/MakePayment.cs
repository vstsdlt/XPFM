using PFML.Shared.Model.DbDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFML.Shared.ViewModels.Premium.Payments.MakePayment
{
    /// <summary>
    /// This class model beholds the values related to Employer Payment Details
    /// </summary>
    [Serializable]
    public class MakePaymentViewModel
    {
        public MakePaymentViewModel()
        {
            PaymentMainDto = new PaymentMainDto();
            PaymentProfileDto = new PaymentProfileDto();
            EmployerAccountTransactionDto = new EmployerAccountTransactionDto();
            EmployerDto = new EmployerDto();

        }
        public EmployerDto EmployerDto { get; set; }
        public PaymentMainDto PaymentMainDto { get; set; }
        public PaymentProfileDto PaymentProfileDto { get; set; }
        public EmployerAccountTransactionDto EmployerAccountTransactionDto { get; set; }
    }

    /// <summary>
    /// This class model contains the Employer Account Transaction Details
    /// </summary>
    [Serializable]
    public class EmployerAccountTransactionCustomDto
    {
        public int employeeID { get; set; }
        public EmployerAccountTransactionDto employerAccountTransactionCustDto { get; set; }
        public bool IsDeleted { get; set; }

        public EmployerAccountTransactionCustomDto()
        {
            employerAccountTransactionCustDto = new EmployerAccountTransactionDto();
        }
    }
}
