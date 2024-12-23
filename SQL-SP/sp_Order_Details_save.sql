USE [MNew]
GO
/****** Object:  StoredProcedure [dbo].[sp_Order_Details_save]    Script Date: 17/05/2022 12:01:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2021-03-07>
-- sp_order_get 3036
-- =============================================
ALTER PROCEDURE [dbo].[sp_Order_Details_save]	
    @Id INT,    	
    @RestaurantId INT,
	@Status TINYINT,
	@Type TINYINT,
	@PaymentType TINYINT,
	@IsPaid BIT,
	@IsDelivery BIT,
	@DeliveryCost FLOAT,
	@DeliveryNotes NVARCHAR(200),
	@Floor TINYINT,
	@Apartment TINYINT,
	@ClientName NVARCHAR(100), 
	@ClientPhone VARCHAR(20), 
	@ClientEmail NVARCHAR(250), 
	@ClientAddress NVARCHAR(500),
	@Notes NVARCHAR(500),  
    @Tip FLOAT,    
	@Discount FLOAT, 
	@NameOnReceipt NVARCHAR(100), 
	@TableNumber INT,
	@OrderDate DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	IF(@OrderDate IS NULL)
		SET @OrderDate = DATEADD(MINUTE, 30, GETDATE())	

	/*
	DECLARE @OrderTimeInMin INT = (SELECT TOP 1 OrderTimeInMin FROM [dbo].[RestaurantSettings] WITH(NOLOCK) WHERE RestaurantId = @RestaurantId)	
	SET @OrderDate = DATEADD(MINUTE, ISNULL(@OrderTimeInMin, 30), @OrderDate) -- add x minutes to the provision date
	*/

	IF(@Id <= 0)
	BEGIN 
		INSERT INTO [dbo].[Orders] ([ClientPhone], [OrderDate])
		VALUES (ISNULL(@ClientPhone, ''), @OrderDate)

		SET @Id = SCOPE_IDENTITY();
	END 

	UPDATE	[dbo].[Orders] 
	SET		RestaurantId = @RestaurantId,
			[Status] = @Status,
			[Type] = @Type,
			PaymentType = @PaymentType,
			IsPaid = @IsPaid,
			IsDelivery = @IsDelivery,
			DeliveryCost = @DeliveryCost,
			DeliveryNotes = ISNULL(@DeliveryNotes, ''),
			[Floor] = @Floor,
			Apartment = @Apartment,
			ClientName = ISNULL(@ClientName, ''),
			ClientPhone = ISNULL(@ClientPhone, ''),
			ClientEmail = ISNULL(@ClientEmail, ''),
			ClientAddress = ISNULL(@ClientAddress, ''),
			Notes = ISNULL(@Notes, ''),
			Tip = @Tip,
			Discount = @Discount,
			NameOnReceipt = ISNULL(@NameOnReceipt, ''),
			TableNumber = @TableNumber,
			UpdatedDate = GETDATE()
	WHERE	Id = @Id

	SELECT @Id AS 'Res'
END