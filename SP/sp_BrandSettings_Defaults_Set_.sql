-- ==========================================================
-- Create Stored Procedure Template for SQL Azure Database
-- ==========================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Roby>
-- Create date: <2020-04-30>
-- sp_BrandSettings_Defaults_Set '1efa31b1-9b04-4aef-aa58-8453a0a90ed4'

/*
	DECLARE @UserId NVARCHAR(128) = '1efa31b1-9b04-4aef-aa58-8453a0a90ed4'
	DECLARE @BrandId BIGINT = (SELECT TOP 1 Id FROM [dbo].[BrandSettings] WHERE UserId = @UserId)	

	DELETE FROM [WidgetToTags] WHERE WidgetId in (SELECT Id FROM widgets WHERE UserId = @UserId)
	DELETE FROM [Widgets] WHERE UserId = @UserId  
	DELETE FROM [TrafficFilters] WHERE BrandId = @BrandId
	DELETE FROM [Groups] WHERE BrandId = @BrandId
	DELETE FROM [Tags] WHERE UserId = @UserId  
	DELETE FROM [UrlMaps] WHERE UserId = @UserId 
	DELETE FROM [Contents] WHERE Id = (SELECT TOP 1 ContentId FROM UrlMaps WHERE UserId = @UserId)

	---

	DECLARE @UserId NVARCHAR(128) = '1efa31b1-9b04-4aef-aa58-8453a0a90ed4'
	DECLARE @BrandId BIGINT = (SELECT TOP 1 Id FROM [dbo].[BrandSettings] WHERE UserId = @UserId)

	SELECT * FROM widgets WHERE UserId = @UserId
	SELECT * FROM TrafficFilters WHERE BrandId = @BrandId
	SELECT * FROM Groups WHERE BrandId = @BrandId
	SELECT * FROM Tags WHERE UserId = @UserId
	SELECT * FROM UrlMaps WHERE UserId = @UserId
	SELECT * FROM Contents WHERE Id = (SELECT TOP 1 ContentId FROM UrlMaps WHERE UserId = @UserId)
*/
-- =============================================
ALTER PROCEDURE sp_BrandSettings_Defaults_Set
	@UserId NVARCHAR(128)
AS
BEGIN	
	SET NOCOUNT ON;
    
	DECLARE @BrandId BIGINT, @BrandName NVARCHAR(256)	
	SELECT TOP 1 @BrandId = Id, @BrandName = Name FROM [dbo].[BrandSettings] WHERE UserId = @UserId

	DECLARE @META_DATA XML = 
		'<Settings>
			<TrafficFilters>
				<TrafficFilter>
					<Name>{{BrandName}} Bot Filter</Name>
					<Action>2</Action>
					<Type>2</Type>
				</TrafficFilter>
			</TrafficFilters>
			<Groups>
				<Group>
					<Name>Contact Us</Name>
					<Type>1</Type>
					<IncludeInDaVinci>0</IncludeInDaVinci>
				</Group>
				<Group>
					<Name>Request a demo</Name>
					<Type>1</Type>
					<IncludeInDaVinci>0</IncludeInDaVinci>
				</Group>
				<Group>
					<Name>Subscribe to newsletter</Name>
					<Type>1</Type>
					<IncludeInDaVinci>0</IncludeInDaVinci>
				</Group>
			</Groups>
			<Tags>
				<Tag>
					<Name>General</Name>
					<Type>1</Type>
					<IsDefault>false</IsDefault>
				</Tag>
			</Tags>
			<Posts>
				<Post>
					<OriginalUrl>https://www.cliclap.com/wp-content/uploads/2020/01/The-PDF-dead-or-on-the-verge-of-a-content-marketing-renaissance.pdf</OriginalUrl>
					<HostWithoutTld>cliclap</HostWithoutTld>
					<Host>cliclap.com</Host>					
					<ContentId>966u15</ContentId>
					<ContentType>pdf</ContentType>
					<ViewPermissionId>3</ViewPermissionId>
					<IsManuallyCreated>1</IsManuallyCreated>					
				</Post>				
			</Posts>
			<Widgets>
				<Widget>				
					<Name>Blank Widget</Name>
					<TypeId>1</TypeId>
					<MetaData>{}</MetaData>
					<IsArchived>0</IsArchived>
					<IsDisplayable>0</IsDisplayable>
					<ContentTypeId>NULL</ContentTypeId>
					<CtaUrl>NULL</CtaUrl>
					<CtaUrlContentId>NULL</CtaUrlContentId>
					<GroupId>NULL</GroupId>
					<IsValid>1</IsValid>
					<isDraft>0</isDraft>				
				</Widget>
				<Widget>				
					<Name>Example 1</Name>
					<TypeId>7</TypeId>
					<MetaData>{"content_text":"This is an example for a content recommendation widget","content_image_url":"https://cliclapstoragewesteu.blob.core.windows.net/brandsettingsimagesblob/af5ffb05-104c-4064-a190-602b68860eed_sZlAocDjNb.Png","ImagesUrls":["https://www.cliclap.com/wp-content/uploads/2020/04/Lead_Generation_FB_New.jpg","https://www.cliclap.com/wp-content/uploads/2020/04/bg_blue_new.png","https://cliclapstoragewesteu.blob.core.windows.net/brandsettingsimagesblob/af5ffb05-104c-4064-a190-602b68860eed_sZlAocDjNb.Png"],"content_url":"https://www.cliclap.com/lead-generation/","cta_text":"This is a CTA","widget_title":"[Example 1] This is an example for a widget"}</MetaData>
					<IsArchived>0</IsArchived>
					<IsDisplayable>1</IsDisplayable>
					<ContentTypeId>NULL</ContentTypeId>
					<CtaUrl>https://www.cliclap.com/lead-generation/</CtaUrl>
					<CtaUrlContentId>1epp90h</CtaUrlContentId>
					<GroupId>NULL</GroupId>
					<IsValid>1</IsValid>
					<isDraft>0</isDraft>				
				</Widget>	
				<Widget>				
					<Name>Example 2</Name>
					<TypeId>7</TypeId>
					<MetaData>{"content_text":"This is an example for a content recommendation widget","content_image_url":"https://cliclapstoragewesteu.blob.core.windows.net/brandsettingsimagesblob/af5ffb05-104c-4064-a190-602b68860eed_RHhZGe4Lii.Png","ImagesUrls":["https://www.cliclap.com/wp-content/uploads/2020/04/Content_manager_FB.jpg","https://www.cliclap.com/wp-content/uploads/2020/04/bg_yellow_new.png","https://cliclapstoragewesteu.blob.core.windows.net/brandsettingsimagesblob/af5ffb05-104c-4064-a190-602b68860eed_RHhZGe4Lii.Png"],"content_url":"https://www.cliclap.com/content-manager/","cta_text":"This is a CTA","widget_title":"[Example 2] This is an example for a widget"}</MetaData>
					<IsArchived>0</IsArchived>
					<IsDisplayable>1</IsDisplayable>
					<ContentTypeId>NULL</ContentTypeId>
					<CtaUrl>https://www.cliclap.com/content-manager/</CtaUrl>
					<CtaUrlContentId>1hx6uvx</CtaUrlContentId>
					<GroupId>NULL</GroupId>
					<IsValid>1</IsValid>
					<isDraft>0</isDraft>				
				</Widget>	
				<Widget>				
					<Name>Example 3</Name>
					<TypeId>7</TypeId>
					<MetaData>{"content_text":"This is an example for a content recommendation widget","content_image_url":"https://cliclapstoragewesteu.blob.core.windows.net/brandsettingsimagesblob/af5ffb05-104c-4064-a190-602b68860eed_QHVXwOluDP.Png","ImagesUrls":["https://www.cliclap.com/wp-content/uploads/2020/04/Marketing_Operations_FB.jpg","https://www.cliclap.com/wp-content/uploads/2020/04/bg_purple_new.png","https://cliclapstoragewesteu.blob.core.windows.net/brandsettingsimagesblob/af5ffb05-104c-4064-a190-602b68860eed_QHVXwOluDP.Png"],"content_url":"https://www.cliclap.com/marketing-operations/","cta_text":"This is a CTA","widget_title":"[Example 3] This is an example for a widget"}</MetaData>
					<IsArchived>0</IsArchived>
					<IsDisplayable>1</IsDisplayable>
					<ContentTypeId>NULL</ContentTypeId>
					<CtaUrl>https://www.cliclap.com/marketing-operations/</CtaUrl>
					<CtaUrlContentId>164lc0v</CtaUrlContentId>
					<GroupId>NULL</GroupId>
					<IsValid>1</IsValid>
					<isDraft>0</isDraft>				
				</Widget>
				<Widget>				
					<Name>Example 4</Name>
					<TypeId>7</TypeId>
					<MetaData>{"content_text":"This is an example for a content recommendation widget","content_image_url":"https://cliclapstoragewesteu.blob.core.windows.net/brandsettingsimagesblob/af5ffb05-104c-4064-a190-602b68860eed_jnhEkbLOs5.Png","ImagesUrls":["https://www.cliclap.com/wp-content/uploads/2020/04/Marketing_Management_FB.jpg","https://www.cliclap.com/wp-content/uploads/2020/04/bg_green_new.png","https://cliclapstoragewesteu.blob.core.windows.net/brandsettingsimagesblob/af5ffb05-104c-4064-a190-602b68860eed_jnhEkbLOs5.Png"],"content_url":"https://www.cliclap.com/marketing-management/","cta_text":"This is a CTA","widget_title":"[Example 4] This is an example for a widget"}</MetaData>
					<IsArchived>0</IsArchived>
					<IsDisplayable>1</IsDisplayable>
					<ContentTypeId>NULL</ContentTypeId>
					<CtaUrl>https://www.cliclap.com/marketing-management/</CtaUrl>
					<CtaUrlContentId>5o2oyu</CtaUrlContentId>
					<GroupId>NULL</GroupId>
					<IsValid>1</IsValid>
					<isDraft>0</isDraft>				
				</Widget>
				<Widget>				
					<Name>Example of ClClap''s video widget</Name>
					<TypeId>8</TypeId>
					<MetaData>{"video_text":"","video_image_url":"https://cliclapstoragewesteu.blob.core.windows.net/brandsettingsimagesblob/af5ffb05-104c-4064-a190-602b68860eed_KkRzNpNW0k.Png","ImagesUrls":["https://i.ytimg.com/vi/-GaU59rjbPY/hqdefault.jpg","https://i.ytimg.com/vi/-GaU59rjbPY/default.jpg","https://cliclapstoragewesteu.blob.core.windows.net/brandsettingsimagesblob/af5ffb05-104c-4064-a190-602b68860eed_XbIxQqEF0V.Png","https://cliclapstoragewesteu.blob.core.windows.net/brandsettingsimagesblob/af5ffb05-104c-4064-a190-602b68860eed_KkRzNpNW0k.Png"],"video_url":"https://www.youtube.com/embed/-GaU59rjbPY","video_type":"youtube","cta_text":"Click to watch","widget_title":"Example of ClClap''s video widget"}</MetaData>
					<IsArchived>0</IsArchived>
					<IsDisplayable>1</IsDisplayable>
					<ContentTypeId>NULL</ContentTypeId>
					<CtaUrl>https://www.youtube.com/embed/-GaU59rjbPY</CtaUrl>
					<CtaUrlContentId>141tdss</CtaUrlContentId>
					<GroupId>NULL</GroupId>
					<IsValid>1</IsValid>
					<isDraft>0</isDraft>				
				</Widget>	
				<Widget>				
					<Name>Request a Demo</Name>
					<TypeId>100</TypeId>
					<MetaData><![CDATA[{"ctaUrl":"https://www.dummy.com/Mailchimp/9fbe5701dcf71991febe2619b/0299234ee6","description":"","integration":{"name":"Mailchimp","script":"<!-- Begin Mailchimp Signup Form -->\n<div id=\"mc_embed_signup\">\n<form action=\"https://cliclap.us11.list-manage.com/subscribe/post?u=9fbe5701dcf71991febe2619b&amp;id=0299234ee6\" method=\"post\" id=\"mc-embedded-subscribe-form\" name=\"mc-embedded-subscribe-form\" class=\"validate\" target=\"_blank\" novalidate>\n    <div id=\"mc_embed_signup_scroll\">\n\t\n<div class=\"mc-field-group\">\n\t<label for=\"mce-FNAME\">First Name  <span class=\"asterisk\">*</span>\n</label>\n\t<input type=\"text\" value=\"\" name=\"FNAME\" class=\"required\" id=\"mce-FNAME\">\n</div>\n<div class=\"mc-field-group\">\n\t<label for=\"mce-LNAME\">Last Name  <span class=\"asterisk\">*</span>\n</label>\n\t<input type=\"text\" value=\"\" name=\"LNAME\" class=\"required\" id=\"mce-LNAME\">\n</div>\n<div class=\"mc-field-group\">\n\t<label for=\"mce-EMAIL\">Work Email  <span class=\"asterisk\">*</span>\n</label>\n\t<input type=\"email\" value=\"\" name=\"EMAIL\" class=\"required email\" id=\"mce-EMAIL\">\n</div>\n<div class=\"mc-field-group size1of2\">\n\t<label for=\"mce-PHONE\">Phone Number  <span class=\"asterisk\">*</span>\n</label>\n\t<input type=\"text\" name=\"PHONE\" class=\"required\" value=\"\" id=\"mce-PHONE\">\n</div>\n\t<div id=\"mce-responses\" class=\"clear\">\n\t\t<div class=\"response\" id=\"mce-error-response\" style=\"display:none\"></div>\n\t\t<div class=\"response\" id=\"mce-success-response\" style=\"display:none\"></div>\n\t</div>    <!-- real people should not fill this in and expect good things - do not remove this or risk form bot signups-->\n    <div style=\"position: absolute; left: -5000px;\" aria-hidden=\"true\"><input type=\"text\" name=\"b_9fbe5701dcf71991febe2619b_0299234ee6\" tabindex=\"-1\" value=\"\"></div>\n    <div class=\"clear\"><input type=\"submit\" value=\"Submit\" name=\"subscribe\" id=\"mc-embedded-subscribe\" class=\"button\"></div>\n    </div>\n</form>\n</div>\n\n<!--End mc_embed_signup-->"},"theme":{"backgroudColor":{"startColor":"#009eed","endColor":"#004780"},"text":{"color":"#ffffff","font":"''Roboto'', sans-serif","fontStyle":"normal","fontWeight":"normal","fontStyleName":"Regular"},"mandetoryFieldIcon":{"color":"#ffffff","font":"''Roboto'', sans-serif","fontStyle":"normal","fontWeight":"normal","fontStyleName":"Regular"},"checkboxText":{"color":"#ffffff","font":"''Roboto'', sans-serif","fontStyle":"normal","fontWeight":"normal","fontStyleName":"Regular"},"checkboxTextHyperlink":{"color":"#ffffff","font":"''Roboto'', sans-serif","fontStyle":"normal","fontWeight":"normal","fontStyleName":"Regular"},"submitButtonBackground":{"color":"#ffffff"},"submitButtonText":{"color":"#009eed","font":"''Roboto'', sans-serif","fontStyle":"normal","fontWeight":"normal","fontStyleName":"Regular"},"useDefault":true},"small":{"title":"Click Goal Example"},"medium":{"title":"Example for CliClap''s goal widget","cta":"Click Example"},"large":{"title":"Example of an Embedded Form"},"mobile":{"title":"","small":"","medium":"Click Example"},"thankYou":{"mode":"internal","internal":{"title":"Thanks for getting in touch.\nWe will get back to you shortly","isLandingPageRedirectEnable":false,"buttonText":"","url":""},"external":{"url":""}}}]]></MetaData>
					<IsArchived>0</IsArchived>
					<IsDisplayable>1</IsDisplayable>
					<ContentTypeId>NULL</ContentTypeId>
					<CtaUrl>https://www.dummy.com/Mailchimp/9fbe5701dcf71991febe2619b/0299234ee6</CtaUrl>
					<CtaUrlContentId>xio784</CtaUrlContentId>
					<GroupId>489</GroupId>
					<IsValid>1</IsValid>
					<isDraft>0</isDraft>				
				</Widget>				
			</Widgets>			
		</Settings>';
	
	-- WIDGETS --
	INSERT INTO [dbo].[Widgets](Name, TypeId, MetaData, IsArchived,IsDisplayable, ContentTypeId, CtaUrl, CtaUrlContentId, GroupId, IsValid, isDraft, UserId, CreatedTimeStamp)
		SELECT	xWidgets.xRows.query('Name').value('.', 'NVARCHAR(256)') AS 'Name', 
				xWidgets.xRows.query('TypeId').value('.', 'INT') AS 'TypeId',
				xWidgets.xRows.query('MetaData').value('.', 'NVARCHAR(MAX)') AS 'MetaData',
				xWidgets.xRows.query('IsArchived').value('.', 'BIT') AS 'IsArchived',
				xWidgets.xRows.query('IsDisplayable').value('.', 'BIT') AS 'IsDisplayable',
				xWidgets.xRows.query('ContentTypeId').value('. cast as xs:int?', 'INT') AS 'ContentTypeId',
				xWidgets.xRows.query('CtaUrl').value('.', 'NVARCHAR(2024)') AS 'CtaUrl',
				xWidgets.xRows.query('CtaUrlContentId').value('.', 'NVARCHAR(32)') AS 'CtaUrlContentId',
				xWidgets.xRows.query('GroupId').value('. cast as xs:int?', 'INT') AS 'GroupId',
				xWidgets.xRows.query('IsValid').value('.', 'BIT') AS 'IsValid',
				xWidgets.xRows.query('isDraft').value('.', 'BIT') AS 'isDraft',
				@UserId,
				GETDATE()
		FROM	@META_DATA.nodes('/Settings/Widgets/Widget') as xWidgets(xRows)	
	
	-- TRAFFIC FILTERS --
	INSERT INTO [dbo].[TrafficFilters](Name, [Action], [Type], BrandId)	
		SELECT	REPLACE(xTrafficFilters.xRows.query('Name').value('.', 'NVARCHAR(256)'), '{{BrandName}}', @BrandName) AS 'Name', 
				xTrafficFilters.xRows.query('Action').value('.', 'INT') AS 'Action',
				xTrafficFilters.xRows.query('Type').value('.', 'INT') AS 'Type',
				@BrandId
		FROM	@META_DATA.nodes('/Settings/TrafficFilters/TrafficFilter') as xTrafficFilters(xRows)	

	-- GROUPS --
	INSERT INTO [dbo].[Groups](Name, [Type], IncludeInDaVinci, BrandId)
		SELECT	xGroups.xRows.query('Name').value('.', 'NVARCHAR(64)') AS 'Name', 			
				xGroups.xRows.query('Type').value('.', 'INT') AS 'Type',
				xGroups.xRows.query('IncludeInDaVinci').value('.', 'BIT') AS 'IncludeInDaVinci',
				@BrandId
		FROM	@META_DATA.nodes('/Settings/Groups/Group') as xGroups(xRows)

	-- TAGS --
	INSERT INTO [dbo].[Tags](Name, [Type], IsDefault, UserId, CreatedTimeStamp)	
		SELECT	xTags.xRows.query('Name').value('.', 'NVARCHAR(64)') AS 'Name', 			
				xTags.xRows.query('Type').value('.', 'INT') AS 'Type',
				xTags.xRows.query('IsDefault').value('.', 'BIT') AS 'IsDefault',
				@UserId,
				GETDATE()
		FROM	@META_DATA.nodes('/Settings/Tags/Tag') as xTags(xRows)
	
	-- POSTS --
	DECLARE @xPostsTemp TABLE(RowNum INT, OriginalUrl NVARCHAR(2048), HostWithoutTld NVARCHAR(128), Host NVARCHAR(128), ContentId NVARCHAR(128), ContentType NVARCHAR(128), ViewPermissionId SMALLINT, IsManuallyCreated BIT);
	INSERT INTO @xPostsTemp
		SELECT	ROW_NUMBER() OVER (ORDER BY NEWID()),
				xPosts.xRows.query('OriginalUrl').value('.', 'NVARCHAR(2048)') AS 'OriginalUrl', 			
				xPosts.xRows.query('HostWithoutTld').value('.', 'NVARCHAR(128)') AS 'HostWithoutTld',
				xPosts.xRows.query('Host').value('.', 'NVARCHAR(128)') AS 'Host',				
				xPosts.xRows.query('ContentId').value('.', 'NVARCHAR(128)') AS 'ContentId',
				xPosts.xRows.query('ContentType').value('.', 'NVARCHAR(128)') AS 'ContentType',
				xPosts.xRows.query('ViewPermissionId').value('.', 'SMALLINT') AS 'ViewPermissionId',
				xPosts.xRows.query('IsManuallyCreated').value('.', 'BIT') AS 'IsManuallyCreated'				
		FROM	@META_DATA.nodes('/Settings/Posts/Post') as xPosts(xRows)
	
	DECLARE @currentId INT	
	DECLARE crsr CURSOR FOR 
		SELECT RowNum FROM @xPostsTemp
	OPEN crsr
	FETCH NEXT FROM crsr INTO @currentId
	WHILE @@FETCH_STATUS = 0
	BEGIN

		INSERT INTO BaseCounter VALUES(NEWID())
		DECLARE @Max BIGINT = (SELECT MAX(Id) FROM BaseCounter);
		DECLARE @ShortUrlCode NVARCHAR(128) = (SELECT [dbo].[ConvertToBase62](@Max))

		INSERT INTO [dbo].[UrlMaps](OriginalUrl, HostWithoutTld, Host, ContentId, ViewPermissionId, IsManuallyCreated, UserId, CreatedTimeStamp, IsActive, Clicks, Conversions, BounceRate, ShortUrl, ShortUrlCode)	
			SELECT	OriginalUrl, HostWithoutTld, Host, ContentId, ViewPermissionId, IsManuallyCreated,
					@UserId,
					GETDATE(),
					1, 
					0, 0, 0,
					CONCAT('https://clp.guru/', @ShortUrlCode),
					@ShortUrlCode
			FROM	@xPostsTemp
			WHERE	RowNum = @currentId

		INSERT INTO [dbo].[Contents](Id, OriginalUrl, IsIFrameDisplayable, LastUpdatedTimeStamp, ContentType)
			SELECT	ContentId, OriginalUrl, 0, GETDATE(), ContentType					
			FROM	@xPostsTemp
			WHERE	RowNum = @currentId

		FETCH NEXT FROM crsr INTO @currentId
	END
	CLOSE crsr
	DEALLOCATE crsr	

END
GO
