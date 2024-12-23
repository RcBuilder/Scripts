USE [Jobs]
GO
/****** Object:  StoredProcedure [dbo].[SP_AUCTIONS_PROD_QA_MERGE]    Script Date: 18/02/2019 9:52:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Roby>
-- Create date: <2019-02-17>
-- Description:	<merge Auctions PROD > QA>
-- =============================================
ALTER PROCEDURE [dbo].[SP_AUCTIONS_PROD_QA_MERGE]
AS
BEGIN	
	SET NOCOUNT ON;
		
	MERGE [MACrawlerQA].[dbo].[Auctions] AS TARGET
	USING [MACrawler_].[dbo].[Auctions] AS SOURCE 
	ON (
		TARGET.Id = SOURCE.Id 
		AND TARGET.SourceId = SOURCE.SourceId 
		AND SOURCE.[Date] > '2019-01-01'
	) 

	WHEN MATCHED THEN 
		UPDATE SET 	
		TARGET.[SourceId] = SOURCE.[SourceId], 
		TARGET.[Id] = SOURCE.[Id], 
		TARGET.[MA_Id] = NULL, 
		TARGET.[UpdatedDate] = SOURCE.[UpdatedDate], 
		TARGET.[Title] = SOURCE.[Title], 
		TARGET.[DateRaw] = SOURCE.[DateRaw], 
		TARGET.[Date] = SOURCE.[Date], 
		TARGET.[DateUntilRaw] = SOURCE.[DateUntilRaw], 
		TARGET.[DateUntil] = SOURCE.[DateUntil], 
		TARGET.[URL] = SOURCE.[URL], 
		TARGET.[PhotoRaw] = SOURCE.[PhotoRaw], 
		TARGET.[Photo] = SOURCE.[Photo], 
		TARGET.[IsCompleted] = SOURCE.[IsCompleted], 
		TARGET.[Category] = SOURCE.[Category], 
		TARGET.[Location] = SOURCE.[Location], 
		TARGET.[SaleNumber] = SOURCE.[SaleNumber], 
		TARGET.[ViewingTimes] = SOURCE.[ViewingTimes], 
		TARGET.[PremiumRates] = SOURCE.[PremiumRates]
	
	WHEN NOT MATCHED BY TARGET THEN 
		INSERT 
		VALUES (
			SOURCE.[SourceId], 
			SOURCE.[Id], 
			NULL, 
			SOURCE.[UpdatedDate], 
			SOURCE.[Title], 
			SOURCE.[DateRaw], 
			SOURCE.[Date], 
			SOURCE.[DateUntilRaw], 
			SOURCE.[DateUntil], 
			SOURCE.[URL], 
			SOURCE.[PhotoRaw], 
			SOURCE.[Photo], 
			SOURCE.[IsCompleted], 
			SOURCE.[Category], 
			SOURCE.[Location], 
			SOURCE.[SaleNumber], 
			SOURCE.[ViewingTimes], 
			SOURCE.[PremiumRates]
		);

	--WHEN NOT MATCHED BY SOURCE THEN 
		--DELETE;

	PRINT('Auctions MERGED - MACrawler_ > MACrawlerQA')

END
