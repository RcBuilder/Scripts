USE [TegrityUpdates]
GO
/****** Object:  UserDefinedFunction [dbo].[Split]    Script Date: 2/24/2016 5:04:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Roby>
-- Create date: <24/02/2016>
-- Description:	<split>
-- Demo : select Value from Split('200,400,500,202,122',DEFAULT)
-- =============================================
ALTER FUNCTION [dbo].[Split] 
(	
	@String nvarchar (MAX),
	@Delimiter nvarchar (5) = ',' -- use the keyword 'DEFAULT' in order to use the default parameter 
)
RETURNS @ValueTable table ([Value] nvarchar(50))
AS 
BEGIN
	 if(RTRIM(LTRIM(@String)) = '') 
		return

	 declare @NextString nvarchar(50) = '', 
			 @Pos int, 
			 @NextPos int;

	 set @String = @String + @Delimiter
	 
	 set @Pos = charindex(@Delimiter, @String)
	 set @NextPos = 1
	 
	 --Loop while there is still a comma in the String of levels
	 while (@pos <>  0)  
	 BEGIN
		  set @NextString = RTRIM(LTRIM(substring(@String, 1, @Pos - 1)))	 
		  insert into @ValueTable ([Value]) Values (@NextString)	
		   
		  set @String = substring(@String, @pos + 1, len(@String))		  
		  set @NextPos = @Pos
		  set @pos  = charindex(@Delimiter, @String)
	 END 
	 
	 RETURN
END	  