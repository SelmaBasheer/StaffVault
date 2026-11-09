/* =============================================================
   StaffVault - database schema 
   ============================================================= */

IF DB_ID(N'StaffVaultDB') IS NULL
    CREATE DATABASE StaffVaultDB;
GO

USE StaffVaultDB;
GO

-- Table --------------------------------------------------------
IF OBJECT_ID(N'dbo.Employee', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Employee
    (
        Id           INT IDENTITY(1, 1) NOT NULL,
        Name         NVARCHAR(100)      NOT NULL,
        DateOfBirth  DATE               NOT NULL,
        Email        NVARCHAR(254)      NOT NULL,
        PicturePath  NVARCHAR(300)      NULL,
        CreatedOnUtc DATETIME2(0)       NOT NULL CONSTRAINT DF_Employee_CreatedOnUtc DEFAULT SYSUTCDATETIME(),
        UpdatedOnUtc DATETIME2(0)       NOT NULL CONSTRAINT DF_Employee_UpdatedOnUtc DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_Employee_Email       PRIMARY KEY NONCLUSTERED (Email),
        CONSTRAINT UQ_Employee_Id          UNIQUE CLUSTERED (Id),
        CONSTRAINT CK_Employee_DateOfBirth CHECK (DateOfBirth < CAST(SYSDATETIME() AS DATE))
    );
END
GO

-- Read all -----------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Employee_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, DateOfBirth, Email, PicturePath, CreatedOnUtc, UpdatedOnUtc
    FROM dbo.Employee
    ORDER BY Id;
END
GO

-- Read one -----------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Employee_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, DateOfBirth, Email, PicturePath, CreatedOnUtc, UpdatedOnUtc
    FROM dbo.Employee
    WHERE Id = @Id;
END
GO

-- Insert -------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Employee_Insert
    @Name        NVARCHAR(100),
    @DateOfBirth DATE,
    @Email       NVARCHAR(254),
    @PicturePath NVARCHAR(300) = NULL,
    @Id          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Employee (Name, DateOfBirth, Email, PicturePath)
    VALUES (@Name, @DateOfBirth, @Email, @PicturePath);

    SET @Id = CAST(SCOPE_IDENTITY() AS INT);
END
GO

-- Update -------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Employee_Update
    @Id          INT,
    @Name        NVARCHAR(100),
    @DateOfBirth DATE,
    @Email       NVARCHAR(254),
    @PicturePath NVARCHAR(300) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Employee
    SET Name         = @Name,
        DateOfBirth  = @DateOfBirth,
        Email        = @Email,
        PicturePath  = @PicturePath,
        UpdatedOnUtc = SYSUTCDATETIME()
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Delete -------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Employee_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Employee
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO


------------------TestData----------------------------------------------
USE StaffVaultDB;
GO

EXEC sp_help 'dbo.Employee';

DECLARE @NewId INT;
EXEC dbo.usp_Employee_Insert
    @Name = N'Abid', @DateOfBirth = '1994-03-04',
    @Email = N'abid@gmail.com', @Id = @NewId OUTPUT;
SELECT @NewId AS NewId;

EXEC dbo.usp_Employee_GetAll;
EXEC dbo.usp_Employee_GetById @Id = @NewId;

EXEC dbo.usp_Employee_Update
    @Id = @NewId, @Name = N'Abid MK', @DateOfBirth = '1994-03-04',
    @Email = N'abidmk@gmail.com';

EXEC dbo.usp_Employee_GetAll;