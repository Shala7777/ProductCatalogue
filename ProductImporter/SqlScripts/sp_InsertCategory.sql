create procedure sp_InsertProduct
	@CategoryID int,
	@ProductCode varchar(10),
	@ProductName nvarchar(100),
	@ProductPrice money,
	@ProductQuantity int,
	@ProductIsDeleted bit
as
begin
	set nocount on;
		
	if not exists (select 1 from Products where Code = @ProductCode)
	begin
		insert into Products (CategoryID, Code, Name, Price, Quantity, IsDeleted)
		values (@CategoryID, @ProductCode, @ProductName, @ProductPrice, @ProductQuantity, @ProductIsDeleted);
	end
	else
	begin
		update Products
		set Name = @ProductName, 
			Price = @ProductPrice, 
			Quantity = @ProductQuantity, 
			IsDeleted = @ProductIsDeleted, 
			CategoryID = @CategoryID,
			UpdateDate = getdate()
		where Code = @ProductCode and (
			Name != @ProductName or
			Price != @ProductPrice or
			Quantity != @ProductQuantity or
			IsDeleted != @ProductIsDeleted or
			CategoryID != @CategoryID
		);
	end

	return 0;
end