create database Task_E_Commerce;
use Task_E_Commerce

create table Category(
				c_id int identity(1,1) primary key,
				c_Name varchar(20) not null);
select * from Category
go
select * from Category
select * from SubCategory
select * from Product


create table SubCategory(
				sc_id int identity(1,1) primary key,
				sc_Name varchar(20) not null,
				c_id int not null,
				Foreign key(c_id) references Category(c_id) on delete cascade			
				);
insert into Category values ('Man'),('Woman'),('Children');

create table Product (ProductId int Identity(1,1) primary key,
					Name varchar(25) not null,
					Description varchar(150) not null,
					c_id int not null,
					sc_id int not null,
					Price decimal(18,2) not null,
					Stock int not null,
					ImageUrl varchar(255),
					CreatedDate Date default(getdate()),
					Foreign key(c_id) references Category(c_id) ,
					Foreign key(sc_id) references SubCategory(sc_id) on delete cascade,
						
			)
go



--create user table 
Create Table UserModel (UserId int identity(1,1)primary key,
				FirstName varchar(25) not null,
				LastName varchar(25) not null,
				Email varchar(30) not null,
				Password varchar(100) not null,
				solt varchar(100) not null,
				PhoneNumber varchar(15) not null,
				Gender varchar(10) 
			)
go


create Table Cart(CartId int identity(1,1) primary key ,  
				UserId int  ,
				ProductId int ,
				Stock int   ,
				Foreign key(Userid) references UserModel(UserId) on delete cascade,
				Foreign key(ProductId) references Product(ProductId) on delete cascade
				)
go



create table OrderTable (
				OrderId int identity(1,1) primary key,
				UserId int,
				ShippingAddress VARCHAR(255) NOT NULL, 
				OrderDate Date default(GetDate()),
				Status varchar(10) default('false'),
				Foreign key(Userid) references UserModel(UserId) on delete cascade)


CREATE TABLE OrderItems (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT,
    ProductId INT,
    Quantity INT,
    UserId INT,
    FOREIGN KEY (OrderId) REFERENCES OrderTable(OrderId),
    FOREIGN KEY (ProductId) REFERENCES Product(ProductId),
    FOREIGN KEY (UserId) REFERENCES UserModel(UserId)
);



 ALTER TABLE Cart
ADD ShoesNumber INT default(0) with values;


 ALTER TABLE OrderItems
ADD ShoesNumber INT default(0) with values;

alter table UserModel add 
				Role varchar(20) default('User') with values;
update UserModel set Role ='Admin' where UserId = 1;
select * from UserModel

CREATE TABLE Payment (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    OrderId INT,
    TotalPayment DECIMAL(18, 2) NOT NULL,
    Status VARCHAR(10) DEFAULT 'false',
    PaymentDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES UserModel(UserId) ,
    FOREIGN KEY (OrderId) REFERENCES OrderTable(OrderId) 
);



CREATE TABLE Reviews (
    ReviewId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ProductId INT NOT NULL,
    Comment NVARCHAR(255),
	reviews_date date default getDate(),
    CONSTRAINT FK_Reviews_Users FOREIGN KEY (UserId) REFERENCES UserModel(UserId) ON DELETE CASCADE,
    CONSTRAINT FK_Reviews_Games FOREIGN KEY (ProductId) REFERENCES Product(ProductId) ON DELETE CASCADE
);




select * from UserModel
select * from Product
select * from Category
select * from SubCategory
select * from Cart
select * from OrderTable
select * from OrderItems
select * from Payment;
select * from Reviews;

delete from Cart ;
delete from UserModel ;
delete from OrderTable ;
delete from OrderItems ;
delete from Payment ;
delete from Reviews ;
go



alter PROCEDURE AddReview
    @UserId INT,
    @ProductId INT,
    @Comment NVARCHAR(MAX)
AS
BEGIN


	 SET NOCOUNT ON;

     INSERT INTO Reviews (UserId, ProductId, Comment)
    VALUES (@UserId, @ProductId, @Comment);


 
    SELECT SCOPE_IDENTITY() AS ReviewId;
END;





alter PROCEDURE GetReviewsByGameId
    @ProductId INT
AS
BEGIN
	SELECT 
    u.FirstName + ' ' + u.LastName AS Fullname,
    max(r.Comment) AS Comment
FROM Reviews r
JOIN UserModel u ON u.UserId = r.UserId
WHERE r.ProductId = @ProductId
GROUP BY u.FirstName, u.LastName 

END;



--CREATE PROCEDURE GetAllReviews
--AS
--BEGIN
--    SET NOCOUNT ON;
--	select r.ReviewId,(u.FirstName + ' ' + u.LastName ) as Fullname,u.Email,p.Name,r.Comment,r.reviews_date from  Reviews r join Product p on r.ProductId = p.ProductId join UserModel u on u.UserId = r.UserId
--END;

alter PROCEDURE GetAllReviews
AS
BEGIN
    SELECT r.ReviewId, 
		   p.ProductId,
		   u.UserId,
           (u.FirstName + ' ' + u.LastName) AS Fullname,
           u.Email, 
           p.Name, 
           r.Comment,
		   r.reviews_date
    FROM Reviews r
    JOIN Product p ON r.ProductId = p.ProductId
    JOIN UserModel u ON u.UserId = r.UserId
    ORDER BY r.reviews_date DESC;  -- Order by reviews_date in descending order
END;





SELECT SUM(Stock)  as total_cart_items
FROM Cart 
WHERE UserId = 4;

--SELECT (u.FirstName + ' ' + u.LastName)as UserName, u.Email, o.ShippingAddress
--FROM UserModel u join OrderTable o on u.UserId = o.UserId
--WHERE u.UserId = 5 and o.OrderId 
SELECT 
    (u.FirstName + ' ' + u.LastName) AS UserName, 
    u.Email, 
    o.ShippingAddress,
	o.OrderId,
	o.OrderDate
FROM 
    UserModel u
JOIN 
    OrderTable o 
    ON u.UserId = o.UserId
WHERE 
    u.UserId = 5 
    AND o.OrderId = (
        SELECT TOP 1 OrderId
        FROM OrderTable
        WHERE UserId = 5
        ORDER BY OrderId DESC
    )

SELECT  p.ProductId, p.ImageUrl, p.Name, p.Price, oi.Quantity, oi.OrderItemId, oi.OrderId
FROM OrderItems oi
JOIN UserModel u ON u.UserId = oi.UserId
JOIN Product p ON p.ProductId = oi.ProductId
WHERE u.UserId = 5
ORDER BY oi.OrderId DESC;


SELECT p.ProductId, p.ImageUrl, p.Name, p.Price, oi.Quantity, oi.OrderItemId, oi.OrderId
FROM OrderItems oi
JOIN UserModel u ON u.UserId = oi.UserId
JOIN Product p ON p.ProductId = oi.ProductId
WHERE u.UserId = 5
AND oi.OrderId = (
    SELECT TOP 1 OrderId
    FROM OrderItems
    WHERE UserId = 5
    ORDER BY OrderId DESC
)
ORDER BY oi.OrderItemId DESC;





SELECT SUM(oi.Quantity * p.Price) 
FROM OrderItems oi
JOIN Product p ON oi.ProductId = p.ProductId
WHERE oi.OrderId = 8 AND p.Price IS NOT NULL AND oi.Quantity > 0

--insert product using procedure 
alter procedure Insert_product(
	@name varchar(100),
	@description varchar(150),
	@catagory int,
	@subcatagory int,
	@price decimal(18,2),
	@stock int,
	@imageurl varchar(255)
)
as
	begin
		insert into Product (Name,Description,c_id,sc_id,Price,Stock,ImageUrl) 
                 values (@name,@description,@catagory,@subcatagory,@price,@stock,@imageurl);
end
go
select * from Product
go
--select All Product
--alter PROCEDURE Select_All
--AS
--BEGIN
--    SELECT p.ProductId, p.Name, p.Description, c.c_Name,sc.sc_Name, p.Price, p.Stock, p.ImageUrl 
--    FROM Product p 
--    JOIN Category c ON p.c_id = c.c_id
--	join SubCategory sc on sc.sc_id =  p.sc_id
--END
--GO

alter PROCEDURE ShowAllCart(@UserId INT)
AS
BEGIN
    SELECT 
        p.ProductId, 
        p.ImageUrl, 
        p.Name, 
        p.Price, 
        c.Stock, 
        c.CartId, 
		c.ShoesNumber,
        cat.c_Name AS CategoryName 
    FROM Cart c
    JOIN UserModel u ON u.UserId = c.UserId
    JOIN Product p ON c.ProductId = p.ProductId
    JOIN Category cat ON p.c_id = cat.c_id
    WHERE u.UserId =  @UserId;
END
GO


--select By Id Product
alter PROCEDURE SelectById(@id INT)
AS
BEGIN
    SELECT 
        p.ProductId,p.Name,p.Description,c.c_Name AS Category,sc.sc_Name As Subategory,p.Price,p.Stock,p.ImageUrl 
    FROM 
        Product p JOIN Category c ON p.c_id = c.c_id
		join SubCategory sc on sc.sc_id =  p.sc_id
    WHERE 
        p.ProductId = @id; 
END
GO


alter PROCEDURE SelectBySubCategory(@Subcategoryid int)
AS
BEGIN
    SELECT 
        p.ProductId,p.Name,p.Description,c.c_Name AS Category,sc.sc_Name As Subategory,p.Price,p.Stock,p.ImageUrl 
    FROM 
        Product p 
		JOIN Category c ON p.c_id = c.c_id
		join SubCategory sc on sc.c_id =  c.c_id
    WHERE 
        c.c_id =  @Subcategoryid; 
END
GO

alter PROCEDURE SelectByCategory(@categoryid int)
AS
BEGIN
    SELECT 
        p.ProductId,p.Name,p.Description,c.c_Name AS Category,sc.sc_Name As Subategory,p.Price,p.Stock,p.ImageUrl 
    FROM 
        Product p 
		JOIN Category c ON c.c_id = p.c_id
		join SubCategory sc on sc.c_id =  c.c_id
    WHERE 
        c.c_id =2 @categoryid; 
END
GO



ALTER PROCEDURE SelectByCategory(@categoryName NVARCHAR(100))
AS
BEGIN
     SELECT 
    p.ProductId,
    p.Name,
    p.Description,
    c.c_Name AS Category,
    MAX(sc.sc_Name) AS Subcategory,  
    p.Price,
    p.Stock,
    p.ImageUrl
FROM 
    Product p
JOIN 
    Category c ON p.c_id = c.c_id
JOIN 
    SubCategory sc ON sc.c_id = c.c_id
WHERE 
    c.c_Name = @categoryName
GROUP BY 
    p.ProductId, p.Name, p.Description, c.c_Name, p.Price, p.Stock, p.ImageUrl; 
END
GO
  SELECT 
    p.ProductId,
    p.Name,
    p.Description,
    c.c_Name AS Category,
    MAX(sc.sc_Name) AS Subcategory,  
    p.Price,
    p.Stock,
    p.ImageUrl
FROM 
    Product p
JOIN 
    Category c ON p.c_id = c.c_id
JOIN 
    SubCategory sc ON sc.c_id = c.c_id
WHERE 
    c.c_Name = 'man'
GROUP BY 
    p.ProductId, p.Name, p.Description, c.c_Name, p.Price, p.Stock, p.ImageUrl; 
select * from Category
select * from SubCategory
select * from Product

select * from UserModel 



go
-- insert data in userModel table 
alter procedure  Insert_UserModel(
		@fname varchar(25),
		@lname varchar(25),
		@email varchar(30),
		@password varchar(100),
		@solt varchar(100),
		@phonenumber varchar(15),
		@gender varchar(10)
	)
As
	Begin
		insert into UserModel (FirstName,LastName,Email,Password,solt,PhoneNumber,Gender)
                            Values (@fname,@lname,@email,@password,@solt,@phonenumber,@gender)
	End
go

--Select All User
create procedure SelectAllUserModel
As
	Begin
		select * from UserModel where Role = 'User';
	End
go

--Select By Id User
alter Procedure SelectByIdUserModel(@UserId int)
As
	Begin
		Select * from UserModel Where UserId = @UserId;
	end
go
select * from Cart

--Update User By Id
create Procedure UpdateUserModel(
		@id int,
		@fname varchar(100),
		@lname varchar(100),
		@email varchar(100),
		@phone varchar(100),
		@gender varchar(100)
	)
As
	Begin
		Update UserModel set FirstName = @fname ,LastName = @lname ,
				Email=@email,PhoneNumber=@phone,
                         Gender = @gender Where UserId = @id;
	End
go

--Delete User
Create Procedure DeleteUserModel(@id int)
as
	begin
		Delete From UserModel Where UserId = @id;
	end
go

--login 
alter PROCEDURE CheckUserLogin
    @email varchar(30)
AS
BEGIN
    SELECT UserId,FirstName,Email, Password,solt,Role
    FROM UserModel
    WHERE Email = @email ;
END;
GO




--create procedure updtaeCartProductStock
--(@UserId int ,@ProductId int)
--as
--begin
--	Update Cart set Stock = Stock + 1 where UserId = @UserId;
--	UPDATE Product SET Stock = Stock - 1 WHERE ProductId = @ProductId;
--end;



alter PROCEDURE updtaeCartProductStock
    @UserId INT,
    @ProductId INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Product WHERE ProductId = @ProductId AND Stock = 0)
    BEGIN        
        RETURN 0; 
    END

    UPDATE Cart
    SET Stock = Stock + 1
    WHERE ProductId = @ProductId;

    UPDATE Product
    SET Stock = Stock - 1
    WHERE ProductId = @ProductId;
END;

exec updtaeCartProductStock @UserId  = 1002 , @ProductId = 1004



--create PROCEDURE insertCartProductStock
--    @uid INT,
--    @pid INT
--AS
--BEGIN
--    IF EXISTS (SELECT 1 FROM Product WHERE ProductId = @pid AND Stock = 0)
--    BEGIN       
--        RETURN 0; 
--    END

--    INSERT INTO Cart (UserId, ProductId,Stock) VALUES (@uid, @pid,'1')

--    UPDATE Product
--    SET Stock = Stock - 1
--    WHERE ProductId = @pid;
--END;



alter PROCEDURE insertCartProductStock
    @uid INT,
    @pid INT,
    @shoesNumber INT -- Added ShoesNumber as a parameter
AS
BEGIN
    -- Check if the product is in stock
    IF EXISTS (SELECT 1 FROM Product WHERE ProductId = @pid AND Stock = 0)
    BEGIN       
        RETURN 0; 
    END

    -- Insert into the Cart table with ShoesNumber
    INSERT INTO Cart (UserId, ProductId, Stock, ShoesNumber) 
    VALUES (@uid, @pid, 1, @shoesNumber); -- Insert ShoesNumber as provided

    -- Update the Product stock
    UPDATE Product
    SET Stock = Stock - 1
    WHERE ProductId = @pid;
END;

exec insertCartProductStock @uid  = 3 , @pid = 20 ,@shoesNumber = 5
exec updtaeCartProductStock @UserId  = 3 , @ProductId = 20 ,@ShoesNumber = 10



ALTER PROCEDURE [dbo].[updtaeCartProductStock]
    @UserId INT,
    @ProductId INT,
    @ShoesNumber INT -- Added ShoesNumber as a parameter
AS
BEGIN
    -- Check if the product is in stock
    IF EXISTS (SELECT 1 FROM Product WHERE ProductId = @ProductId AND Stock = 0)
    BEGIN        
        RETURN 0; 
    END

    -- Update the Cart with the new ShoesNumber
    UPDATE Cart
    SET ShoesNumber = @ShoesNumber ,Stock = Stock + 1
    WHERE UserId = @UserId AND ProductId = @ProductId;

    -- Update the Product stock
    UPDATE Product
    SET Stock = Stock - 1
    WHERE ProductId = @ProductId;
END;


UPDATE Product  SET Stock = 2 WHERE ProductId = 1004

select * from UserModel
select * from Product
select * from Cart
go
--Show All Cart 
create procedure ShowAllCart(@UserId int)
as
	begin
		 select p.ProductId, p.ImageUrl,p.Name,p.Price,c.Stock,c.CartId from Cart c 
			join UserModel u  on u.UserId = c.UserId
			join Product p on c.ProductId = p.ProductId where u.UserId  = @UserId
	End
go


create procedure decreaseStock(
	@uid INT,
    @pid INT)
AS
BEGIN 
    Update  Cart set Stock = Stock - 1 Where UserId = @uid and ProductId = @pid;

	IF EXISTS (SELECT 1 FROM Cart WHERE UserId = @uid AND ProductId = @pid AND Stock = 0)
			BEGIN
				DELETE FROM Cart WHERE UserId = @uid AND ProductId = @pid;
			END
    UPDATE Product SET Stock = Stock + 1 WHERE ProductId = @pid;
	
END
go

--exec decreaseStock @uid = 1, @pid = 1004


go
create procedure removeToCart(@id int ,@uid int )
as
	begin
		 DECLARE @removeStock INT;
	
		 SELECT @removeStock = Stock FROM Cart WHERE ProductId = @id and UserId = @uid;

		Delete from Cart where ProductId = @id and UserId = @uid;

		UPDATE Product SET Stock = Stock+@removeStock  WHERE ProductId = @id;
	end
go



