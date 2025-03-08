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

DECLARE @DBBackupList TABLE(DBName NVARCHAR(MAX), DBIdentity INT, FilePath NVARCHAR(MAX), FolderPath NVARCHAR(MAX), BakFolderPath NVARCHAR(MAX)) 
INSERT INTO @DBBackupList
	SELECT	DISTINCT 
			t.DBName, 
			DB_ID(DBName),
			physical_name,
			SUBSTRING(physical_name, 1, LEN(physical_name) - CHARINDEX('\', REVERSE(physical_name))),
			'D:\SQL-DB\Backup'
	FROM sys.master_files
	INNER JOIN (
		SELECT Value FROM (
			VALUES 
			('DanielBikes'), 
			('db_smElectronics'),
			('Docsee'),
			('DSI'),
			('Petsee'),
			('Xena')
		) AS t(Value)
	) AS t(DBName) ON(DB_ID(DBName) = database_id) 

SELECT * FROM @DBBackupList

/*
SELECT *
FROM sys.master_files files
INNER JOIN 
@DBBackupList dbs
ON (files.database_id = dbs.DBIdentity)
*/

DECLARE @NOW DATE = GETDATE()
DECLARE @sql NVARCHAR(MAX);
DECLARE @currentValue NVARCHAR(MAX);

-- LOOP 
DECLARE myCursor CURSOR FOR 

SELECT DISTINCT 'BACKUP DATABASE [' + DBName + '] TO DISK = N''' + 
		CONCAT(BakFolderPath, '\' + DBName, '_', @NOW, '_backup.bak') + 
		''' WITH NOFORMAT, INIT,  
		SKIP, REWIND, NOUNLOAD, STATS = 10;' -- INIT = OVERRIDE 
FROM sys.master_files files
INNER JOIN 
@DBBackupList dbs
ON (files.database_id = dbs.DBIdentity)
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
