
alter proc sp_GetAllCategoryAndProduct

as
begin

	set nocount on

	select c.CategoryName,
		c.IsActive,
		p.ProductID, 
		p.ProductName, 
		p.QuantityPerUnit, 
		p.UnitPrice,
		p.IsActive
	from
	Categories as c
	left join Products as p on c.CategoryID = p.CategoryID

end


