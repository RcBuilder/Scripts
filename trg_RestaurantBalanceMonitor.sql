SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <07/09/2018>
/*
	update RestaurantBank set Balance = 1000 where RestaurantId = '11223344'
	insert into RestaurantBank values('11223344', 300)	
	select * from ChangesLog
	select * from RestaurantBank
	truncate table ChangesLog
*/
-- =============================================
alter TRIGGER trg_RestaurantBalanceMonitor
	ON	RestaurantBank
	AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	
	-- tables: inserted, deleted				

	declare @hasDeleted bit = isNull((select 1 from deleted), 0)
	declare @hasInserted bit = isNull((select 1 from inserted), 0)
		
	declare @action varchar(10)
	if(@hasDeleted = 1 and @hasInserted = 1)
		set @action = 'UPDATE';
	else if(@hasInserted = 1)
		set @action = 'INSERT';
	else if(@hasDeleted = 1)
		set @action = 'DELETE';

	-------

	declare @OldValue nvarchar(200), @NewValue nvarchar(200), @RelatedId varchar(30)
	set @OldValue = (select Balance from deleted) 
	set @NewValue = (select Balance from inserted)
	
	if(@hasInserted = 1)
		set @RelatedId = (select RestaurantId from inserted)
	else if(@hasDeleted = 1)
		set @RelatedId = (select RestaurantId from deleted)

	insert into ChangesLog
	([Action], TableName, ColumnName, OldValue, NewValue, RelatedId, Notes)
	values	
	(@action, 'RestaurantBank', 'Balance', isNull(@OldValue, 0), isNull(@NewValue, 0), isNull(@RelatedId, 0), '')
		
END
GO
