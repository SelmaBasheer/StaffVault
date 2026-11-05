CREATE DATABASE StaffVaultDB;

USE StaffVaultDB
GO
CREATE TABLE dbo.Employee
(
Id INT IDENTITY(1,1) NOT NULL,
Name NVARCHAR(100) NOT NULL,
DateOfBirth DATE NOT NULL,
Email NVARCHAR(254) NOT NULL,
PicturePath NVARCHAR(300) NULL,
CreatedOnUtc DATETIME2(0) NOT NULL CONSTRAINT DF_Employee_CreatedOnUtc DEFAULT SYSUTCDATETIME(),
UpdatedOnUtc DATETIME2(0) NOT NULL CONSTRAINT DF_Employee_UpdatedOnUtc DEFAULT SYSUTCDATETIME(),
CONSTRAINT PK_Employee_Email PRIMARY KEY NONCLUSTERED (Email),
CONSTRAINT UQ_Employee_Id UNIQUE CLUSTERED (Id)
);

EXEC sp_help 'dbo.Employee';

select * from dbo.Employee;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Employee_GetAll
AS
BEGIN
SET NOCOUNT ON;
	SELECT Id, Name, DateOfBirth, Email, PicturePath, UpdatedOnUtc
    FROM dbo.Employee
    ORDER BY ID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Employee_GetById
	@Id INT
AS
BEGIN
SET NOCOUNT ON;
	SELECT Id, Name, DateOfBirth, Email, PicturePath, UpdatedOnUtc
	FROM dbo.Employee
	WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Employee_Insert
	@Name NVARCHAR(100),
	@DateOfBirth DATE,
	@Email NVARCHAR(254),
	@PicturePath NVARCHAR(300) = NULL,
	@Id INT OUTPUT
AS 
BEGIN
SET NOCOUNT ON;
	INSERT INTO dbo.Employee (Name, DateOfBirth, Email, PicturePath)
	VALUES
	(@Name, @DateOfBirth, @Email, @PicturePath);

	SET @Id = CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Employee_Update
	@Id INT,
	@Name NVARCHAR(100),
	@DateOfBirth DATE,
	@Email NVARCHAR(254),
	@PicturePath NVARCHAR(300) = NULL
AS
BEGIN
SET NOCOUNT ON;
	UPDATE dbo.Employee
	SET 
	Name = @Name,
	DateOfBirth = @DateOfBirth,
	Email = @Email,
	PicturePath = @PicturePath,
	UpdatedOnUtc = SYSUTCDATETIME()
	WHERE Id = @Id;

	SELECT @@ROWCOUNT AS RowsAffected;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Employee_Delete
	@Id INT
AS
BEGIN
SET NOCOUNT ON;
	DELETE FROM dbo.Employee
	WHERE Id = @Id;

	SELECT @@ROWCOUNT AS RowsAffected;
END

DECLARE @NewId INT;
EXEC dbo.usp_Employee_Insert
	@Name = N'Abid',
	@DateOfBirth = '04-03-1994',
	@Email =N'abid@gmail.com',
	@Id = @NewId OUTPUT;
SELECT @NewId AS NewId;

EXEC dbo.usp_Employee_GetAll;

EXEC dbo.usp_Employee_GetById
@Id = 1;

EXEC dbo.usp_Employee_Update
@Id = 1,
@Name = N'Abid MK',
@DateOfBirth = '1994-03-04',
@Email = N'abidmk@gmail.com'
;

DECLARE @NId INT;
EXEC dbo.usp_Employee_Insert
@Name = N'Selma',
@DateOfBirth = '1997-05-30',
@Email = N'selma@gmail.com',
@Id = @NId OUTPUT;
SELECT @NId AS NewId;

