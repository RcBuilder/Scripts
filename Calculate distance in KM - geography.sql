USE [MochileroDB]
GO
/****** Object:  UserDefinedFunction [dbo].[CalculateDistance]    Script Date: 06/11/2010 02:11:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <11/06/2010>
-- Description:	<calculate distance in KM between 2 coordinates>
/* 
	select dbo.CalculateDistance_('POINT(34.770 32.070)','POINT(34.860 32.330)')
*/
-- =============================================
alter FUNCTION [dbo].[CalculateDistance]
(
	@Point1 GEOGRAPHY = null, 
	@Point2 GEOGRAPHY = null 
)
RETURNS FLOAT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @distance float
	SET @distance = 0 -- default --
	
	if(@Point1 is null or @Point2 is null) 
		RETURN @distance
	SET @distance = @point1.STDistance(@point2) / 1000.0
	RETURN @distance
END
