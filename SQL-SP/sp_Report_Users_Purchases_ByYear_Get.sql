USE [ExciteRollTime]
GO
/****** Object:  StoredProcedure [dbo].[sp_Report_Users_Purchases_ByYear_Get]    Script Date: 14/12/2021 09:52:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- sp_Report_Users_Purchases_ByYear_Get 
-- sp_Report_Users_Purchases_ByYear_Get @CompanyName=N'רולנט'
-- sp_Report_Users_Purchases_ByYear_Get @AmountFrom=200
-- =============================================
ALTER PROCEDURE [dbo].[sp_Report_Users_Purchases_ByYear_Get]		
	@CompanyName NVARCHAR(50) = '',
	@AmountFrom FLOAT = '-1.79E+308',
	@AmountTo FLOAT = '1.79E+308',
	@IsPromotionApproved BIT = NULL
AS
BEGIN	
	SET NOCOUNT ON;

	;WITH CTE(Company, Branch, ClientId, ClientName, [YEAR], Trs_CNT) AS (		
		SELECT	S.CompanyName,		
				S.BranchNo,
				C.ClientUniqueId,
				C.ClientName,						
				YEAR([OpeningDate]),												
				COUNT(TransactionNo)				
		FROM	[dbo].[SalesLines] L WITH(NOLOCK)						
				INNER JOIN 
				[dbo].[Stores] S WITH(NOLOCK)
				ON(S.BranchNo = L.SaleBranchNo)
				INNER JOIN 
				[dbo].[Clients] C WITH(NOLOCK)
				ON(C.ClientUniqueId = L.ClientUniqueId)
		WHERE	L.CategoryNo NOT IN('99999', '9005', '0')		
		AND		(@CompanyName = '' OR S.CompanyName = @CompanyName)			
		AND		(Amount BETWEEN @AmountFrom AND @AmountTo)
		AND		(@IsPromotionApproved IS NULL OR C.IsPromotionApproved = @IsPromotionApproved)
		AND      C.ClientUniqueId IN ('016_0523996963_חייםקריסי')	
		GROUP BY CompanyName, S.BranchNo, C.ClientUniqueId, C.ClientName, YEAR([OpeningDate])
	)
	SELECT * FROM CTE	
	PIVOT(
		SUM(Trs_CNT) 
		FOR [YEAR] IN ([2013], [2014], [2015], [2016], [2017], [2018], [2019], [2020], [2021], [2022])
    ) AS Pvt	

	SELECT * FROM SalesLines WHERE ClientUniqueId = '016_0523996963_חייםקריסי'
END
