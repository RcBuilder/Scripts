SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
/* 
	Description: 
	Replace The IIF Built-In Function for BIT type with INT result
	IIF exists since SQL-2012 and up, this patch is for lower versions
	IIF(@BitValue = 1, 5, -5) -> dbo.fn_IIF_BIT(@BitValue, 1, 5, -5)
*/

-- select dbo.fn_IIF_BIT(1, 1, 1, 0)
-- select dbo.fn_IIF_BIT(1, 0, 1, 0)
-- =============================================
ALTER FUNCTION [dbo].[fn_IIF_BIT]
(
	@value BIT,
	@ValueToMatch BIT = 1, 
	@ValueIfTrue INT = 1, 
	@ValueIfFalse INT = 0 
)
RETURNS INT
AS
BEGIN
	RETURN CASE @value WHEN @ValueToMatch THEN @ValueIfTrue ELSE @ValueIfFalse END
END