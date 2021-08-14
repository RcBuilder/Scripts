
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-07-07>
-- SELECT dbo.fn_GetAgeFromBirthDate('2000-01-01')
-- SELECT dbo.fn_GetAgeFromBirthDate(NULL)
-- =============================================
CREATE FUNCTION fn_GetAgeFromBirthDate
(
	@BirthDate DATE
)
RETURNS INT
AS
BEGIN
	DECLARE @Today DATETIME = GETDATE()
	RETURN DATEDIFF(YEAR, @BirthDate, @Today)
END
GO

