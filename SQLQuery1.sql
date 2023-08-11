use Tovars123


CREATE TABLE Users
(ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
Login_ NVARCHAR(50) NOT NULL UNIQUE,
Password_ NVARCHAR(50) NOT NULL,
IsAdmin BIT DEFAULT 0 NOT NULL)






select * from Tovars
select * from Users



delete from Tovars where id > 0