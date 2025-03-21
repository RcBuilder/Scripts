USE [MemorialDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_Posts_Get]    Script Date: 25/10/2022 12:18:18 ******/
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

	DECLARE @tIds TABLE(Sort INT IDENTITY(1,1), Id INT)		
	INSERT INTO @tIds
		SELECT Id FROM @tIdsAll WHERE RowId BETWEEN @IndexFrom AND @IndexTo
	
	-- 1st table: posts --
	SELECT	P.*
	FROM	[dbo].[Posts] P WITH(NOLOCK)		
			INNER JOIN 
			@tIds T
			ON(T.Id = P.Id)	
	WHERE	P.IsDeleted = 0	
	ORDER BY T.Sort	
	-- ORDER BY UpdatedDate DESC  -- DO NOT SORT! PRESERVE ORIGINAL ORDER! (See Sort Column)

	-- 2nd table: tags --
	SELECT	PostId, Tag 
	FROM	PostsTags P WITH(NOLOCK) 
			INNER JOIN 
			@tIds T
			ON(T.Id = P.PostId)		
	
	-- 3rd table: paging --
	DECLARE @RowCount INT = (SELECT COUNT(*) FROM @tIdsAll)
	SELECT	@RowCount As 'RowCount', 
			@PageNum As 'PageNum', 
			@PageSize As 'PageSize', 
			@IndexFrom As 'IndexFrom', 
			@IndexTo As 'IndexTo' 	 

END