USE [ExciteRollTime]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- sp_Report_Transactions_ByAgeGroup_WithAmountRange_Count
-- sp_Report_Transactions_ByAgeGroup_WithAmountRange_Count @AmountRange=3000
-- sp_Report_Transactions_ByAgeGroup_WithAmountRange_Count '2021-05-01', '2021-05-01', N'רולתיק'
-- sp_Report_Transactions_ByAgeGroup_WithAmountRange_Count @CompanyName=N'רולתיק'
-- sp_Report_Transactions_ByAgeGroup_WithAmountRange_Count @CompanyName=N'רולנט', @AmountRange=3000
-- =============================================
ALTER PROCEDURE [dbo].[sp_Report_Transactions_ByAgeGroup_WithAmountRange_Count]	
	@DateFrom DATE = NULL,  
	@DateTo DATE = NULL,
	@CompanyName NVARCHAR(50) = '',
	@AmountRange FLOAT = NULL 
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
	
	;WITH CTE_DATA(BirthDate, Company, Client, TransactionNo, Amount) AS ( 
		SELECT	C.BirthDate,
				S.CompanyName,		
				L.ClientUniqueId,
				L.TransactionNo,
				L.Amount				
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
		--AND	C.IsPromotionApproved = 1
		--AND   C.ClientUniqueId IN ('000_0509522272_רולתיקגרנדקניוןחיפה')
		
	), CTE_STATS(AgeGroup, Company, Usrs_Unique_CNT, Trs_Unique_CNT, Trs_CNT, Trs_SUM, Trs_AVG) AS(
		SELECT	dbo.fn_GetAgeGroup(dbo.fn_GetAgeFromBirthDate(BirthDate)),
				Company,	
				
				COUNT(DISTINCT Client), 
				COUNT(DISTINCT TransactionNo), 			
				COUNT(TransactionNo),
				ROUND(SUM(Amount), 2),
				ROUND(SUM(Amount) / COUNT(TransactionNo), 2)

		FROM	CTE_DATA		
		GROUP BY dbo.fn_GetAgeGroup(dbo.fn_GetAgeFromBirthDate(BirthDate)), Company		
	), CTE_UNIQUE_ABOVE_X(AgeGroup, Company, Usrs_Unique_CNT) AS(
		SELECT	dbo.fn_GetAgeGroup(dbo.fn_GetAgeFromBirthDate(BirthDate)),
				Company,					
				COUNT(DISTINCT Client)
		FROM	CTE_DATA
		WHERE	Amount > @AmountRange
		GROUP BY dbo.fn_GetAgeGroup(dbo.fn_GetAgeFromBirthDate(BirthDate)), Company		
	), CTE_UNIQUE_BELOW_X(AgeGroup, Company, Usrs_Unique_CNT) AS(
		SELECT	dbo.fn_GetAgeGroup(dbo.fn_GetAgeFromBirthDate(BirthDate)),
				Company,					
				COUNT(DISTINCT Client)
		FROM	CTE_DATA
		WHERE	Amount <= @AmountRange
		GROUP BY dbo.fn_GetAgeGroup(dbo.fn_GetAgeFromBirthDate(BirthDate)), Company		
	)	
	
	SELECT	S.*, 
			@AmountRange AS 'X',
			ISNULL(B.Usrs_Unique_CNT, 0) AS 'Usrs_Unique_Below_X_CNT', 
			ISNULL(A.Usrs_Unique_CNT, 0) AS 'Usrs_Unique_Above_X_CNT' 
	FROM	CTE_STATS S			
			LEFT JOIN 
			CTE_UNIQUE_BELOW_X B
			ON(B.AgeGroup = S.AgeGroup AND B.Company = S.Company)
			LEFT JOIN 
			CTE_UNIQUE_ABOVE_X A
			ON(A.AgeGroup = S.AgeGroup AND A.Company = S.Company)	
END
