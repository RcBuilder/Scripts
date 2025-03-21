USE [GF_db]
GO
/****** Object:  StoredProcedure [dbo].[FindGurByMakats]    Script Date: 24/10/2024 12:20:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>

/*
	DECLARE @FilterMatch AS dbo.FilterMatch -- (RowIndex INT, Makat NVARCHAR(MAX)) 
	INSERT INTO @FilterMatch VALUES(1, 'GFI81700'), (2, 'A3125H')
	EXEC FindManByMakats @FilterMatch
*/

-- GFI81700 > BFU700x
-- SELECT Makat, ID FROM [dbo].[GurFilters] WHERE Makat = 'GFI81700' -- 3871
-- SELECT ManID, GurID FROM [dbo].[FilterConnections] WHERE GurID = 3871
-- SELECT Makat FROM ManFilters WHERE ID = 159532
-- =============================================
ALTER PROCEDURE [dbo].[FindManByMakats]
	@data FilterMatch readonly
AS
BEGIN	
	SET NOCOUNT ON;
	

	SELECT	DISTINCT d.RowIndex, m.Makat
	FROM	FilterConnections fc 
			INNER JOIN 
			GurFilters g
			ON(g.ID = fc.GurID) 
			INNER JOIN 
			ManFilters m
			ON(m.ID = fc.ManID)
			INNER JOIN 
			AllMakats am 				
			ON(g.Makat = am.Makat) 
			INNER JOIN @data d
			ON(am.ShortM = d.Makat)
	WHERE 	fc.IsActive = 1
	AND		g.IsActive = 1
	AND		m.IsActive = 1	
END