USE [MemorialDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_Tag_Posts_Get]    Script Date: 25/10/2022 17:44:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- sp_Tag_Posts_Get '000000001', N'תגית-3'
-- =============================================
ALTER PROCEDURE [dbo].[sp_Tag_Posts_Get]	
	@AccountId VARCHAR(15),
	@Tag NVARCHAR(30),
	-- paging --
	@PageNum INT = 1,
	@PageSize TINYINT = 20 
AS
BEGIN	
	SET NOCOUNT ON;    
		
	DECLARE @sIds VARCHAR(MAX) = (
		SELECT STUFF((
			SELECT	CONCAT(',', PostId)
			FROM	PostsTags T WITH(NOLOCK) 
					INNER JOIN 
					Posts P WITH(NOLOCK) 
					ON(P.Id = T.PostId)
			WHERE	AccountId = @AccountId
			AND		T.Tag = @Tag
			AND		IsDeleted = 0
			ORDER BY P.UpdatedDate DESC

			FOR XML PATH('')
		), 1, 1, '')
	)

	PRINT(@sIds)
	EXEC sp_Posts_Get @sIds, @PageNum, @PageSize
END