Create DataBase InternetPhoneBook;

Create Table People(
	ID int IDENTITY(1,1) NOT NULL PRIMARY KEY,
	FirstName nvarchar(256) NOT NULL,
	LastName nvarchar(256) NOT NULL,
	Phone varchar(256) NOT NULL,
	Email varchar(256),
	Created DateTime,
	Updated DateTime
);