USE [ExciteRollTime]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- sp_Report_Weekly_Get
-- sp_Report_Weekly_Get @CompanyName=N'רולתיק'
-- =============================================
ALTER PROCEDURE [dbo].[sp_Report_Weekly_Get]		
	@CompanyName NVARCHAR(50) = ''	
AS
BEGIN	
	SET NOCOUNT ON;

	DECLARE @TODAY DATE = GETDATE()	
	DECLARE @SUNDAY DATE = (SELECT DATEADD(DAY, -(DATEPART(WEEKDAY, @TODAY)) + 1, @TODAY))  
	DECLARE @SATURDAY DATE = (SELECT DATEADD(DAY, 7, @SUNDAY))  

	EXEC sp_Report_Users_Purchases_Get @SUNDAY, @SATURDAY, @CompanyName, 0

END
