USE [MemorialEvents]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- sp_Manager_Delete_FULL 'rcbuilder@gmail.com'
-- =============================================
CREATE PROCEDURE [dbo].[sp_Manager_Delete_FULL]
	@Email NVARCHAR(250)	
AS
BEGIN	
	SET NOCOUNT OFF;    
	
	DECLARE @Ids TABLE(Id VARCHAR(15)); 
	INSERT INTO @Ids SELECT AccountId FROM ManagersAccounts WHERE [Owner] = @Email
	SELECT Id FROM @Ids
	-- SELECT * FROM Accounts WHERE Id IN (SELECT Id FROM @Ids)

	DELETE FROM Managers WHERE Email = @Email
	DELETE FROM ManagersAccounts WHERE Email = @Email -- manager watch permissions

	DELETE FROM ManagersAccounts WHERE AccountId IN (SELECT Id FROM @Ids) -- manager accounts
	DELETE FROM Accounts WHERE Id IN (SELECT Id FROM @Ids)
	DELETE FROM GuestList WHERE AccountId IN (SELECT Id FROM @Ids)	
	DELETE FROM Posts WHERE AccountId IN (SELECT Id FROM @Ids)
	DELETE FROM PostsDraft WHERE AccountId IN (SELECT Id FROM @Ids)
	DELETE FROM Questionnaire WHERE AccountId IN (SELECT Id FROM @Ids)
	DELETE FROM Settings WHERE AccountId IN (SELECT Id FROM @Ids)
END