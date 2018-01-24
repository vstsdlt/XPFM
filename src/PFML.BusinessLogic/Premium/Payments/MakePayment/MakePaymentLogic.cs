using FACTS.Framework.Service;
using PFML.Shared.ViewModels.Premium.Payments.MakePayment;
using DbContext = PFML.DAL.Model.DbContext;
using System.Linq;
using PFML.Shared.Model.DbDtos;

namespace PFML.BusinessLogic.Premium.MakePayment
{
    public static class MakePaymentLogic
    {
        /// <summary>
        /// This method will fetch the Employer details, payment details on the
        /// basis of Employer Account ID
        /// </summary>
        /// <param name="emprAccountID"></param>
        /// <returns></returns>
        [OperationMethod]
        public static MakePaymentViewModel GetEmployerDueAmount(int emprAccountID)
        {
            MakePaymentViewModel LocalPaymentViewModel = new MakePaymentViewModel();
            EmployerAccountTransactionDto localEmployerAccountTransaction = new EmployerAccountTransactionDto();
            EmployerDto localEmployerDto = new EmployerDto();
            PaymentMainDto localPaymentMainDto = new PaymentMainDto();
            PaymentProfileDto localPaymentProfileDto = new PaymentProfileDto();
            using (DbContext context = new DbContext())
            {
                
                localEmployerDto = (from employerDetail in context.Employers
                                    where employerDetail.EmployerId == emprAccountID
                                    select new EmployerDto
                                    {
                                        EmployerId = employerDetail.EmployerId,
                                        EntityName = employerDetail.EntityName,

                                    }).FirstOrDefault();
                if (localEmployerDto != null)
                {
                    localEmployerAccountTransaction = (from emptranDetail in context.EmployerAccountTransactions
                                                       where emptranDetail.EmployerId == localEmployerDto.EmployerId
                                                       select new EmployerAccountTransactionDto
                                                       {
                                                           OwedAmount = emptranDetail.OwedAmount,
                                                           UnpaidAmount = emptranDetail.UnpaidAmount
                                                       }).FirstOrDefault();

                    localPaymentMainDto = (from pmtMainDetail in context.PaymentMains
                                           where pmtMainDetail.EmployerId == localEmployerAccountTransaction.EmployerId
                                           select new PaymentMainDto
                                           {
                                               RoutingTransitNumber = pmtMainDetail.RoutingTransitNumber,
                                               BankAccountNumber = pmtMainDetail.BankAccountNumber,
                                               BankAccountTypeCode = pmtMainDetail.BankAccountTypeCode,
                                               PaymentAmount = pmtMainDetail.PaymentAmount,
                                               PaymentTransactionDate = pmtMainDetail.PaymentTransactionDate,
                                               PaymentMethodCode = pmtMainDetail.PaymentMethodCode,
                                               PaymentStatusCode = pmtMainDetail.PaymentStatusCode,
                                               PaymentMainId = pmtMainDetail.PaymentMainId
                                           }).FirstOrDefault();
                    localPaymentProfileDto = (from pmtProfile in context.PaymentProfiles
                                              where pmtProfile.EmployerId == localPaymentMainDto.EmployerId && pmtProfile.BankAccountNumber == localPaymentMainDto.BankAccountNumber
                                              select new PaymentProfileDto
                                              {
                                                  AgentId = pmtProfile.AgentId,
                                                  BankAccountNumber = pmtProfile.BankAccountNumber,
                                                  CreateDateTime = pmtProfile.CreateDateTime,
                                                  CreateUserId = pmtProfile.CreateUserId,
                                                  EmployerId = pmtProfile.EmployerId,
                                                  PaymentAccountTypeCode = pmtProfile.PaymentAccountTypeCode,
                                                  PaymentProfileId = pmtProfile.PaymentProfileId,
                                                  PaymentTypeCode = pmtProfile.PaymentTypeCode,
                                                  RoutingTransitNumber = pmtProfile.RoutingTransitNumber,
                                                  UpdateDateTime = pmtProfile.UpdateDateTime,
                                                  UpdateNumber = pmtProfile.UpdateNumber,
                                                  UpdateUserId = pmtProfile.UpdateUserId,
                                              }).FirstOrDefault();
                }
            }
            LocalPaymentViewModel.EmployerAccountTransactionDto = localEmployerAccountTransaction;
            LocalPaymentViewModel.EmployerDto = localEmployerDto;
            LocalPaymentViewModel.PaymentMainDto = localPaymentMainDto;
            LocalPaymentViewModel.PaymentProfileDto = localPaymentProfileDto;
            return LocalPaymentViewModel;
        }
    }
}
