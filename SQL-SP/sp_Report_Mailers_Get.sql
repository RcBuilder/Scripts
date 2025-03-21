USE [ExciteRollTime]
GO
/****** Object:  StoredProcedure [dbo].[sp_Report_Mailers_Get]    Script Date: 07/08/2021 19:47:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- sp_Report_Mailers_Get
-- sp_Report_Mailers_Get @CompanyName=N'רולתיק'
-- sp_Report_Mailers_Get '2021-05-01', '2021-05-01', N'רולתיק'
-- sp_Report_Mailers_Get '2021-05-01', '2021-05-01', N'רולתיק', @AmountMin = 0, @AmountMax = 300
-- sp_Report_Mailers_Get @CompanyName = 'רולתיק', @AmountMin = 0, @AmountMax = 300, @SearchTerm = N'תיקים'
-- =============================================
ALTER PROCEDURE [dbo].[sp_Report_Mailers_Get]	
	@DateFrom DATE = NULL,  
	@DateTo DATE = NULL,
	@CompanyName NVARCHAR(50) = '',	
	@Gender CHAR(1) = '',
	@AmountMin FLOAT = NULL,
	@AmountMax FLOAT = NULL,
	@SearchTerm NVARCHAR(100) = ''

	/*
		@RemoveWrongEmails
		@RemoveWrongPhones
	*/
AS
BEGIN	
	SET NOCOUNT ON;

	IF(@AmountMin IS NULL)
		SET @AmountMin = CAST('-1.79E+308' AS FLOAT) -- MIN FLOAT VALUE
	IF(@AmountMax IS NULL)
		SET @AmountMax = CAST('1.79E+308' AS FLOAT) -- MAX FLOAT VALUE

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

	-- 1st level - transaction  & client details
	;WITH CTE(Company, TrsDate, TrsId, Amount, CategoryName, CategorySearchTerm1, Client, [Name], Phone, Email, Age, Gender) AS ( 
		SELECT	S.CompanyName,
				L.TransactionDate,
				L.TransactionNo,
				L.Amount,
				L.CategoryName,
				CD.SearchTerm1,						
				C.ClientUniqueId,
				C.ClientName,
				C.Phone,
				C.Email,				
				dbo.fn_GetAgeFromBirthDate(C.BirthDate),
				C.Gender
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
		AND		(@addTimeFilter = 0 OR [TransactionDate] between @DateFrom and @DateTo)		
		AND		(@CompanyName = '' OR S.CompanyName = @CompanyName)		
		AND		(@Gender = '' OR C.Gender = @Gender)		
		AND		(L.Amount BETWEEN @AmountMin AND @AmountMax)
		AND		(C.Phone <> '' OR C.Email <> '')
		AND		(@SearchTerm = '' OR L.CategoryName = @SearchTerm OR CD.SearchTerm1 = @SearchTerm)
		AND		C.IsPromotionApproved = 1
		--AND      C.ClientUniqueId IN ('000_0509522272_רולתיקגרנדקניוןחיפה')				
	)
	-- 2nd level - unique clients
	SELECT DISTINCT Company, [Name], Phone, Email	
	FROM CTE 
	-- WHERE dbo.fn_IsValidEmail(Email) = 1	
	-- WHERE dbo.fn_IsValidPhone(Phone) = 1	

END
