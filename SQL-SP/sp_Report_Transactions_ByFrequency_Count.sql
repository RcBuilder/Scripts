USE [ExciteRollTime]
GO
/****** Object:  StoredProcedure [dbo].[sp_Report_Transactions_ByMonth_Count]    Script Date: 02/08/2021 14:42:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- sp_Report_Transactions_ByFrequency_Count
-- sp_Report_Transactions_ByFrequency_Count '2021-04-28', '2021-04-28', N'רולנט'
-- sp_Report_Transactions_ByFrequency_Count @CompanyName=N'רולתיק'
-- =============================================
ALTER PROCEDURE [dbo].[sp_Report_Transactions_ByFrequency_Count]	
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
	
	SELECT	S.CompanyName AS 'Company',			
			COUNT(DISTINCT TransactionDate) AS 'Frequency', 
			COUNT(DISTINCT TransactionNo) AS 'Trs_Unique_CNT', 
			COUNT(TransactionNo) AS 'Trs_CNT',
			ROUND(SUM(Amount), 2) AS 'Trs_SUM',
			ROUND(SUM(Amount) / COUNT(TransactionNo), 2) AS 'Trs_AVG'
	FROM	[dbo].[SalesLines] L WITH(NOLOCK)						
			INNER JOIN 
			[dbo].[Stores] S WITH(NOLOCK)
			ON(S.BranchNo = L.SaleBranchNo)
	WHERE	L.CategoryNo NOT IN('99999', '9005', '0')
	AND		(@addTimeFilter = 0 OR [TransactionDate] between @DateFrom and @DateTo)		
	AND		(@CompanyName = '' OR S.CompanyName = @CompanyName)	
	--AND      ClientUniqueId IN ('000_0509522272_רולתיקגרנדקניוןחיפה')
	GROUP BY CompanyName		
END

