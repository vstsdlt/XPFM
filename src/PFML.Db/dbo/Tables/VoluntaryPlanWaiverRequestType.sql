CREATE TABLE [dbo].[VoluntaryPlanWaiverRequestType] (
    [VoluntaryPlanWaiverRequestTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [VoluntaryPlanWaiverRequestId]     INT            NOT NULL,
    [LeaveTypeCode]                    VARCHAR (20)   NOT NULL,
    [PercentagePaid]                   DECIMAL (5, 2) NOT NULL,
    [DurationInWeeksCode]              VARCHAR (20)   NOT NULL,
    [CreateUserId]                     VARCHAR (50)   NOT NULL,
    [CreateDateTime]                   DATETIME       NOT NULL,
    [UpdateUserId]                     VARCHAR (50)   NOT NULL,
    [UpdateDateTime]                   DATETIME       NOT NULL,
    [UpdateNumber]                     INT            NULL,
    [UpdateProcess]                    VARCHAR (100)  NULL,
    CONSTRAINT [PK_VoluntaryPlanWaiverRequestType] PRIMARY KEY CLUSTERED ([VoluntaryPlanWaiverRequestTypeId] ASC),
    CONSTRAINT [FK_VoluntaryPlanWaiverRequestType_VoluntaryPlanWaiverRequest_VoluntaryPlanWaiverRequestId] FOREIGN KEY ([VoluntaryPlanWaiverRequestId]) REFERENCES [dbo].[VoluntaryPlanWaiverRequest] ([VoluntaryPlanWaiverRequestId])
);

