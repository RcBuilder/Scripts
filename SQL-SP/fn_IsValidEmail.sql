USE [ExciteRollTime]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
/*
	SELECT dbo.fn_IsValidEmail('RcBuilder@walla.com')	
	SELECT dbo.fn_IsValidEmail('RcBuilder@walla.co.il')
	SELECT dbo.fn_IsValidEmail('Rc_Builder@walla.co.il')	
	SELECT dbo.fn_IsValidEmail(NULL)
	SELECT dbo.fn_IsValidEmail('RcBuilder@@walla.com')
	SELECT dbo.fn_IsValidEmail('RcBuild er@walla.com')
	SELECT dbo.fn_IsValidEmail('_RcBuilder@walla.com')
	SELECT dbo.fn_IsValidEmail('RcBuilder@walla.com_')
	SELECT dbo.fn_IsValidEmail('RcBuilder@')
	SELECT dbo.fn_IsValidEmail('@walla.com')
*/
-- =============================================
ALTER FUNCTION [dbo].[fn_IsValidEmail]
(
	@Value NVARCHAR(MAX)
)
RETURNS BIT
AS
BEGIN
	-- split email parts (Recipient and Domain)
	DECLARE @temp TABLE(idx INT, val NVARCHAR(MAX)) 
	INSERT INTO @temp
		SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)), [Value] FROM STRING_SPLIT(@Value, '@')
	
	DECLARE @Recipient NVARCHAR(MAX) = (SELECT TOP 1 val FROM @temp WHERE idx = 1), 
			@Domain NVARCHAR(MAX) = (SELECT TOP 1 val FROM @temp WHERE idx = 2)	

	---

	DECLARE @IsValid BIT = 0;

	SELECT	@IsValid = 1 
	FROM	(VALUES(@Value)) T(v) 
	WHERE	(@Domain IS NOT NULL AND @Domain <> '' AND @Recipient IS NOT NULL AND @Recipient <> '') -- must have both email parts
	AND		v NOT LIKE '%@%@%'	-- only one @ sign allowed
	AND		v NOT LIKE '% %'	-- no spaces allowed
	AND		(v NOT LIKE '%[-_.+]' AND v NOT LIKE '[-_.+]%')	 -- no special-char prefix or suffix
	AND		v NOT LIKE '%["(),:;<>\]%'  -- non-allowed characters list for the email
	AND		@Domain NOT LIKE '%[!#$%&*+/=?^`_{|]%'  -- non-allowed characters list for the domain
	AND		CHARINDEX('.', @Domain) > 0  -- domain must include . (gmail.com, walla.co.il etc.) 
	AND		@Recipient LIKE '%[a-zA-Z0-9]%'  -- at least 1 letter or digit

	RETURN @IsValid; 
END


-- SELECT SUBSTRING('Rc_Builder@walla.com', CHARINDEX('@', 'Rc_Builder@walla.com') + 1, 1000)
-- SELECT ROW_NUMBER() OVER(ORDER BY [Value]), [Value] FROM STRING_SPLIT('Rc_Builder@walla.com', '@')
/*
	DECLARE @temp TABLE(idx INT, val NVARCHAR(MAX)) 
	INSERT INTO @temp
		SELECT ROW_NUMBER() OVER(ORDER BY [Value]), [Value] FROM STRING_SPLIT('Rc_Builder@walla.com', '@')
	
	DECLARE @Recipient NVARCHAR(MAX) = (SELECT TOP 1 val FROM @temp WHERE idx = 1), 
			@Domain NVARCHAR(MAX) = (SELECT TOP 1 val FROM @temp WHERE idx = 2)

	SELECT @Recipient, @Domain
*/

/*
	DECLARE @Value NVARCHAR(MAX) = '1@1'

	DECLARE @temp TABLE(idx INT, val NVARCHAR(MAX)) 
	INSERT INTO @temp
		SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)), [Value] FROM STRING_SPLIT(@Value, '@')
	
	select * from @temp

	DECLARE @Recipient NVARCHAR(MAX) = (SELECT TOP 1 val FROM @temp WHERE idx = 1), 
			@Domain NVARCHAR(MAX) = (SELECT TOP 1 val FROM @temp WHERE idx = 2)	

	SELECT @Recipient, @Domain

	DECLARE @IsValid BIT = 0;

	SELECT	1 
	FROM	(VALUES(@Value)) T(v) 
	WHERE	(@Domain IS NOT NULL AND @Domain <> '' AND @Recipient IS NOT NULL AND @Recipient <> '') -- must have both email parts
	AND		v NOT LIKE '%@%@%'	-- only one @ sign allowed
	AND		v NOT LIKE '% %'	-- no spaces allowed
	AND		(v NOT LIKE '%[-_.+]' AND v NOT LIKE '[-_.+]%')	 -- no special-char prefix or suffix
	AND		v NOT LIKE '%["(),:;<>\]%'  -- non-allowed characters list for the email
	AND		@Domain NOT LIKE '%[!#$%&*+/=?^`_{|]%'  -- non-allowed characters list for the domain
	AND		CHARINDEX('.', @Domain) > 0  -- domain must include . (gmail.com, walla.co.il etc.) 
	AND		@Recipient LIKE '%[a-zA-Z0-9]%'  -- at least 1 letter or digit
*/
	