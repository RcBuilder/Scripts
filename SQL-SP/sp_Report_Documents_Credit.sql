SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Description:	<Credit Documents Report>
-- sp_Report_Documents_Credit N'סושיאל'
-- sp_Report_Documents_Credit N'סושיאל', @RowCount = 50
-- sp_Report_Documents_Credit N'סושיאל', @DateFrom = '2023-02-26', @DateTo = '2023-02-26' 

---------------------------------
-- SELECT TOP 10 * FROM V_BILL_R

-- SELECT [Value] FROM STRING_SPLIT('A,B,C,D', ',')
-- SELECT @@VERSION
-- ALTER DATABASE YZ SET COMPATIBILITY_LEVEL = 130
-- SELECT NAME, COMPATIBILITY_LEVEL FROM SYS.DATABASES

---------------------------------

--- ORG > ORG_NAME
--- ARTISTS > ARTIST_NAME
--- BILL > Description

-- CREATE NONCLUSTERED INDEX [IX_ARTIST_NAME] ON [dbo].[ARTISTS] (ARTIST_NAME)
-- CREATE NONCLUSTERED INDEX [IX_ORG_NAME] ON [dbo].[ORG] (ORG_NAME)

-- =============================================
ALTER PROCEDURE sp_Report_Documents_Credit
	@SearchTexts NVARCHAR(300),
	@DateFrom DATE = NULL,  
	@DateTo DATE = NULL,
	@RowCount INT = 1000
AS
BEGIN	
	SET NOCOUNT ON;
	
	-- TIME FILTER -- 
	DECLARE @addTimeFilter bit = 0;
	if(@DateFrom IS NOT NULL AND @DateTo IS NOT NULL)
		set @addTimeFilter = 1;
	
	if(@addTimeFilter = 1) 
	BEGIN
		-- fix dates -- 
		set @DateFrom = CAST((CONVERT(VARCHAR, @DateFrom, 101) + ' 00:00:00') AS DATE)
		set @DateTo = CAST((CONVERT(VARCHAR, @DateTo, 101) + ' 23:59:00') AS DATE)
	END	
	
	PRINT(CONCAT(@DateFrom, ' - ', @DateTo))

	SELECT	DISTINCT TOP(@RowCount) 
			BILL_R_ID AS '#',
			ARTIST_NAME AS 'שם אומן', 
			ORG_NAME AS 'שם מוסד',
			[Description] AS 'תיאור',
			total AS 'סכום לא כולל מע"מ',			
			BILL_DATE AS 'תאריך',
			bill_m_id AS 'חשבונית מס'			
	FROM	V_BILL_R 
			CROSS APPLY (
				SELECT TRIM([Value]) FROM STRING_SPLIT(@SearchTexts, ',')
			) T(SearchText)
	WHERE	
	(
			T.SearchText = ''
			OR
			ORG_NAME LIKE '%' + T.SearchText + '%'
			OR 
			ARTIST_NAME LIKE '%' + T.SearchText + '%'
			OR 
			[Description] LIKE '%' + T.SearchText + '%'
	) 	
	AND		
	(
			@addTimeFilter = 0 
			OR 
			(BILL_DATE BETWEEN @DateFrom AND @DateTo)
		)		
	ORDER BY bill_m_id DESC

END
GO