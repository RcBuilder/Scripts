USE [MemorialDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_Post_Save]    Script Date: 25/10/2022 12:08:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- =============================================
ALTER PROCEDURE [dbo].[sp_Post_Save]
	@Id INT, -- 0 = NEW 
	@AccountId VARCHAR(15), 
	@Title NVARCHAR(500), 
	@Body NVARCHAR(MAX),
	@AuthorName NVARCHAR(500),
	@sTags NVARCHAR(MAX)
AS
BEGIN	
	SET NOCOUNT ON;    
	
	IF(@Id <= 0) -- NEW --
	BEGIN				
		INSERT INTO [dbo].[Posts]
		(AccountId, Title)
		VALUES
		(@AccountId, @Title)

		SET @Id = SCOPE_IDENTITY()
	END 

	-- save post details 
	UPDATE	[dbo].[Posts]
	SET		UpdatedDate = GETDATE(),				
			Title = @Title,
			Body = @Body,
			AuthorName = @AuthorName
	WHERE	Id = @Id

	-- save post tags 
	EXEC sp_Post_Tags_Save @Id, @sTags

	SELECT @Id AS 'Res'
END