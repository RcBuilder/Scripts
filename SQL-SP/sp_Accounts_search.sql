SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- sp_Accounts_search 'τμεπι'
-- =============================================
ALTER PROCEDURE [dbo].[sp_Accounts_search]	
	@FreeText NVARCHAR(100) = '',
	-- paging --
	@PageNum int = 1,
	@PageSize tinyint = 20 
AS
BEGIN
	SET NOCOUNT ON;
		
	-- all posts (grade or class level) under the school/s --
	DECLARE @sIds VARCHAR(MAX) = (
		SELECT STUFF((
			SELECT	CONCAT(',', Id)
			FROM	[dbo].[Accounts] WITH(NOLOCK)
			WHERE	IsDeleted = 0	
			AND		(
						@FreeText = '' 
						OR 
						Id LIKE '%' + @FreeText + '%'
						OR 
						FirstName LIKE '%' + @FreeText + '%'
						OR 				
						LastName LIKE '%' + @FreeText + '%'
						OR 				
						About LIKE '%' + @FreeText + '%'
						OR 				
						City LIKE '%' + @FreeText + '%'
					)
			FOR XML PATH('')
		), 1, 1, '')
	)

	PRINT(@sIds)
	EXEC sp_Accounts_Get @sIds, @PageNum, @PageSize
	
END
