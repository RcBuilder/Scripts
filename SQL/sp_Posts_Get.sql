USE [MemoryApp]
GO
/****** Object:  StoredProcedure [dbo].[sp_Posts_Get]    Script Date: 03/04/2022 09:07:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- sp_Posts_Get '3,2,1'
-- sp_Posts_Get '3,2,1', @PageNum = 1, @PageSize = 2
-- =============================================
ALTER PROCEDURE [dbo].[sp_Posts_Get]
	@sIds VARCHAR(MAX),
	-- paging --
	@PageNum int = 1,
	@PageSize tinyint = 20 
AS
BEGIN	
	SET NOCOUNT ON;    
		
	-- PAGING CALCULATION --
	declare @IndexFrom int = ((@PageNum - 1) * @PageSize) + 1
	declare @IndexTo int = @PageNum * @PageSize
	--------

	DECLARE @tIdsAll TABLE(RowId INT, Id INT)
	;WITH CTE_Ids (RowId, Id) AS (		
		SELECT	ROW_NUMBER() OVER(ORDER BY (SELECT NULL)),
				RTRIM(LTRIM([Value])) 
		FROM	STRING_SPLIT(@sIds, ',')		
	)	
	INSERT INTO @tIdsAll
		SELECT RowId, Id FROM CTE_Ids

	DECLARE @tIds TABLE(Id INT)		
	INSERT INTO @tIds
		SELECT Id FROM @tIdsAll WHERE RowId BETWEEN @IndexFrom AND @IndexTo

	-- 1st table: posts --
	SELECT	p.*
	FROM	[dbo].[Posts] p WITH(NOLOCK)	
			INNER JOIN @tIds t  -- preserve order!
			ON(t.Id = p.Id)
	-- WHERE	Id IN (SELECT Id FROM @tIds)
	WHERE	IsDeleted = 0	
	-- ORDER BY UpdatedDate DESC  -- DO NOT SORT!

	-- 2nd table: likes --
	SELECT	L.*, U.ProfilePhoto
	FROM	[dbo].[Likes] L WITH(NOLOCK)
			INNER JOIN 
			[dbo].[Users] U WITH(NOLOCK)
			ON (U.Id = L.UserId)
	WHERE	L.PostId IN (SELECT Id FROM @tIds)	

	-- 3rd table: comments --
	SELECT	C.*, U.ProfilePhoto
	FROM	[dbo].[Comments] C WITH(NOLOCK)
			INNER JOIN 
			[dbo].[Users] U WITH(NOLOCK)
			ON (U.Id = C.UserId)
	WHERE	PostId IN (SELECT Id FROM @tIds)
	AND		C.IsDeleted = 0

	-- 4th table: paging --
	DECLARE @RowCount INT = (SELECT COUNT(*) FROM @tIdsAll)
	SELECT	@RowCount As 'RowCount', 
			@PageNum As 'PageNum', 
			@PageSize As 'PageSize', 
			@IndexFrom As 'IndexFrom', 
			@IndexTo As 'IndexTo' 	 

END
