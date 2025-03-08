-- SQL BACKUP DBs --

/*	
BACKUP DATABASE [KeterTEST] 
TO  DISK = N'D:\SQL-DB\Backup\KeterTEST_backup_2025_01_12_120454_1738488.bak' 
WITH NOFORMAT, NOINIT,  
NAME = N'KeterTEST_backup_2025_01_12_120454_1738488', 
SKIP, REWIND, NOUNLOAD,  STATS = 10
GO
--
BACKUP DATABASE [YourDatabaseName] 
TO DISK = 'C:\Backups\YourDatabase.bak' 
WITH COMPRESSION, INIT
--
BACKUP DATABASE [YourDatabaseName] 
TO DISK = 'C:\Backups\YourDatabase.bak' 
WITH NOFORMAT, NOINIT,  
SKIP, REWIND, NOUNLOAD, STATS = 10;
*/

DECLARE @DBBackupList TABLE(DBName NVARCHAR(MAX), FilePath NVARCHAR(MAX), FolderPath NVARCHAR(MAX), BakFolderPath NVARCHAR(MAX)) 
INSERT INTO @DBBackupList
	SELECT	[name], 
			physical_name,
			SUBSTRING(physical_name, 1, LEN(physical_name) - CHARINDEX('\', REVERSE(physical_name))),
			'D:\SQL-DB\Backup'
	FROM sys.master_files
	WHERE database_id IN (
		SELECT DB_ID(DBID) FROM (
			VALUES ('DanielBikes'), ('db_smElectronics')
		) AS t(DBID)
	)	
	
/*
SELECT *
FROM sys.master_files files
INNER JOIN 
@DBBackupList dbs
ON (files.database_id = DB_ID(dbs.DBName))
*/

DECLARE @NOW DATE = GETDATE()
DECLARE @sql NVARCHAR(MAX);
DECLARE @currentValue NVARCHAR(MAX);

-- LOOP 
DECLARE myCursor CURSOR FOR 

SELECT DISTINCT 'BACKUP DATABASE [' + DBName + '] TO DISK = N''' + 
		CONCAT(BakFolderPath, '\' + DBName, '_', @NOW, '_', NEWID(), '_backup.bak') + 
		''' WITH NOFORMAT, NOINIT,  
		SKIP, REWIND, NOUNLOAD, STATS = 10;'
FROM sys.master_files files
INNER JOIN 
@DBBackupList dbs
ON (files.database_id = DB_ID(dbs.DBName))
WHERE type_desc = 'ROWS'
--- WHERE database_id IN (SELECT DB_ID(DBName) FROM @DBBackupList);

OPEN myCursor;
FETCH NEXT FROM myCursor INTO @currentValue;

WHILE @@FETCH_STATUS = 0
BEGIN 
	PRINT(@currentValue)
	EXEC sp_executesql @currentValue;    
    FETCH NEXT FROM myCursor INTO @currentValue;
END;

CLOSE myCursor;
DEALLOCATE myCursor;
