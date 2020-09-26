CREATE TABLE dbo.Temp
(
    Id int,
    [Key] VARCHAR(50),
    [Value] VARCHAR(200)
)
 
INSERT INTO Temp 
VALUES
(1, 'Key1', 'Value1'),
(1, 'Key2', 'Value2'),
(1, 'Key3', 'Value3'),
(2, 'Key1', 'Value1'),
(2, 'Key2', 'Value2'),
(3, 'Key1', 'Value1'),
(4, 'Key1', 'Value1'),
(4, 'Key2', 'Value2'),
(4, 'Key3', 'Value3'),
(5, 'Key1', 'Value1')

---SELECT * FROM Temp

-- using pivot --
SELECT
    Id,
    Key1,
    Key2,
    Key3
FROM (
	SELECT Id, [Key], [Value] FROM Temp
     ) AS Src
PIVOT(
	MIN([Value]) -- MUST have some aggregation function
	FOR [Key] IN ( Key1, Key2, Key3)
     ) AS Pvt

---

-- using group by --
SELECT	Id,
		MAX(CASE WHEN [Key] = 'Key1' THEN [Value] END) as 'Key1',
		MAX(CASE WHEN [Key] = 'Key2' THEN [Value] END) as 'Key2',
		MAX(CASE WHEN [Key] = 'Key3' THEN [Value] END) as 'Key3'
FROM	Temp
GROUP BY Id
ORDER BY Id

---

/* OUTPUT
Id	Key1	Key2	Key3
1	Value1	Value2	Value3
2	Value1	Value2	NULL
3	Value1	NULL	NULL
4	Value1	Value2	Value3
5	Value1	NULL	NULL
*/