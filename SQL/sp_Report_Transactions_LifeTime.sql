GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-02-01>
-- =============================================
CREATE PROCEDURE [dbo].[sp_Report_Transactions_LifeTime]	
AS
BEGIN	
	SET NOCOUNT ON;

	SELECT 
	Id,
	[Name],
	ISNULL([11-2020], '') AS [11-2020],
	ISNULL([12-2020], '') AS [12-2020],
	ISNULL([01-2021], '') AS [01-2021],
	ISNULL([02-2021], '') AS [02-2021], 
	ISNULL([03-2021], '') AS [03-2021], 
	ISNULL([04-2021], '') AS [04-2021], 
	ISNULL([05-2021], '') AS [05-2021], 
	ISNULL([06-2021], '') AS [06-2021], 
	ISNULL([07-2021], '') AS [07-2021], 
	ISNULL([08-2021], '') AS [08-2021], 		
	ISNULL([09-2021], '') AS [09-2021], 
	ISNULL([10-2021], '') AS [10-2021], 
	ISNULL([11-2021], '') AS [11-2021], 
	ISNULL([12-2021], '') AS [12-2021]
FROM (
	SELECT	S.Id, 	
			CONCAT(S.FirstName, ' ', S.LastName) AS 'Name',
			STUFF(CONVERT(VARCHAR, T.CreatedDate, 105), 1, 3, '') AS 'Date', 
			'YES' AS 'Value'
	FROM	[dbo].[Transactions] T 
			INNER JOIN 
			[dbo].[Subscriptions] S
			ON(T.SubscriptionId = S.Id)
	WHERE	PackageId = 2	
) T
PIVOT(
	MIN([Value]) -- MUST have some aggregation function
	FOR [Date] IN (		
		[11-2020],
		[12-2020],
		[01-2021],
		[02-2021], 
		[03-2021], 
		[04-2021], 
		[05-2021], 
		[06-2021], 
		[07-2021], 
		[08-2021], 		
		[09-2021], 
		[10-2021], 
		[11-2021], 
		[12-2021]
	)
) AS Pvt

END