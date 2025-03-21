USE [GF_db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>

/*
	DECLARE @FilterMatch AS dbo.FilterMatch -- (RowIndex INT, Makat NVARCHAR(MAX)) 
	INSERT INTO @FilterMatch VALUES(1, 'BFU700x')
	EXEC FindGurByMakats @FilterMatch
*/

-- BFU700x > GFI81700
-- SELECT Makat, ID FROM ManFilters WHERE Makat = 'BFU700x' -- 155648
-- SELECT ManID, GurID FROM [dbo].[FilterConnections] WHERE ManID = 155648
-- SELECT Makat FROM [dbo].[GurFilters] WHERE ID = 3871
-- =============================================
ALTER PROCEDURE [dbo].[FindGurByMakats]
	@data FilterMatch readonly
AS
BEGIN	
	SET NOCOUNT ON;
	

	SELECT	DISTINCT d.RowIndex, g.Makat
	FROM	FilterConnections fc 
			INNER JOIN 
			GurFilters g
			ON(g.ID = fc.GurID) 
			INNER JOIN 
			ManFilters m
			ON(m.ID = fc.ManID)
			INNER JOIN 
			AllMakats am 				
			ON(m.Makat = am.Makat) 
			INNER JOIN @data d
			ON(am.ShortM = d.Makat)
	WHERE 	fc.IsActive = 1
	AND		g.IsActive = 1
	AND		m.IsActive = 1	
END



