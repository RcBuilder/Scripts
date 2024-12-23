USE [ExciteRollTime]
GO
/****** Object:  StoredProcedure [dbo].[sp_Report_Users_Average_Get]    Script Date: 12/12/2021 13:53:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- sp_Report_Users_Average_Get
-- sp_Report_Users_Average_Get @CompanyName=N'רולתיק'
-- sp_Report_Users_Average_Get '2021-05-01', '2021-10-01'
-- =============================================
ALTER PROCEDURE [dbo].[sp_Report_Users_Average_Get]	
	@DateFrom DATE = NULL,  
	@DateTo DATE = NULL,
	@CompanyName NVARCHAR(50) = '',
	@IsPromotionApproved BIT = 1
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

	;WITH CTE(Company, Branch, ClientId, ClientName, ClientPhone, Trs_Unique_CNT, Trs_CNT, Trs_SUM, Trs_AVG) AS ( 
		SELECT	S.CompanyName,		
				S.BranchNo,
				C.ClientUniqueId,
				C.ClientName,
				C.Phone,
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
		AND		C.IsPromotionApproved = @IsPromotionApproved
		--AND      C.ClientUniqueId IN ('000_0509522272_רולתיקגרנדקניוןחיפה')
		GROUP BY CompanyName, S.BranchNo, C.ClientUniqueId, C.ClientName, C.Phone
	)
	SELECT * FROM CTE

END
