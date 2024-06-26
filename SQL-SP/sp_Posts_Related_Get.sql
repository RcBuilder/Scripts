USE [MemorialDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_Posts_Related_Get]    Script Date: 25/10/2022 11:32:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- sp_Posts_Related_Get '000000001'
-- =============================================
ALTER PROCEDURE [dbo].[sp_Posts_Related_Get]
	@AccountId VARCHAR(15),
	-- paging --
	@PageNum INT = 1,
	@PageSize TINYINT = 20 
AS
BEGIN	
	SET NOCOUNT ON;    

	DECLARE @sIds VARCHAR(MAX) = (
		SELECT STUFF((
			SELECT	CONCAT(',', Id)
			FROM	[dbo].[Posts] WITH(NOLOCK)					
			WHERE	AccountId = @AccountId
			AND		IsDeleted = 0
			ORDER BY UpdatedDate DESC
			FOR XML PATH('')
		), 1, 1, '')
	)

	PRINT(@sIds)	
	EXEC sp_Posts_Get @sIds, @PageNum, @PageSize
END