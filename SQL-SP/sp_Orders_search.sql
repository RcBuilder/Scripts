USE [MNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_Orders_search]    Script Date: 13/05/2022 12:21:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-04-08>
/* 
	sp_Orders_search 
		@RowCount = 9, 
		@sStatuses='1,2,3',
		@OrderBy = 'OrderDateASC', 
		@sDepartments = '3'
*/
-- =============================================
ALTER PROCEDURE [dbo].[sp_Orders_search]	
	@RowCount INT = 9,
	@RestaurantId INT = 0,	
	@sStatuses VARCHAR(50) = '',  -- statuses comma separated (',') 	
	@sPaymentTypes VARCHAR(50) = '',  -- payment-types comma separated (',') 
	@IsPaid BIT = NULL,
	@OrderBy VARCHAR(20) = '',
	@FreeText NVARCHAR(100) = '',
	@sDepartments VARCHAR(50) = '',
	@OnlyTables BIT = 0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @tStatuses TABLE(Id INT)
	INSERT INTO @tStatuses(Id)
		SELECT [value] FROM STRING_SPLIT(@sStatuses, ',')	

	DECLARE @tPaymentTypes TABLE(Id INT)
	INSERT INTO @tPaymentTypes(Id)
		SELECT [value] FROM STRING_SPLIT(@sPaymentTypes, ',')

	DECLARE @tDepartments TABLE(Id INT)
	INSERT INTO @tDepartments(Id)
		SELECT [value] FROM STRING_SPLIT(@sDepartments, ',')

	-- 1st phase - base search --
	DECLARE @tOrderIds TABLE(Id INT)
	INSERT INTO @tOrderIds(Id)
		SELECT	Id
		FROM	[dbo].[Orders] WITH(NOLOCK)		
		WHERE	IsDeleted = 0
		AND		(@RestaurantId = 0 OR RestaurantId = @RestaurantId)
		AND		(ISNULL(@sStatuses, '') = '' OR [Status] IN (SELECT Id FROM @tStatuses))	
		AND		(ISNULL(@sPaymentTypes, '') = '' OR [PaymentType] IN (SELECT Id FROM @tPaymentTypes))
		AND		(@IsPaid IS NULL OR IsPaid = @IsPaid)
		AND		(@OnlyTables = 0 OR TableNumber > 0)
		AND		(
					@FreeText = '' 
					OR 
					ClientName LIKE '%' + @FreeText + '%'
					OR 
					ClientPhone LIKE '%' + @FreeText + '%'
					OR 				
					NameOnReceipt LIKE '%' + @FreeText + '%'
				)

	-- create order-departments mapping table -- 
	DECLARE @tDepartmentsMap TABLE(OrderId INT, DepartmentId INT)
	INSERT INTO @tDepartmentsMap(OrderId, DepartmentId)
		SELECT	DISTINCT R.OrderId, D.Id 
		FROM	[dbo].[OrderRows] R WITH(NOLOCK)
				INNER JOIN
				[dbo].[MenuItems] M WITH(NOLOCK)
				ON(M.Id = R.ItemId)
				INNER JOIN
				[dbo].[Departments] D WITH(NOLOCK)
				ON(D.Id = M.DepartmentId)
		WHERE	R.OrderId IN (SELECT Id FROM @tOrderIds)

	-- 2nd phase - filter by departments --
	IF(@sDepartments <> '')
	BEGIN 
		DELETE FROM @tOrderIds
		WHERE Id NOT IN(
			SELECT OrderId FROM @tDepartmentsMap WHERE DepartmentId IN (SELECT Id FROM @tDepartments)
		)
	END 

	-- 3rd phase - rowcount and sort --
	SELECT	TOP(@RowCount) Id
	FROM	[dbo].[Orders] WITH(NOLOCK)		
	WHERE	Id IN (SELECT Id FROM @tOrderIds)		
	ORDER BY 
		CASE WHEN @OrderBy = '' THEN OrderDate END DESC,
		CASE WHEN @OrderBy = 'OrderDateDESC' THEN OrderDate END DESC,		
		CASE WHEN @OrderBy = 'OrderDateASC' THEN OrderDate END ASC,
		CASE WHEN @OrderBy = 'StatusDESC' THEN [Status] END DESC,		
		CASE WHEN @OrderBy = 'StatusASC' THEN [Status] END ASC
END
