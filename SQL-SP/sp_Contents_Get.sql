USE [MemorialEvents]
GO
/****** Object:  StoredProcedure [dbo].[sp_Contents_Get]    Script Date: 25/12/2022 13:09:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	-- =============================================
	-- Author:		<RcBuilder>
	-- sp_Contents_Get ''
	-- =============================================
	ALTER PROCEDURE [dbo].[sp_Contents_Get]
		@sIds VARCHAR(MAX)
	AS
	BEGIN	
		SET NOCOUNT ON;    
			
		IF(@sIds = '') -- ALL --
		BEGIN
			SELECT	*
			FROM	[dbo].[Contents] WITH(NOLOCK)
		END 
		ELSE -- By Ids filter --
		BEGIN
			;WITH CTE_Ids (Id) AS (		
				SELECT * FROM STRING_SPLIT(@sIds, ',')
			)		
			SELECT	*
			FROM	[dbo].[Contents] WITH(NOLOCK)		
			WHERE	RowId IN (SELECT Id FROM CTE_Ids)
		END 
	END
