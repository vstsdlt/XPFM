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
using PFML.Shared.ViewModels.Premium.Waiver;
using FACTS.Framework.Support;

namespace PFML.BusinessLogic.Premium.Waiver.VPRequest
{

	public static class VPRequestLogic
	{

		[OperationMethod]
		public static List<VPRequestViewModel> GetList(int EmployerId)
		{
			return GetRequestColl(EmployerId);
		}

		[OperationMethod]
		public static VoluntaryPlanWaiverRequestDto GetRequest(int RequestId)
		{

			using (DbContext context = new DbContext())
			{
				#region Commented
				//var query = (from req in context.VoluntaryPlanWaiverRequests

				//			 join frm in context.Forms
				//			 on req.FormId equals frm.FormId

				//			 join reqtyp in context.VoluntaryPlanWaiverRequestTypes
				//			 on req.VoluntaryPlanWaiverRequestId equals reqtyp.VoluntaryPlanWaiverRequestId

				//			 //join frmatt in context.FormAttachments
				//			 //on frm.FormId equals frmatt.FormId

				//			 //join doc in context.Documents
				//			 //on frmatt.DocumentId equals doc.DocumentID

				//			 where req.VoluntaryPlanWaiverRequestId == RequestId
				//			 select new VPRequestViewModel
				//			 {
				//				 RequestId = req.VoluntaryPlanWaiverRequestId,
				//				 StartDate = req.StartDate,
				//				 EndDate = req.EndDate,
				//				 TypesofLeaveAvailable = reqtyp.LeaveTypeCode,
				//				 WeeksAvailableAnnually = reqtyp.DurationInWeeksCode,
				//				 PercentageofWagesPaid = reqtyp.PercentagePaid,
				//				 DocumentName = "My Document"//doc.DocumentName

				//			 })?.ToList();
				//return query; 
				#endregion
				var record = (from vpwr in context.VoluntaryPlanWaiverRequests.Include("VoluntaryPlanWaiverRequestTypes").Include("Form.FormAttachments.Document")
							  where vpwr.VoluntaryPlanWaiverRequestId == RequestId
							  select vpwr).FirstOrDefault();
				return record.ToDtoDeep(context);

			}
		}

		[OperationMethod]
		public static List<VPRequestViewModel> SubmitRequest(VPRequestForm VPRequestForm)
		{
			#region Commented
			//VoluntaryPlanWaiverRequestDto obj = new VoluntaryPlanWaiverRequestDto();
			//obj.EmployerId = 1;
			//obj.StartDate = DateTime.Now;
			//obj.EndDate = DateTime.Now;
			//obj.IsVoluntaryPlanWaiverRequestAcknowledged = true;
			//obj.VoluntaryPlanWaiverRequestTypes.Add(new VoluntaryPlanWaiverRequestTypeDto()
			//{
			//	LeaveTypeCode = LookupTable_WaiverLeaveType.Medical,
			//	PercentagePaid = 2,
			//	DurationInWeeksCode = LookupTable_WaiverWeeksAvailable._1Week
			//});
			//obj.VoluntaryPlanWaiverRequestTypes.Add(new VoluntaryPlanWaiverRequestTypeDto()
			//{
			//	LeaveTypeCode = LookupTable_WaiverLeaveType.Medical,
			//	PercentagePaid = 1,
			//	DurationInWeeksCode = LookupTable_WaiverWeeksAvailable._4Weeks
			//});

			//VPRequestForm = new VPRequestForm()
			//{
			//	EmployerId = 1,
			//	StartDate = DateTime.Now,
			//	EndDate = DateTime.Now,
			//	IsAcknolwedged = true,
			//	RequestType = new List<VoluntaryPlanWaiverRequestTypeDto>() {
			//		new VoluntaryPlanWaiverRequestTypeDto()
			//		{
			//			 LeaveTypeCode = LookupTable_WaiverLeaveType.Medical,
			//			  PercentagePaid = 2,
			//			  DurationInWeeksCode = LookupTable_WaiverWeeksAvailable._1Week
			//		},
			//		new VoluntaryPlanWaiverRequestTypeDto()
			//		{
			//			 LeaveTypeCode = LookupTable_WaiverLeaveType.Medical,
			//			  PercentagePaid = 1,
			//			  DurationInWeeksCode = LookupTable_WaiverWeeksAvailable._1Week
			//		}
			//	},
			//	Document = new List<DocumentDto>() {
			//		 new DocumentDto(){
			//			  DocumentDescription="Doc Description",
			//			  DocumentName ="DJ.pdf"
			//		 }
			//	}

			//};
			#endregion

			ExecuteSubmitRequestRules(VPRequestForm);
			if (Context.ValidationMessages.Count(ValidationMessageSeverity.Error) == 0)
			{
				using (DbContext context = new DbContext())
				{
					FormDto FormReq = new FormDto()
					{
						FormTypeCode = LookupTable_FormType.VPWaiverForm,
						StatusCode = LookupTable_WaiverRequestStatus.SavedasDraft

					};
					Form.FromDto(context, FormReq);

					VoluntaryPlanWaiverRequestDto VPWReq = new VoluntaryPlanWaiverRequestDto()
					{
						EmployerId = VPRequestForm.EmployerId,
						StartDate = VPRequestForm.StartDate,
						EndDate = VPRequestForm.EndDate,
						IsVoluntaryPlanWaiverRequestAcknowledged = VPRequestForm.IsAcknolwedged
					};
					VoluntaryPlanWaiverRequest.FromDto(context, VPWReq);


					VPRequestForm.RequestType.RemoveAll(p => string.IsNullOrWhiteSpace(p.LeaveTypeCode));
					foreach (var requestType in VPRequestForm.RequestType)
					{
						VoluntaryPlanWaiverRequestType VPWReqType = new VoluntaryPlanWaiverRequestType()
						{
							LeaveTypeCode = requestType.LeaveTypeCode,
							PercentagePaid = requestType.PercentagePaid,
							DurationInWeeksCode = requestType.DurationInWeeksCode
						};
						context.VoluntaryPlanWaiverRequestTypes.Add(VPWReqType);
					}

					if (VPRequestForm.Document != null)
					{
						foreach (var docItem in VPRequestForm.Document)
						{
							Document doc = new Document()
							{
								DocumentName = docItem.DocumentName,
								ExternalDocumentId = Guid.NewGuid(),
								DocumentDescription = docItem.DocumentDescription

							};
							context.Documents.Add(doc);
							context.FormAttachments.Add(new FormAttachment());
						}

					}

					context.SaveChanges();
				}
			}
			else
			{
				Context.ValidationMessages.ThrowCheck(ValidationMessageSeverity.Error);
			}
			return GetRequestColl(VPRequestForm.EmployerId);
		}

		[OperationMethod]
		public static VPRequestForm SetData(int EmployerId)
		{
			return SetLeaveType();
		}



		#region Private Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="EmployerId"></param>
		/// <returns></returns>
		internal static List<VPRequestViewModel> GetRequestColl(int? EmployerId)
		{
			if (EmployerId is null)
			{
				return new List<VPRequestViewModel>();
			}
			else
			{
				using (DbContext context = new DbContext())
				{
					var query = (from req in context.VoluntaryPlanWaiverRequests
								 from form in context.Forms
								 where req.EmployerId == EmployerId && req.FormId == form.FormId
								 select new VPRequestViewModel
								 {
									 RequestId = req.VoluntaryPlanWaiverRequestId,
									 StartDate = req.StartDate,
									 EndDate = req.EndDate,
									 StatusCode = form.StatusCode
								 })?.ToList();

					return query;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="VPRequestForm"></param>
		/// <returns></returns>
		internal static VPRequestForm SetLeaveType(VPRequestForm VPRequestForm = null)
		{
			List<VoluntaryPlanWaiverRequestTypeDto> VoluntaryPlanWaiverRequestType = new List<VoluntaryPlanWaiverRequestTypeDto>();
			for (int i = 0; i < 4; i++)
			{
				VoluntaryPlanWaiverRequestType.Add(new VoluntaryPlanWaiverRequestTypeDto());
			}
			if (VPRequestForm is null)
			{
				return new VPRequestForm()
				{
					RequestType = VoluntaryPlanWaiverRequestType,
					StartDate = DateTime.Now.Date,
					EndDate = new DateTime(DateTime.Now.Year, 12, 31)

				};
			}
			else
			{
				VPRequestForm.RequestType = VoluntaryPlanWaiverRequestType;
				return VPRequestForm;
			}
		}
		#endregion

		#region Business Rules
		internal static void ExecuteSubmitRequestRules(VPRequestForm VPRequestForm)
		{
			if (VPRequestForm.StartDate > DateTime.Today)
			{
				Context.ValidationMessages.AddError("Start Date cannot be before today.");
			}

			if (VPRequestForm.EndDate < VPRequestForm.StartDate)
			{
				Context.ValidationMessages.AddError("End date cannot be before Start date.");
			}

			if (VPRequestForm.EndDate > VPRequestForm.StartDate.AddYears(1))
			{
				Context.ValidationMessages.AddError("End Date cannot be more than 1 year after the Start Date");
			}

			if (!VPRequestForm.IsAcknolwedged)
			{
				Context.ValidationMessages.AddError("You must agree to the T&C to submit your request");
			}

			bool leaveTypeChecked = false;

			foreach (var item in VPRequestForm.RequestType)
			{
				if (!string.IsNullOrWhiteSpace(item.LeaveTypeCode))
				{
					leaveTypeChecked = true;
					if (string.IsNullOrWhiteSpace(item.DurationInWeeksCode) && (item.PercentagePaid <= 0))
					{
						Context.ValidationMessages.AddError("Weeks Available annually and Percentage of Wages Paid are required fields for the checked leave type");
						break;
					}
					if (string.IsNullOrWhiteSpace(item.DurationInWeeksCode) && (item.PercentagePaid >= 0))
					{
						Context.ValidationMessages.AddError("Please select Weeks Available annually for the checked leave type");
					}
					if (!string.IsNullOrWhiteSpace(item.DurationInWeeksCode) && (item.PercentagePaid <= 0))
					{
						Context.ValidationMessages.AddError("Please provide Percentage of wages paid for the checked leave type");
						break;
					}
				}
			}
			if (!leaveTypeChecked)
			{
				Context.ValidationMessages.AddError("Atleast one Leave Type should be provided");
				//Available annually and Percentage of Wages Paid in the type of leave available section should be provided
			}

		}
		#endregion
	}
}
