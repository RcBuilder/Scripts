USE [ExciteRollTime]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- sp_Report_Transactions_ByMonth_Count
-- sp_Report_Transactions_ByMonth_Count @Year=2021
-- sp_Report_Transactions_ByMonth_Count @CompanyName=N'רולנט'
-- =============================================
ALTER PROCEDURE [dbo].[sp_Report_Transactions_ByMonth_Count]	
	@Year INT = NULL,
	@CompanyName NVARCHAR(50) = ''	
AS
BEGIN	
	SET NOCOUNT ON;

	IF(@Year IS NULL)
		SET @Year = YEAR(GETDATE());
			
	;WITH CTE([YEAR], [MONTH], Users_CNT, Trs_CNT, Trs_SUM, Trs_AVG) AS ( 
		SELECT	YEAR([TransactionDate]),
				MONTH([TransactionDate]),	
				COUNT(DISTINCT ClientUniqueId), 
				COUNT(TransactionNo), 
				ROUND(SUM(Amount), 2),
				ROUND(SUM(Amount) / COUNT(TransactionNo), 2)
		FROM	[dbo].[SalesLines] L WITH(NOLOCK)						
				INNER JOIN 
				[dbo].[Stores] S WITH(NOLOCK)
				ON(S.BranchNo = L.SaleBranchNo)
		WHERE	L.CategoryNo NOT IN('99999', '9005', '0')
		AND		YEAR([TransactionDate]) = @Year
		AND		(@CompanyName = '' OR S.CompanyName = @CompanyName)
		GROUP BY YEAR([TransactionDate]), MONTH([TransactionDate])		
	), CTE_Extended([YEAR], [MONTH], Users_CNT, Trs_CNT, Trs_SUM, Trs_AVG, Total_Users_CNT, Total_Trs_CNT, Total_Trs_SUM, Total_Trs_AVG) AS (
		SELECT 
			*,
			SUM(Users_CNT) OVER (PARTITION BY [YEAR]),
			SUM(Trs_CNT) OVER (PARTITION BY [YEAR]),
			SUM(Trs_SUM) OVER (PARTITION BY [YEAR]),
			ROUND((SUM(Trs_SUM) OVER (PARTITION BY [YEAR])) / (SUM(Trs_CNT) OVER (PARTITION BY [YEAR])), 2)
		FROM CTE
	) 
	SELECT	*,
			CONCAT(ROUND((Users_CNT / CAST(Total_Users_CNT AS FLOAT)), 2) * 100, '%') AS 'Users_P'
	FROM CTE_Extended
	ORDER BY [MONTH]
END

