USE [ExciteRollTime]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- sp_Report_Transactions_ByFrequency_ByClient_Count
-- sp_Report_Transactions_ByFrequency_ByClient_Count '2021-05-01', '2021-05-01', N'רולתיק'
-- sp_Report_Transactions_ByFrequency_ByClient_Count @CompanyName=N'רולתיק'
-- =============================================
ALTER PROCEDURE [dbo].[sp_Report_Transactions_ByFrequency_ByClient_Count]	
	@DateFrom DATE = NULL,  
	@DateTo DATE = NULL,
	@CompanyName NVARCHAR(50) = ''	
AS
BEGIN	
	SET NOCOUNT ON;

	-- TIME FILTER -- 
	DECLARE @addTimeFilter bit = 0;
	if(@DateFrom IS NOT NULL AND @DateTo IS NOT NULL)
		set @addTimeFilter = 1;
	
	if(@addTimeFilter = 1) 
	BEGIN
		-- fix dates -- 
		set @DateFrom = CAST((CONVERT(VARCHAR, @DateFrom, 101) + ' 00:00:00') AS DATE)
		set @DateTo = CAST((CONVERT(VARCHAR, @DateTo, 101) + ' 23:59:00') AS DATE)
	END	
	
	DECLARE @Today DATETIME = GETDATE()

	-- 1st level - stats per Company + Client + Date
	;WITH CTE([DATE], Company, Client, [Name], Phone, Email, Age, Gender, Frequency, Trs_Unique_CNT, Trs_CNT, Trs_SUM, Trs_AVG) AS ( 
		SELECT	TransactionDate,				
				S.CompanyName,		
				C.ClientUniqueId,
				C.ClientName,
				C.Phone,
				C.Email,
				DATEDIFF(YEAR, C.BirthDate, @Today),
				C.Gender,
				COUNT(DISTINCT TransactionDate),
				COUNT(DISTINCT TransactionNo), 
				COUNT(TransactionNo),
				ROUND(SUM(Amount), 2),
				ROUND(SUM(Amount) / COUNT(TransactionNo), 2)
		FROM	[dbo].[SalesLines] L WITH(NOLOCK)						
				INNER JOIN 
				[dbo].[Stores] S WITH(NOLOCK)
				ON(S.BranchNo = L.SaleBranchNo)
				INNER JOIN 
				[dbo].[Clients] C WITH(NOLOCK)
				ON(C.ClientUniqueId = L.ClientUniqueId)
		WHERE	L.CategoryNo NOT IN('99999', '9005', '0')
		AND		(@addTimeFilter = 0 OR [TransactionDate] between @DateFrom and @DateTo)		
		AND		(@CompanyName = '' OR S.CompanyName = @CompanyName)		
		--AND      C.ClientUniqueId IN ('000_0509522272_רולתיקגרנדקניוןחיפה')
		GROUP BY TransactionDate, CompanyName, C.ClientUniqueId, C.ClientName, C.Phone, C.Email, DATEDIFF(YEAR, C.BirthDate, @Today), C.Gender	
	)
	-- 2nd level - stats per Company + Client
	-- note! ignore date
	SELECT	Company, Client, [Name], Phone, Email, Age, Gender,
			SUM(Frequency) AS 'Frequency', 
			SUM(Trs_Unique_CNT) AS 'Trs_Unique_CNT', 
			SUM(Trs_CNT) AS 'Trs_CNT', 
			SUM(Trs_SUM) AS 'Trs_SUM', 
			ROUND(SUM(Trs_SUM) / SUM(Trs_CNT), 2) AS 'Trs_AVG'			
	FROM CTE
	GROUP BY Company, Client, [Name], Phone, Email, Age, Gender

END
