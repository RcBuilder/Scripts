USE [ExciteRollTime]
GO
/****** Object:  StoredProcedure [dbo].[sp_Report_NewUsers_ByMonth_Count]    Script Date: 21/07/2021 10:27:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- sp_Report_NewUsers_ByMonth_Count 
-- sp_Report_NewUsers_ByMonth_Count @CompanyName=N'רולנט'
-- =============================================
ALTER PROCEDURE [dbo].[sp_Report_NewUsers_ByMonth_Count]	
	@CompanyName NVARCHAR(50) = '',
	@MinYear INT = 2020
AS
BEGIN	
	SET NOCOUNT ON;

	;WITH CTE([CNT], [YEAR], [MONTH]) AS (
		SELECT	COUNT(DISTINCT C.ClientUniqueId) AS CNT, 
				YEAR([OpeningDate]) AS 'YEAR',
				MONTH([OpeningDate]) AS 'MONTH'			
		FROM	[dbo].[Clients] C WITH(NOLOCK)			
				INNER JOIN 
				[dbo].[Stores] S WITH(NOLOCK)
				ON(S.BranchNo = C.OpeningBranchNo)
		WHERE	(@CompanyName = '' OR S.CompanyName = @CompanyName)
		AND		(@MinYear = 0 OR YEAR([OpeningDate]) >= @MinYear)
		GROUP BY YEAR([OpeningDate]), MONTH([OpeningDate])		
	)
	SELECT * FROM CTE	
	PIVOT(
		SUM([CNT]) 
		FOR [YEAR] IN ([2020], [2021])
    ) AS Pvt
	ORDER BY [MONTH]

END
