CREATE TABLE #tbl([c1] NVARCHAR(MAX), [c2] NVARCHAR(MAX), [rest] NVARCHAR(MAX))
BULK INSERT #tbl
FROM 'E:\\Projects\\MemorialEventsApp\\Documents\\[manual] import_guests_20230822081211.csv'
WITH 
(
	FIRSTROW = 2,
	CODEPAGE = '65001',
    FIELDTERMINATOR =',', -- field delimiter --
    ROWTERMINATOR ='\n' -- row delimiter --
)
-- SELECT * FROM #tbl

-----------------------------------------------
-- SELECT * FROM Accounts WHERE Id = '20230822081211'
--INSERT INTO GuestList([AccountId], [Phone], [FirstName], [LastName])
	SELECT	'20230822081211', 
			c2,			
			TRIM(RIGHT(t.c1, LEN(t.c1) - CHARINDEX(' ', t.c1) + 1)) as ls,
			TRIM(LEFT(t.c1, CHARINDEX(' ', t.c1))) as rs
	FROM #tbl as t
-----------------------------------------------

DROP TABLE #tbl
