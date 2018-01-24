using FACTS.Framework.Lookup;
using FACTS.Framework.Web;
using PFML.Shared.LookupTable;
using System;
using System.Collections.Generic;

namespace PFML.Web.Controllers.Premium.Payments
{
    /// <summary>
    /// Make Payment Contoller Method
    /// </summary>
    public class MakePaymentController : Controller
    {
        public void MachineExecute()
        {
            //TO DO
            //Get Values from the DB Context
            Machine["emprAccountID"] = 100101;
            Machine["emprName"] = "Test Employer";
            Machine["dbaName"] = "Test DBA";

            Machine["ListSection"] = LookupUtil.GetValues(LookupTable.MakePaymentWizardFlow, null, "{DisplaySortOrder}", null);
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.MakePaymentWizardFlow, LookupTable_MakePaymentWizardFlow.SelectPaymentMethod);
            Machine["ShowHeader"] = true;
        }

        public void SelectPaymentMethodStart()
        {
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.MakePaymentWizardFlow, LookupTable_MakePaymentWizardFlow.SelectPaymentMethod);
        }

        public void SelectPaymentMethodExecute()
        {
            //TO DO
            //Get Values from the DB Context
            decimal amountDue = 987;
            Machine["PaymentDueDates"] = "<b>Quarter 1 - April 30<br/>Quarter 2 - July 31<br/>Quarter 3 - October 31<br/>Quarter 4 - January 31</b>";
            Machine["PrepaymentDueDates"] = "<b>10 calendar days after the start of the quarter</b>";


            Machine["ActualDueDate"] = DateTime.Today;
            Machine["AmountDue"] = amountDue;
            Machine["TotalAmount"] = amountDue;
            Machine["PmtAmount"] = amountDue;
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.MakePaymentWizardFlow, LookupTable_MakePaymentWizardFlow.SelectPaymentMethod);
            Machine["CheckPayment"] = Machine["SearchType"];
        }

        public void SelectPaymentMethodNext()
        {
            decimal pmtAmount = Convert.ToDecimal(Machine["PmtAmount"]);
            decimal totalAmountDue = Convert.ToDecimal(Machine["TotalAmount"]);
            string pmtMethod = Machine["CheckPaymentMethodType"].ToString();
            string OnlinePaymentMthod = LookupUtil.GetLookupCode(LookupTable.PaymentMethodType, LookupTable_PaymentMethodType.ACHDebit).Code;

            Machine["CheckPayment"] = ((totalAmountDue - pmtAmount) > 0) ? "Partial" : "Full";
            Machine["CheckPaymentMethodType"] = (pmtMethod.Equals(OnlinePaymentMthod, StringComparison.InvariantCultureIgnoreCase)) ? "Online" : "Paper";
        }

        public void OnlinePaymentStart()
        {
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.MakePaymentWizardFlow, LookupTable_MakePaymentWizardFlow.SubmitPaymentDetails);
        }

        public void PaymentDetailsExecute()
        {
            List<AccountType> listAccountType = new List<AccountType>();
            AccountType actype = new AccountType
            {
                AccountTypeName = "Savings"
            };
            listAccountType.Add(actype);
            actype.AccountTypeName = "Checking";
            listAccountType.Add(actype);
            //TO DO
            /*  If Employer does not have current period (based on Employer Payment Method: Contributory or Reimbursable) debt and effective date is greater than current date, 
             *  System shall not allow a payment to be scheduled for a future date and display Standard Message “You cannot post date this payment as you owe prior debt that is past due”*/
            Machine["PymtEffectiveDt"] = DateTime.Now;
            Machine["AccountTypeDDL"] = string.Empty;

        }
        public void PaymentDetailsNext()
        {
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.MakePaymentWizardFlow, LookupTable_MakePaymentWizardFlow.SubmitPaymentDetails);
            if (Machine["SaveBankInformation"] != null && Machine["SaveBankInformation"].ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                Machine["SaveBankInformationText"] = "Yes";
            }
            else
            {
                Machine["SaveBankInformationText"] = "No";
            }
            Machine["PaymentEffectiveDate"] = DateTime.Today;
            Machine["AccountType"] = Machine["AccountTypeDDL"].ToString().Equals(LookupTable_BankAccountType.Savings, StringComparison.InvariantCultureIgnoreCase) ? "Savings" : "Checking";
            //TO DO
            //Get Values from the DB Context
            Machine["PmtVerificationMsg"] = String.Format("By paying your New Mexico Department of Workforce Solutions bill by way of this online service, you are authorizing NMDWS to charge your {0} account for the amount you submitted.", Machine["AccountType"]);
        }

        public void PaymentConfirmationExecute()
        {
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.MakePaymentWizardFlow, LookupTable_MakePaymentWizardFlow.Complete);
            decimal pmtAmount = Convert.ToDecimal(Machine["PmtAmount"]);
            decimal totalAmountDue = Convert.ToDecimal(Machine["TotalAmount"]);
            Machine["BalanceAmount"] = (totalAmountDue - pmtAmount);
        }
        public void PaperCheckVoucherExecute()
        {
            //TO DO
            //Get Values from the DB Context
            Machine["PmtAmountPaperVoucher"] = "XXX,XXX.XX";
            Machine["emprAccountIDPaperVoucher"] = "XXXXXXX (must be written on check)";
            Machine["ChecksPayableAt"] = "NM Department of Workforce Solutions";
            Machine["MailingAddress"] = "PO BOX 2281 AlbuQuerque, NM 87103";
        }
    }

    /// <summary>
    /// Get Account Type
    /// </summary>
    [Serializable]
    public class AccountType
    {
        public string AccountTypeName { get; set; }
    }
}
