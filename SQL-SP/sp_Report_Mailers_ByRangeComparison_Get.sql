USE [ExciteRollTime]
GO
/****** Object:  StoredProcedure [dbo].[sp_Report_Mailers_ByRangeComparison_Get]    Script Date: 05/08/2021 11:03:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- sp_Report_Mailers_ByRangeComparison_Get '2021-04-01', '2021-04-30', 3000, '<', '2021-05-01', '2021-05-31', 3000, '<'
-- sp_Report_Mailers_ByRangeComparison_Get '2021-04-01', '2021-04-30', 3000, '<', '2021-05-01', '2021-05-31', 3000, '>', N'רולתיק'
-- sp_Report_Mailers_ByRangeComparison_Get @A_DateFrom='2020-01-01', @A_DateTo='2020-12-31', @A_Amount=3000, @B_DateFrom='2021-01-01', @B_DateTo='2021-12-31', @B_Amount=3000
-- =============================================
ALTER PROCEDURE [dbo].[sp_Report_Mailers_ByRangeComparison_Get]	
	-- RangeA --
	@A_DateFrom DATE = NULL,  
	@A_DateTo DATE = NULL,
	@A_Amount FLOAT = 0,
	@A_ConditionType CHAR(1) = '<', -- =, <. >
	
	-- RangeB --
	@B_DateFrom DATE = NULL,  
	@B_DateTo DATE = NULL,
	@B_Amount FLOAT = 0,
	@B_ConditionType CHAR(1) = '>', -- =, <. >

	@CompanyName NVARCHAR(50) = '',	
	@Gender CHAR(1) = '',	
	@SearchTerm NVARCHAR(100) = ''
AS
BEGIN	
	SET NOCOUNT ON;
	
	if(@A_DateFrom IS NULL OR @A_DateTo IS NULL)
	BEGIN
		PRINT('NO Dates For Range A')
		RETURN
	END 

	if(@B_DateFrom IS NULL OR @B_DateTo IS NULL)
	BEGIN
		PRINT('NO Dates For Range B')
		RETURN
	END 
	
	-- 1st level - clients stats + basic filters 
	;WITH CTE_A(Client, Trs_CNT, Trs_SUM) AS ( 
		SELECT	C.ClientUniqueId,				
				COUNT(TransactionNo),
				ROUND(SUM(Amount), 2)
		FROM	[dbo].[SalesLines] L WITH(NOLOCK)						
				INNER JOIN 
				[dbo].[Stores] S WITH(NOLOCK)
				ON(S.BranchNo = L.SaleBranchNo)
				INNER JOIN 
				[dbo].[Clients] C WITH(NOLOCK)
				ON(C.ClientUniqueId = L.ClientUniqueId)
				LEFT JOIN 
				CategoryDetails CD WITH(NOLOCK)
				ON(CD.CategoryNo = L.CategoryNo)
		WHERE	L.CategoryNo NOT IN('99999', '9005', '0')
		AND		([TransactionDate] between @A_DateFrom and @A_DateTo)		
		AND		(@CompanyName = '' OR S.CompanyName = @CompanyName)		
		AND		(@Gender = '' OR C.Gender = @Gender)				
		AND		(C.Phone <> '' OR C.Email <> '')
		AND		(@SearchTerm = '' OR L.CategoryName = @SearchTerm OR CD.SearchTerm1 = @SearchTerm)
		AND		C.IsPromotionApproved = 1
		GROUP BY C.ClientUniqueId
			
	), CTE_B(Client, Trs_CNT, Trs_SUM) AS ( 
		SELECT	C.ClientUniqueId,
				COUNT(TransactionNo),
				ROUND(SUM(Amount), 2)
		FROM	[dbo].[SalesLines] L WITH(NOLOCK)						
				INNER JOIN 
				[dbo].[Stores] S WITH(NOLOCK)
				ON(S.BranchNo = L.SaleBranchNo)
				INNER JOIN 
				[dbo].[Clients] C WITH(NOLOCK)
				ON(C.ClientUniqueId = L.ClientUniqueId)
				LEFT JOIN 
				CategoryDetails CD WITH(NOLOCK)
				ON(CD.CategoryNo = L.CategoryNo)
		WHERE	L.CategoryNo NOT IN('99999', '9005', '0')
		AND		([TransactionDate] between @B_DateFrom and @B_DateTo)		
		AND		(@CompanyName = '' OR S.CompanyName = @CompanyName)		
		AND		(@Gender = '' OR C.Gender = @Gender)				
		AND		(C.Phone <> '' OR C.Email <> '')
		AND		(@SearchTerm = '' OR L.CategoryName = @SearchTerm OR CD.SearchTerm1 = @SearchTerm)
		AND		C.IsPromotionApproved = 1	
		GROUP BY C.ClientUniqueId
	)
	-- 2nd level - dynamic filter by condition-type (=<>)
	SELECT	A.Client, 
			CONCAT(@A_DateFrom, ' > ', @A_DateTo, ' | ', @A_ConditionType, ' ', @A_Amount) AS 'RangeA',
			A.Trs_CNT, 
			A.Trs_SUM,
			CONCAT(@B_DateFrom, ' > ', @B_DateTo, ' | ', @B_ConditionType, ' ', @B_Amount) AS 'RangeB',
			B.Trs_CNT, 
			B.Trs_SUM 
	FROM	CTE_A A INNER JOIN CTE_B B ON(A.Client = B.Client)	
	WHERE	(
				(@A_ConditionType = '=' AND A.Trs_SUM = @A_Amount)
				OR 
				(@A_ConditionType = '>' AND A.Trs_SUM > @A_Amount)
				OR
				(@A_ConditionType = '<' AND A.Trs_SUM < @A_Amount)													
			)
	AND		(
				(@B_ConditionType = '=' AND B.Trs_SUM = @B_Amount)
				OR
				(@B_ConditionType = '>' AND B.Trs_SUM > @B_Amount)
				OR
				(@B_ConditionType = '<' AND B.Trs_SUM < @B_Amount)								
			)

END
