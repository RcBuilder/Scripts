USE [ExciteRollTime]
GO
/****** Object:  StoredProcedure [dbo].[sp_Report_BirthDate_Get]    Script Date: 05/08/2021 10:28:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- sp_Report_BirthDate_Get 12
-- sp_Report_BirthDate_Get 12, N'רולתיק'
-- sp_Report_BirthDate_Get @Month=12, @Gender=N'ז'
-- sp_Report_BirthDate_Get @Month=12, @Gender=N'ז', @IsPromotionApproved=1, @CompanyName='רולתיק'
-- =============================================
ALTER PROCEDURE [dbo].[sp_Report_BirthDate_Get]	
	@Month INT = NULL,  	
	@CompanyName NVARCHAR(50) = '',	
	@Gender CHAR(1) = '',
	@IsPromotionApproved BIT = 1
AS
BEGIN	
	SET NOCOUNT ON;
	
	SELECT	S.CompanyName,			
			C.OpeningBranchName,
			--C.ClientUniqueId AS 'Client',
			C.ClientName,
			C.Phone,
			C.Email,
			C.BirthDate AS 'BirthDate',
			dbo.fn_GetAgeFromBirthDate(C.BirthDate) AS 'Age'
	FROM	[dbo].[Clients] C WITH(NOLOCK)
			INNER JOIN
			[dbo].[Stores] S WITH(NOLOCK)
			ON(C.OpeningBranchNo = S.BranchNo)			
	WHERE	MONTH(BirthDate) = @Month
	AND		(@CompanyName = '' OR S.CompanyName = @CompanyName)		
	AND		(@Gender = '' OR C.Gender = @Gender)			
	AND		(C.Phone <> '' OR C.Email <> '')
	AND		(@IsPromotionApproved IS NULL OR C.IsPromotionApproved = @IsPromotionApproved)

END
