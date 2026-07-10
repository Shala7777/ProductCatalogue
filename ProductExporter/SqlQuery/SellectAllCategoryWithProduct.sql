USE [Northwind]
GO

/****** Object:  View [dbo].[SelectAllCategoryWithProduct_V]    Script Date: 6/28/2026 1:10:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER view [dbo].[SelectAllCategoryWithProduct_V]

as
	select c.CategoryName,
		   cast(1 as bit) as CategoryIsActive,
		   p.ProductID as ProductCode,
		   p.ProductName,
		   p.UnitsInStock as ProductQuantity,
		   p.UnitPrice as ProductPrice,
		   cast(1 as bit) as ProductIsActive
	from
	Categories as c
		join Products as p on c.CategoryID = p.CategoryID
GO

