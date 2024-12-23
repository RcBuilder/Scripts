USE [MochileroDB]
GO
/****** Object:  UserDefinedFunction [dbo].[CalculateDistance]    Script Date: 06/11/2010 02:13:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <11/06/2010>
-- Description:	<calculate distance in KM between 2 coordinates>
/* 
	-- Paris '2.3509871 48.8566667'
	-- Lyon '4.8343287 45.767299'
	-- Tel Aviv '34.770 32.070'
	-- Netanya '34.860 32.330'
	-- Elat '34.950 29.560'
	select dbo.CalculateDistanceFromSPoints('34.770 32.070','34.860 32.330')
*/
-- =============================================
create FUNCTION [dbo].[CalculateDistanceFromSPoints]
(
	@sPoint1 varchar(50) = '', -- 'Lng Lat' --
	@sPoint2 varchar(50) = '' -- 'Lng Lat' --
)
RETURNS FLOAT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @distance float
	SET @distance = 0 -- default --
	
	if(@sPoint1 = '' or @sPoint2 = '') 
		RETURN @distance
	
	DECLARE @point1 GEOGRAPHY = 'POINT('+@sPoint1+')'
	DECLARE @point2 GEOGRAPHY = 'POINT('+@sPoint2+')'

	SET @distance = @point1.STDistance(@point2) / 1000.0
	
	RETURN @distance
END
