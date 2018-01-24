using System;
using System.Collections.Generic;
using FACTS.Framework.Web;
using FACTS.Framework.Lookup;
using FACTS.Framework.Support;
using PFML.Shared.LookupTable;
using PFML.Shared.Model.DbDtos;
using PFML.Shared.Utility;
using static PFML.Shared.ViewModels.Premium.WageDetail.WageSubmission.WageSubmissionViewModel;

namespace PFML.Web.Controllers.Premium.WageDetail.WageSubmission
{
    public class WageSubmissionController : Controller
    {

        public void MachineExecute()
        {
            Machine["ListSection"] = LookupUtil.GetValues(LookupTable.WizEmployerWageFiling, null, "{DisplaySortOrder}", null);
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerWageFiling, LookupTable_WizEmployerWageFiling.SelectFilingMethod);

            Machine["ShowHeader"] = true;
            int yearsToGoBack= Convert.ToInt16(LookupTable_WageDetailUnitYears.Yearsforwagefiling);
            List<HtmlValueText> yearlist=DateUtil.PopulateYears(yearsToGoBack, 0, false);
            Machine["ReportingYearList"] = yearlist;

            //Set Default Values for Year and Quarter
            SetReportingYearAndQuarterDefaultValues();

          }


        public void DetailedSummaryExecute()
        {
           
        }
        public void DetailedSummaryEnd()
        {
           

        }
        public void ValidateManualEntrySubmissionEnd()
        {
           
        }

        public void DetailedSummaryNext()
        {
            
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerWageFiling, LookupTable_WizEmployerWageFiling.ProcessandCalculate);

        }

        public void DetailedSummaryPrevious()
        {
          
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerWageFiling, LookupTable_WizEmployerWageFiling.SubmitWageInformation);

        }

        public void ProcessAndCalculateTaxExecute()
        {
            Machine["ContributionRate"] = 0.4;
        }


        public void ProcessAndCalculateTaxSubmit()
        {
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerWageFiling,string.Empty);

        }

        public void ProcessAndCalculateTaxPrevious()
        {
           
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerWageFiling, LookupTable_WizEmployerWageFiling.ConfirmSubmission);

        }


        public void SelectFilingMethodNext()
        {
            int currentQtr = Convert.ToInt32(DateUtil.GetQuarter(DateTime.Now.Month).ToString().Remove(0, 1));
            DateTime subjDt = DateTime.Now;
            int selYr = Convert.ToInt32(Machine["WageUnitDetail.ReportingYear"]);
            int selQtr = Convert.ToInt32(Machine["WageUnitDetail.ReportingQuarter"].ToString().Remove(0, 1));

            bool isValidSubjDate = ((selYr > subjDt.Year) || (subjDt.Year == selYr && selQtr <= currentQtr));

            if (subjDt.Year <= selYr)
            {
                if (currentQtr < selQtr)
                {
                    Context.ValidationMessages.AddError("Submission not allowed for future quarter.");
                    return;
                }

            }

            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerWageFiling, LookupTable_WizEmployerWageFiling.SubmitWageInformation);

        }



        public void ManualEntryNext()
        {
           
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerWageFiling, LookupTable_WizEmployerWageFiling.ConfirmSubmission);

        }

        public void ManualEntryPrevious()
        {
            
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerWageFiling, LookupTable_WizEmployerWageFiling.SelectFilingMethod);

        }

        public void SetReportingYearAndQuarterDefaultValues()
        {
            Machine["WageFilingDefaultQuarter"] = DateUtil.GetQuarter(DateTime.Now.Month);
            Machine["WageFilingDefaultYear"] = Convert.ToString(DateTime.Now.Year);
        }
    }
}
