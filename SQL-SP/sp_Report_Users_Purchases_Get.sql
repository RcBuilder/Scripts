USE [ExciteRollTime]
GO
/****** Object:  StoredProcedure [dbo].[sp_Report_Users_Purchases_Get]    Script Date: 12/12/2021 19:32:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- [@DateFrom, @DateTo, @CompanyName, @AmountFrom, @AmountTo, @IsPromotionApproved]
-- sp_Report_Users_Purchases_Get
-- sp_Report_Users_Purchases_Get @CompanyName=N'רולתיק'
-- sp_Report_Users_Purchases_Get '2021-05-01', '2021-10-01'
-- sp_Report_Users_Purchases_Get @AmountFrom=0, @AmountTo=500
-- sp_Report_Users_Purchases_Get @IsPromotionApproved=1
-- =============================================
ALTER PROCEDURE [dbo].[sp_Report_Users_Purchases_Get]	
	@DateFrom DATE = NULL,  
	@DateTo DATE = NULL,
	@CompanyName NVARCHAR(50) = '',
	@AmountFrom FLOAT = '-1.79E+308',
	@AmountTo FLOAT = '1.79E+308',
	@IsPromotionApproved BIT = NULL
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
	
	PRINT(CONCAT(@DateFrom, ' - ', @DateTo))

	DECLARE @tIDs TABLE(RowId INT)
	INSERT INTO @tIDs
		SELECT	L.RowId
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
		AND		(Amount BETWEEN @AmountFrom AND @AmountTo)
		AND		(@IsPromotionApproved IS NULL OR C.IsPromotionApproved = @IsPromotionApproved)
		--AND      C.ClientUniqueId IN ('016_0523996963_חייםקריסי')

	;WITH CTE(Company, Branch, ClientId, ClientName, ClientPhone, ItemsCount) AS ( 
		SELECT	S.CompanyName,		
				S.BranchNo,
				C.ClientUniqueId,
				C.ClientName,
				C.Phone,
				COUNT(L.RowId)
		FROM	[dbo].[SalesLines] L WITH(NOLOCK)						
				INNER JOIN 
				[dbo].[Stores] S WITH(NOLOCK)
				ON(S.BranchNo = L.SaleBranchNo)
				INNER JOIN 
				[dbo].[Clients] C WITH(NOLOCK)
				ON(C.ClientUniqueId = L.ClientUniqueId)
		WHERE	L.RowId IN(SELECT RowId FROM @tIDs)		
		GROUP BY CompanyName, S.BranchNo, C.ClientUniqueId, C.ClientName, C.Phone
	)
	SELECT	* 
	FROM	CTE C 
			CROSS APPLY (
				SELECT STUFF((
					SELECT	' | ' + ItemName 
					FROM	[dbo].[SalesLines] WITH(NOLOCK) 
					WHERE	RowId IN(SELECT RowId FROM @tIDs)
					AND		ClientUniqueId = C.ClientId AND SaleBranchNo = C.Branch
				FOR XML PATH('')), 1, 1, '')		
			) P([Items])
	--SELECT * FROM SalesLines WHERE ClientUniqueId = '000_0509522272_רולתיקגרנדקניוןחיפה' AND [TransactionDate] between @DateFrom and @DateTo
END
