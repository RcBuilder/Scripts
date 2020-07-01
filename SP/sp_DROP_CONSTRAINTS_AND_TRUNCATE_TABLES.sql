/*
	-- drop all constraints --
	DECLARE @sql NVARCHAR(MAX) = N'';
	SELECT @sql += N'
	ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) +  ' DROP CONSTRAINT ' + QUOTENAME(name) + ';'
	FROM sys.foreign_keys;
	EXEC(@sql)
*/


-- truncate all data excepts of the provided tables --
CREATE TABLE #tablesToDelete(name NVARCHAR(MAX)) 
INSERT INTO #tablesToDelete
	SELECT	name --, object_id 
	FROM	sys.tables
	WHERE	name NOT IN(
		'ContentTypes',
		'GroupTypes',
		'TagTypes',
		'TrafficFilterTypes',
		'ViewPermissionType',
		'WidgetTypes',
		'SummaryTypes'
	)
		
WHILE(EXISTS(SELECT 1 FROM #tablesToDelete))
BEGIN 
	DECLARE @name NVARCHAR(MAX) = (SELECT TOP 1 name FROM #tablesToDelete);	
	PRINT(@name)
	EXEC('TRUNCATE TABLE ' + @name)
	DELETE FROM #tablesToDelete WHERE name = @name;
END 

-- DROP TABLE #tablesToDelete
