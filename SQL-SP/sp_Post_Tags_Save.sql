USE [MemorialDB]
GO
/****** Object:  StoredProcedure [dbo].[sp_Post_Tags_Save]    Script Date: 22/10/2022 18:47:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- sp_Post_Tags_Save 1, 'Friends, Family, Colleagues'		
-- =============================================
ALTER PROCEDURE [dbo].[sp_Post_Tags_Save]	
	@Id INT,    
	@Tags NVARCHAR(MAX) -- splitted by ','   
AS
BEGIN	
	SET NOCOUNT ON;    
	
	BEGIN TRANSACTION
	BEGIN TRY
	
		SET @Tags = RTRIM(LTRIM(@Tags)) -- trim root --

		-- delete old tags -- 
		DELETE FROM PostsTags WHERE PostId = @Id

		-- insert new tags --
		IF(@Tags <> '')
		BEGIN
			INSERT INTO PostsTags 
				SELECT @Id, RTRIM(LTRIM([Value])) FROM STRING_SPLIT(@Tags, ',')
		END 

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH		 
		PRINT(ERROR_MESSAGE())	     		
		ROLLBACK TRANSACTION 
	END CATCH 
END