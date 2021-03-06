USE [DelivIT]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <17-03-2017>
-- Description:	<Search Deliveries>
-- sp_Deliveries_Search @OrderBy = 'Priority', @PageNum = 2, @PageSize = 4
-- =============================================
alter PROCEDURE [dbo].[sp_Deliveries_Search]	
	@PageNum int = 1,
	@PageSize tinyint = 10,
	@RestaurantId varchar(15) = null,
	@EmployeeId varchar(9) = null,
	@StatusId tinyint = null,
	@PaymentStatusId tinyint = null,	
	@FromTime smalldatetime = null,
    @ToTime smalldatetime = null,    
	@OrderBy varchar(20) = 'CreatedDate',
    @OrderByDirection varchar(4) = 'DESC' -- DESC or ASC
AS
BEGIN	
	SET NOCOUNT ON;

	-- CREATE THE ORDER BY PHRASE --
	declare @OrderByCase varchar(24) = @OrderBy + @OrderByDirection
	--------
	
	-- PAGING CALCULATION --
	declare @IndexFrom int = ((@PageNum - 1) * @PageSize) + 1
	declare @IndexTo int = @PageNum * @PageSize
	--------

	;WITH CTE_Deliveries as(
		select	*,
				COUNT(*) OVER() as 'RowsCount',
				ROW_NUMBER() OVER ( 
					order by 
					case when @OrderByCase = 'CreatedDateDESC'	then CreatedDate	end desc,
					case when @OrderByCase = 'CreatedDateASC'	then CreatedDate	end asc,
					case when @OrderByCase = 'DeliveredOnDESC'	then DeliveredOn	end desc,
					case when @OrderByCase = 'DeliveredOnASC'	then DeliveredOn	end asc,
					case when @OrderByCase = 'PriorityDESC'		then [Priority]		end desc,
					case when @OrderByCase = 'PriorityASC'		then [Priority]		end asc,
					case when @OrderByCase = 'TakenOnDESC'		then TakenOn		end desc,
					case when @OrderByCase = 'TakenOnASC'		then TakenOn		end asc	
				) as 'RowNumber'
		from	Deliveries 	
		where	IsDeleted = 0
		and		(@EmployeeId is null OR EmployeeId = @EmployeeId) 
		and		(@RestaurantId is null OR RestaurantId = @RestaurantId)
		and		(@StatusId is null OR DeliveryStatusId = @StatusId)
		and		(@PaymentStatusId is null OR PaymentStatusId = @PaymentStatusId)	 
	)
	select * from CTE_Deliveries where RowNumber between @IndexFrom and @IndexTo
	
END
