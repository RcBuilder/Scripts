USE [MemorialEvents]
GO
/****** Object:  StoredProcedure [dbo].[sp_Dashboard_Data_Get]    Script Date: 04/09/2023 11:59:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- sp_Dashboard_Data_Get '1000'
-- select top 50 * from Accounts order by CreatedDate desc
-- =============================================
ALTER PROCEDURE [dbo].[sp_Dashboard_Data_Get]
	@AccountId VARCHAR(15)
AS
BEGIN	
	SET NOCOUNT ON;    
	
	DECLARE @PostCount INT = 
	(	
		SELECT	COUNT(Id)
		FROM	[dbo].[Posts] WITH(NOLOCK)	
		WHERE	AccountId = @AccountId
		AND		IsDeleted = 0
	)	
		
	DECLARE @PostIds VARCHAR(MAX) = (
		SELECT STUFF((
			SELECT	CONCAT(',', Id)
			FROM	[dbo].[Posts] WITH(NOLOCK)	
			WHERE	AccountId = @AccountId
			AND		IsDeleted = 0
			ORDER BY UpdatedDate DESC
			FOR XML PATH('')
		), 1, 1, '')
	)
	
	DECLARE @GuestCount INT = 
	(	
		SELECT	COUNT(RowId)
		FROM	[dbo].[GuestList] WITH(NOLOCK)	
		WHERE	AccountId = @AccountId		
	)	

	DECLARE @TODAY DATE = GETDATE()
	DECLARE @DayCount INT = 
	(
		SELECT	DATEDIFF(day, @TODAY, [Date])
		FROM	[dbo].[Accounts] WITH(NOLOCK)	
		WHERE	Id = @AccountId		
	)

	SELECT	@PostCount AS 'PostCount',
			@PostIds AS 'PostIds',
			@GuestCount AS 'GuestCount', 
			@DayCount AS 'DayCount'

END
