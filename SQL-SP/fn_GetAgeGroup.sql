USE [ExciteRollTime]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetAgeGroup]    Script Date: 07/08/2021 19:32:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- SELECT dbo.fn_GetAgeGroup(28)
-- SELECT dbo.fn_GetAgeGroup(NULL)
-- =============================================
ALTER FUNCTION [dbo].[fn_GetAgeGroup]
(
	@Age INT
)
RETURNS VARCHAR(10)
AS
BEGIN
	
	RETURN CASE 
		WHEN @Age < 20 THEN 'Below 20'
		WHEN @Age <= 30 THEN '20-30'
		WHEN @Age <= 40 THEN '30-40'
		WHEN @Age <= 50 THEN '40-50'
		WHEN @Age <= 60 THEN '50-60'
		WHEN @Age > 60 THEN 'Above 60'		
		ELSE 'No Age'
	END

END
