using System;
using System.Collections.Generic;
using FACTS.Framework.Web;
using FACTS.Framework.Lookup;
using FACTS.Framework.Support;
using PFML.Shared.LookupTable;
using PFML.Shared.Model.DbDtos;
using PFML.Shared.ViewModels.Premium.Registration;
using PFML.Shared.Utility;

namespace PFML.Web.Controllers.Premium.Registration
{

    public class EmployerRegistrationController : Controller
    {

        public void MachineExecute()
        {
            //used to set wizard header
            Machine["ListSection"] = LookupUtil.GetValues(LookupTable.WizEmployerRegistration, null, "{DisplaySortOrder}", null);
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerRegistration, LookupTable_WizEmployerRegistration.EnterLiabilityInformation);
            Machine["ShowHeader"] = false;

            //initialize EmployerRegistration object and required address objects
            Machine["EntityAddress"] = new AddressLinkDto() { Address = new AddressDto() { StateCode = LookupTable_State.Washington, CountryCode = LookupTable_Country.UnitedStates } };
            Machine["PhysicalAddress"] = new AddressLinkDto() { Address = new AddressDto() { StateCode = LookupTable_State.Washington, CountryCode = LookupTable_Country.UnitedStates } };
            Machine["BusinessAddress"] = new AddressLinkDto() { Address = new AddressDto() { StateCode = LookupTable_State.Washington, CountryCode = LookupTable_Country.UnitedStates } };
            Machine["EmployerRegistration"] = new EmployerRegistrationViewModel() { ListAddressLinkDto =new List<AddressLinkDto> { (AddressLinkDto)Machine["EntityAddress"], (AddressLinkDto)Machine["PhysicalAddress"], (AddressLinkDto)Machine["BusinessAddress"] } };
            Machine["YearList"] = DateUtil.PopulateYears(Convert.ToInt16(LookupTable_WageDetailUnitYears.Yearsforwagefiling), 0, false);
        }

        #region Wizard Navigation methods
        public void ValidateIntroductionNext()
        {
            Machine["ShowHeader"] = true;
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerRegistration, LookupTable_WizEmployerRegistration.EnterLiabilityInformation);
        }

        public void LiabilityWagesPrevious()
        {
            Machine["ShowHeader"] = false;
        }

        public void ValidateLiabilityWeeksNext()
        {
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerRegistration, LookupTable_WizEmployerRegistration.EnterUsers);
        }

        public void AdminInfoPrevious()
        {
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerRegistration, LookupTable_WizEmployerRegistration.EnterLiabilityInformation);
        }

        public void ValidateAdminInfoNext()
        {
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerRegistration, LookupTable_WizEmployerRegistration.EnterEmployerInformation);
        }

        public void EmpIdentificationPrevious()
        {
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerRegistration, LookupTable_WizEmployerRegistration.EnterUsers);
        }

        public void ValidateEmpIdentificationNext()
        {
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerRegistration, LookupTable_WizEmployerRegistration.EnterBusinessInformation);
        }

        public void EnterBusinessInfoPrevious()
        {
            Machine["IsPublicEntity"] = false;
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerRegistration, LookupTable_WizEmployerRegistration.EnterEmployerInformation);
        }

        public void ValidateEnterBusiRcdsAddrNext()
        {
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerRegistration, LookupTable_WizEmployerRegistration.Summary);
        }

        public void RegistrationSummaryPrevious()
        {
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerRegistration, LookupTable_WizEmployerRegistration.EnterBusinessInformation);
        }

        public void SubmitRegistrationNext()
        {
            Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerRegistration, LookupTable_WizEmployerRegistration.Complete);
        }

        #endregion

        #region UI Validations
        public void IntroductionNext()
        {
            if ((bool?)Machine["EmployerRegistration.EmployerDto.IsServiceBegin"] == false)
            {
                Context.ValidationMessages.AddError("You are not required to register at this time because you do not have employment in this state. Return and register once employment begins.");
            }
            if ((bool)Machine["EmployerRegistration.EmployerDto.IsServiceBegin"] && ((DateTime)Machine["EmployerRegistration.EmployerDto.ServiceBeginDate"] > ((DateTime)Machine["EmployerRegistration.EmployerUnitDto.FirstWageDate"])))
            {
                Context.ValidationMessages.AddError("'If yes, enter the date this employer first paid wages to those individuals performing services in State' must be equal to or after 'If yes, enter the date services were first performed for the employer in State'");
            }
        }
        public void LiabilityWagesNext()
        {
            List<bool> listUserInput = new List<bool> { (bool)Machine["EmployerRegistration.EmployerDto.EmployerLiability.HasPaid450RegularWages"], (bool)Machine["EmployerRegistration.EmployerDto.EmployerLiability.HasPaid1KDomesticWages"], (bool)Machine["EmployerRegistration.EmployerDto.EmployerLiability.HasPaid20KAgriculturalLaborWages"] };
            if (listUserInput.FindAll(l => l.Equals(true))?.Count >= 2)
            {
                Context.ValidationMessages.AddError("Only one answer can be Yes. Select the question that occurred first as Yes and the other(s) as No.");
                return;
            }
            else if (listUserInput.FindAll(l => l.Equals(true))?.Count >= 1)
            {
                Machine["IsLiableWages"] = true;
                Machine["CurrentSection"] = LookupUtil.GetLookupCode(LookupTable.WizEmployerRegistration, LookupTable_WizEmployerRegistration.EnterUsers);
            }
            else
            {
                Machine["IsLiableWages"] = false;
                Machine["EmployerRegistration.EmployerDto.EmployerLiability.LiabilityAmountMetYear"] = null;
                Machine["EmployerRegistration.EmployerDto.EmployerLiability.LiabilityAmountMetQuarter"] = null;
            }
        }
        public void LiabilityWeeksNext()
        {
            List<bool> listUserInput = new List<bool> { (bool)Machine["EmployerRegistration.EmployerDto.EmployerLiability.HasEmployed1In20Weeks"], (bool)Machine["EmployerRegistration.EmployerDto.EmployerLiability.HasEmployed10In20Weeks"] };
            if (listUserInput.FindAll(l => l.Equals(true))?.Count == 2)
            {
                Context.ValidationMessages.AddError("Only one answer can be Yes. Select the question that occurred first as Yes and the other(s) as No.");
                return;
            }
            else
            {
                Machine["IsLiableWeeks"] = (bool)Machine["EmployerRegistration.EmployerDto.EmployerLiability.HasEmployed1In20Weeks"] || (bool)Machine["EmployerRegistration.EmployerDto.EmployerLiability.HasEmployed10In20Weeks"] ? "Yes" : "No";
            }
        }
        public void AdminInfoNext()
        {
            if (!Machine["Password"].Equals(Machine["ReTypePassword"]))
            {
                Context.ValidationMessages.AddError("'Password' and 'Re-Enter Password' must be same.");
            }
            if (Machine["EmployerRegistration.EmployerContactDto?.Email"] != null)
            {
                if (!Machine["EmployerRegistration.EmployerContactDto.Email"].Equals(Machine["ReEmail"]))
                {
                    Context.ValidationMessages.AddError("'E-Mail' and 'Re-Enter E-Mail' must be same.");
                }
            }
        }
        public void EmpIdentificationNext()
        {
            if (((bool?)Machine["EmployerRegistration.EmployerDto.EmployerLiability?.HasPaid450RegularWages"]).Equals(true) || ((bool?)Machine["EmployerRegistration.EmployerDto.EmployerLiability?.HasEmployed1In20Weeks"]).Equals(true))
            {
                Machine["EmployerRegistration.EmployerDto.BusinessTypeCode"] = LookupTable_BusinessType.NonAgriculturalRegularEmployment;
            }
            if (((bool?)Machine["EmployerRegistration.EmployerDto.EmployerLiability.HasPaid20KAgriculturalLaborWages"]).Equals(true) || ((bool?)Machine["EmployerRegistration.EmployerDto.EmployerLiability.HasEmployed10In20Weeks"]).Equals(true))
            {
                Machine["EmployerRegistration.EmployerDto.BusinessTypeCode"] = LookupTable_BusinessType.Agricultural;
            }
            if (((bool?)Machine["EmployerRegistration.EmployerDto.EmployerLiability.HasPaid1KDomesticWages"]).Equals(true))
            {
                Machine["EmployerRegistration.EmployerDto.BusinessTypeCode"] = LookupTable_BusinessType.Domestic;
            }
            string entityTypeCode = (string)Machine["EmployerRegistration.EmployerDto.EntityTypeCode"];
            Machine["IsPublicEntity"] = entityTypeCode.Equals(LookupTable_LegalEntity.City) || entityTypeCode.Equals(LookupTable_LegalEntity.GovernmentStateAgency) || entityTypeCode.Equals(LookupTable_LegalEntity.LocalPublicBody) || entityTypeCode.Equals(LookupTable_LegalEntity.USMilitary) || entityTypeCode.Equals(LookupTable_LegalEntity.County) ? true : false;
            ValidateAddress((AddressDto)Machine["EntityAddress.Address"], Machine["LegalAddrReEmail"]?.ToString());
        }
        public void EnterBusinessInfoContNext()
        {
            if ((bool)Machine["IsPublicEntity"] && !(bool)Machine["EmployerRegistration.EmployerDto.IsExemptUnderIRS501C3"])
            {
                Context.ValidationMessages.AddError("'Is this employer a non-profit organization (hospitals, schools, municipalities & counties) that holds an exemption from federal income taxes?' cannot be 'Yes'");
            }
        }
        #endregion

        #region Misc Methods
        private void ValidateAddress(AddressDto address, String reTypeEmail)
        {
            if (!address.Email.Equals(reTypeEmail))
            {
                Context.ValidationMessages.AddError("'E-Mail' and 'Re-Enter E-Mail' must be same.");
            }
            if (address.CountryCode.Equals(LookupTable_Country.UnitedStates) && !Int64.TryParse(address.Zip, out Int64 zip))
            {
                Context.ValidationMessages.AddError("Zip should be numeric if country selected is United States.");
            }
        }
        #endregion
    }
}