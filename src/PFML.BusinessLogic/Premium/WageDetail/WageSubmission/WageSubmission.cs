using System;
using System.Collections.Generic;
using System.Linq;
using FACTS.Framework.DAL;
using FACTS.Framework.Service;
using PFML.DAL.Model.DbEntities;
using PFML.Shared.Model.DbDtos;
using DbContext = PFML.DAL.Model.DbContext;
using PFML.Shared.ViewModels.Revenue;
using PFML.Shared.LookupTable;
using PFML.Shared.ViewModels.Premium.WageDetail.WageSubmission;
using FACTS.Framework.Lookup;
using FACTS.Framework.Support;

namespace PFML.BusinessLogic.Premium.WageDetail
{

    public static class WageSubmission
    {
        /// <summary>
        /// To do 
        /// </summary>
        /// <returns></returns>
        [OperationMethod]
        public static WageSubmissionViewModel GetEmpWageDetails()
        {
            WageSubmissionViewModel wage = new WageSubmissionViewModel();

            return wage;
        }

        [OperationMethod]
        public static WageSubmissionViewModel GetWagePeriod()
        {
            WageSubmissionViewModel wage = new WageSubmissionViewModel();
            wage.Employer.EmployerId = 1;//To do
            wage.Employer.EntityName = "MARCON EXCAVATING INC";//To do
            for (int i = 1; i <= 25; i++)
            {
                WageUnitDetailDto wageUnitDetailDto = new WageUnitDetailDto();
                wageUnitDetailDto.EmployerId = wage.Employer.EmployerId;
                wage.ListWageUnitDetailDto.Add(wageUnitDetailDto);
            }

            return wage;
        }

        [OperationMethod]
        public static WageSubmissionViewModel ValidateSelectionMethod(WageSubmissionViewModel wageDetail)
        {
            FACTS.Framework.Lookup.LookupValue item = LookupUtil.GetLookupValue(LookupTable.WageDetailAdjReasonCode, LookupTable_WageDetailAdjReasonCode.Original, "Display");
            wageDetail.AdjReasonDisplay = item.Value;
            wageDetail.AdjReasonCode = LookupTable_WageDetailAdjReasonCode.Original;
            
            return wageDetail;
        }

        [OperationMethod]
        public static WageSubmissionViewModel ValidateManualEntrySubmission(WageSubmissionViewModel wageDetail)
        {
            int NumberofRecords = 0;
            //To do in Linq Query
            foreach (var wageRow in wageDetail.ListWageUnitDetailDto)
            {
                string Ssn = wageRow.Ssn;

                if (!string.IsNullOrWhiteSpace(Ssn))
                {
                    NumberofRecords++;
                }
            }
            wageDetail.ListWageEmployerUnitSummary = new List<WageSubmissionViewModel.WageDetailSummary>();
            
            Decimal grossWage = wageDetail.ListWageUnitDetailDto.Select(x => x.WageAmount).Sum();
            int month1 = wageDetail.ListWageUnitDetailDto.Where(x => x.IsEmploymentMonth1==true).Count();
            int month2 = wageDetail.ListWageUnitDetailDto.Where(x => x.IsEmploymentMonth2 == true).Count();
            int month3 = wageDetail.ListWageUnitDetailDto.Where(x => x.IsEmploymentMonth3 == true).Count();
           
            wageDetail.ListWageEmployerUnitSummary.Add(new WageSubmissionViewModel.WageDetailSummary { EmployerUnitNo = 1, EntityName = wageDetail.Employer.EntityName, NumberofRecords = NumberofRecords, GrossWage = grossWage, QtrMonth1RecordsCount = month1, QtrMonth2RecordsCount = month2, QtrMonth3RecordsCount = month3 });
            wageDetail.GrossWages = wageDetail.ListWageEmployerUnitSummary.Select(x => x.GrossWage).Sum();
            wageDetail.NumberofRecords = NumberofRecords;
            wageDetail.EmployerAccountTransactionDto.OwedAmount = Decimal.Multiply(wageDetail.GrossWages, (decimal)0.0040);
            wageDetail.EmployerAccountTransactionDto.UnpaidAmount = wageDetail.EmployerAccountTransactionDto.OwedAmount;
            return wageDetail;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wageDetail"></param>
        /// <returns></returns>
        [OperationMethod]
        
        public static WageSubmissionViewModel ValidateTax(WageSubmissionViewModel wageDetail)
        {
            return wageDetail;
        }


    }


}

