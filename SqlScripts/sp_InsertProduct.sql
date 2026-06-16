create procedure sp_InsertCategory
	@CategoryName nvarchar(100),
	@CategoryIsDeleted bit,
	@CategoryID int output
as
begin
	set nocount on;
		
	select @CategoryID = ID
	from Categories
	where Name = @CategoryName;

	if @CategoryID is null
	begin
		insert into Categories (Name, IsDeleted)
		values (@CategoryName, @CategoryIsDeleted);
		set @CategoryID = SCOPE_IDENTITY();
	end
	else
	begin
		update Categories
		set IsDeleted = @CategoryIsDeleted, 
			UpdateDate = getdate()
		where ID = @CategoryID and IsDeleted != @CategoryIsDeleted;
	end

	return 0;
end