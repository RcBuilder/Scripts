USE [MNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_Deals_ByRestaurant_get]    Script Date: 10/03/2021 14:12:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-03-07>
-- sp_Deals_ByRestaurant_get 1
-- =============================================
ALTER PROCEDURE [dbo].[sp_Deals_ByRestaurant_get]
	@RestaurantId INT
AS
BEGIN
	SET NOCOUNT ON;

	-- find relevant deal ids --	
	DECLARE @DealIds TABLE(Id INT) 
	INSERT INTO @DealIds
		SELECT	Id 
		FROM	[dbo].[Deals] WITH(NOLOCK)
		WHERE	[RestaurantId] = @RestaurantId
		AND		IsDeleted = 0

    -- 1st table: deals + deals-items map --
	SELECT	*,
			ISNULL(STUFF((SELECT ',' + CAST(MenuItemId AS VARCHAR(5)) FROM [dbo].[DealItems] WHERE DealId = Id FOR XML PATH('')), 1, 1, ''), '') AS 'sItems'
	FROM	[dbo].[Deals] WITH(NOLOCK)
	WHERE	Id IN (SELECT Id FROM @DealIds)

	/*
	-- 2nd table: deals-items map --
	SELECT	* 
	FROM	[dbo].[DealItems] WITH(NOLOCK)			
	WHERE	DealId IN (SELECT Id FROM @DealIds) 
	*/

END
