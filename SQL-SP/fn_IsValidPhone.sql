USE [ExciteRollTime]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_IsValidPhone]    Script Date: 06/08/2021 10:47:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
/*
	SELECT dbo.fn_IsValidPhone('0545614020')
	SELECT dbo.fn_IsValidPhone('054-5614020')
	SELECT dbo.fn_IsValidPhone('054-561-4020')
	SELECT dbo.fn_IsValidPhone('+972-54-561-4020')
	SELECT dbo.fn_IsValidPhone('+972 54-561-4020')
	SELECT dbo.fn_IsValidPhone('+972545614020')
	SELECT dbo.fn_IsValidPhone(NULL)
	SELECT dbo.fn_IsValidPhone(077-1234567)
*/
-- =============================================
ALTER FUNCTION [dbo].[fn_IsValidPhone]
(
	@Value NVARCHAR(MAX)
)
RETURNS BIT
AS
BEGIN
	
	DECLARE @IsValid BIT = 0;

	SET @Value = REPLACE(@Value, ' ', '')  -- remove spaces

	SELECT	@IsValid = 1 
	FROM	(VALUES(@Value)) T(v) 
	WHERE	(LEFT(@Value, 2) = '05' OR LEFT(@Value, 4) = '+972')  -- starts with 05 or +972
	AND		(LEN(@Value) BETWEEN 10 AND 16)  -- length allowed range
	AND		@Value NOT LIKE '%[^0-9\+\-]%'  -- allowed characters list (using NOT over ^)

	RETURN @IsValid; 
END