create procedure sp_InsertCatalogueItem
	@CategoryName nvarchar(100),
	@CategoryIsDeleted bit,
	@ProductCode varchar(10),
	@ProductName nvarchar(100),
	@ProductPrice money,
	@ProductQuantity int,
	@ProductIsDeleted bit
as
begin
	set nocount on;
		
	declare @Result int, @CategoryID int;

	begin try
		begin transaction;

		exec @Result = sp_InsertCategory 
			@CategoryName, 
			@CategoryIsDeleted, 
			@CategoryID output;

		if @Result != 0
			raiserror('Failed to insert category.', 16, 1);

		exec @Result = sp_InsertProduct 
			@CategoryID, 
			@ProductCode, 
			@ProductName, 
			@ProductPrice, 
			@ProductQuantity, 
			@ProductIsDeleted;

		if @Result != 0
			raiserror('Failed to insert product.', 16, 1);

		commit transaction;
	end try
	begin catch
		if @@TRANCOUNT > 0
			rollback transaction;
		throw;
	end catch

	return 0;
end