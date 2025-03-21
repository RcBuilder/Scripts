USE [ExciteRollTime]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- SELECT dbo.fn_GetGenderName('ז')
-- SELECT dbo.fn_GetGenderName('נ')
-- SELECT dbo.fn_GetGenderName('א')
-- SELECT dbo.fn_GetGenderName(NULL)
-- =============================================
ALTER FUNCTION fn_GetGenderName
(
	@Gender CHAR(1)
)
RETURNS NVARCHAR(10)
AS
BEGIN
	RETURN 
	CASE(@Gender) 
		WHEN 'ז' THEN N'זכר'
		WHEN 'נ' THEN N'נקבה'
		ELSE N'לא ידוע'
	END
END
