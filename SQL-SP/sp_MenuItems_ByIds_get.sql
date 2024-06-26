USE [MNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_MenuItems_ByIds_get]    Script Date: 10/03/2021 14:21:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-03-07>
-- sp_MenuItems_ByIds_get '1,2,3'
-- =============================================
ALTER PROCEDURE [dbo].[sp_MenuItems_ByIds_get]
	@sIds VARCHAR(300) -- ids comma separated (',')
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * 
	FROM	[dbo].[MenuItems] WITH(NOLOCK)
	WHERE	Id IN (SELECT [value] FROM STRING_SPLIT(@sIds, ','))
	AND		IsDeleted = 0

END
