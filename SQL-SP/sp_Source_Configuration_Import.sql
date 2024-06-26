USE [MACrawler]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Roby>
-- Create date: <2018-06-07>
-- sp_Source_Configuration_Import 4, 6
-- =============================================
alter PROCEDURE [dbo].[sp_Source_Configuration_Import] 
	@SourceIdFrom int,
	@SourceIdTo int, 

	@IncludeSettings bit = 1, 
	@IncludeFilters bit = 1, 
	@IncludeEntityNodesCollectors bit = 1, 
	@IncludeEntityLinksCollectors bit = 1, 	
	@IncludeFieldParsers bit = 1
AS
BEGIN
	SET NOCOUNT ON;

	declare @CollectorType tinyint = 0;
	declare @CollectorList table([Name] varchar(30));	
	declare @FieldsList table(RowId int);
	declare @FieldsMap table(RowId_SourceIdFrom int, RowId_SourceIdTo int); -- mapping table between the sources fields --

	BEGIN TRANSACTION
	BEGIN TRY

		

		-- [Fields] --	
		
		-- delete fields and related parsers 
		delete from @FieldsList
		insert into @FieldsList
			select RowId
			from [dbo].[SourceFields]
			where [SourceId] = @SourceIdTo
	
		delete from [dbo].[SourceFields] 
		where RowId in (select RowId from @FieldsList)

		delete from [dbo].[FieldParsers]
		where FieldRowId in (select RowId from @FieldsList)

		-- 

		-- map fields
		declare _cursor CURSOR FOR 
		select RowId 
		from [dbo].[SourceFields] 
		where SourceId = @SourceIdFrom
		OPEN _cursor

		declare @currRowId int
		FETCH NEXT FROM _cursor INTO @currRowId  -- first row --

		WHILE @@FETCH_STATUS = 0
		BEGIN
		
			insert into [dbo].[SourceFields] 
			(SourceId, EntityName, FieldName)
			select @SourceIdTo, EntityName, FieldName 
			from [dbo].[SourceFields] 
			where RowId = @currRowId

			-- map the current row
			insert into @FieldsMap values(@currRowId, @@IDENTITY)

			FETCH NEXT FROM _cursor INTO @currRowId -- next row --
		END
	
		CLOSE _cursor
		DEALLOCATE _cursor


		-- [Settings] --
		IF(@IncludeSettings = 1)
		BEGIN
			delete from [dbo].[SourceSettings]
			where SourceId = @SourceIdTo

			--

			insert into [dbo].[SourceSettings] 
			(SourceId, IntervalMin)
				select @SourceIdTo, IntervalMin
				from [dbo].[SourceSettings] 
				where SourceId = @SourceIdFrom

		END 


		-- [Filters] --
		IF(@IncludeFilters = 1)
		BEGIN
			delete from [dbo].[SourceFilters]
			where SourceId = @SourceIdTo

			-- 

			insert into [dbo].[SourceFilters] 
			(SourceId, FilterName, CustomSort, Properties)
				select @SourceIdTo, FilterName, CustomSort, Properties
				from [dbo].[SourceFilters] 
				where SourceId = @SourceIdFrom

		END 


		-- [Nodes Collector] --
		IF(@IncludeEntityNodesCollectors = 1)
		BEGIN	
			set @CollectorType = 1 -- EntityNodes --

			-- collectors to delete (SourceIdTo)
			delete from @CollectorList
			insert into @CollectorList([Name])
				select [Name]
				from [dbo].[SourceCollectors] 
				where SourceId = @SourceIdTo and CollectorType = @CollectorType

			delete from [dbo].[SourceCollectors]
			where SourceId = @SourceIdTo
			and	[Name] in (select [Name] from @CollectorList)

			delete from [dbo].[SourceCollectorSteps]
			where  SourceId = @SourceIdTo 
			and CollectorName in (select [Name] from @CollectorList)

			-- 
			
			-- collectors to add (SourceIdFrom)	
			delete from @CollectorList
			insert into @CollectorList([Name])
				select [Name]
				from [dbo].[SourceCollectors] 
				where SourceId = @SourceIdFrom and CollectorType = @CollectorType

			-- 

			insert into [dbo].[SourceCollectors] 
			(SourceId, [Name], CollectorType)
				select @SourceIdTo, [Name], @CollectorType
				from @CollectorList
		
			--

			insert into [dbo].[SourceCollectorSteps]
			(SourceId, CollectorName, StepName, Expression, Position)
				select @SourceIdTo, CollectorName, StepName, Expression, Position
				from   [dbo].[SourceCollectorSteps]
				where  SourceId = @SourceIdFrom 
				and CollectorName in (select [Name] from @CollectorList)

		END 


		-- [Links Collector] --
		IF(@IncludeEntityLinksCollectors = 1)
		BEGIN	
			set @CollectorType = 2 -- EntityLinks --

			-- collectors to delete (SourceIdTo)
			delete from @CollectorList
			insert into @CollectorList([Name])
				select [Name]
				from [dbo].[SourceCollectors] 
				where SourceId = @SourceIdTo and CollectorType = @CollectorType

			delete from [dbo].[SourceCollectors]
			where SourceId = @SourceIdTo
			and	[Name] in (select [Name] from @CollectorList)

			delete from [dbo].[SourceCollectorSteps]
			where  SourceId = @SourceIdTo 
			and CollectorName in (select [Name] from @CollectorList)

			-- 
			
			-- collectors to add (SourceIdFrom)	
			delete from @CollectorList
			insert into @CollectorList([Name])
				select [Name]
				from [dbo].[SourceCollectors] 
				where SourceId = @SourceIdFrom and CollectorType = @CollectorType

			-- 

			insert into [dbo].[SourceCollectors] 
			(SourceId, [Name], CollectorType)
				select @SourceIdTo, [Name], @CollectorType
				from @CollectorList
		
			--

			insert into [dbo].[SourceCollectorSteps]
			(SourceId, CollectorName, StepName, Expression, Position)
				select @SourceIdTo, CollectorName, StepName, Expression, Position
				from   [dbo].[SourceCollectorSteps]
				where  SourceId = @SourceIdFrom 
				and CollectorName in (select [Name] from @CollectorList)

		END 


		-- [Parsers] --
		IF(@IncludeFieldParsers = 1)
		BEGIN

			delete from @FieldsList
			insert into @FieldsList
				select RowId
				from [dbo].[SourceFields]
				where [SourceId] = @SourceIdFrom

			--

			insert into [dbo].[FieldParsers]
			(FieldRowId, ParserName, CustomSort, Properties)
				select M.RowId_SourceIdTo, ParserName, CustomSort, Properties
				from [dbo].[FieldParsers] P 
					 inner join 
					 @FieldsMap M -- mapping table between the sources fields --
					 on (P.FieldRowId = M.RowId_SourceIdFrom)
				where FieldRowId in (select FieldRowId from @FieldsList)

		END 




		COMMIT TRANSACTION 
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION 
	END CATCH 

	-- show data 
	/*
	print('Source From: ' + cast(@SourceIdFrom as varchar(10)));
	exec sp_Source__Details_Get @SourceIdFrom
	*/

	print('Source To: ' + cast(@SourceIdTo as varchar(10)));
	exec sp_Source__Details_Get @SourceIdTo	

END
