IF DB_ID('HomeLibrary') IS NULL
BEGIN
    CREATE DATABASE HomeLibrary;
END
GO

USE HomeLibrary;
GO


-- =========================
-- TABLE
-- =========================

IF OBJECT_ID('dbo.Books', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Books
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,

        Title NVARCHAR(300) NOT NULL,
        Author NVARCHAR(300) NOT NULL,
        PublicationYear INT NOT NULL,

        Description NVARCHAR(MAX) NULL,

        ContentsXml XML NULL,

        ContentsFilePath NVARCHAR(500) NULL
    );
END
GO


-- =========================
-- INSERT
-- =========================

CREATE OR ALTER PROCEDURE dbo.Books_Insert
    @Title NVARCHAR(300),
    @Author NVARCHAR(300),
    @PublicationYear INT,
    @Description NVARCHAR(MAX) = NULL,
    @ContentsXml XML = NULL,
    @ContentsFilePath NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Books
    (
        Title,
        Author,
        PublicationYear,
        Description,
        ContentsXml,
        ContentsFilePath
    )
    VALUES
    (
        @Title,
        @Author,
        @PublicationYear,
        @Description,
        @ContentsXml,
        @ContentsFilePath
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO


-- =========================
-- UPDATE
-- =========================

CREATE OR ALTER PROCEDURE dbo.Books_Update
    @Id INT,
    @Title NVARCHAR(300),
    @Author NVARCHAR(300),
    @PublicationYear INT,
    @Description NVARCHAR(MAX) = NULL,
    @ContentsXml XML = NULL,
    @ContentsFilePath NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Books
    SET
        Title = @Title,
        Author = @Author,
        PublicationYear = @PublicationYear,
        Description = @Description,
        ContentsXml = @ContentsXml,
        ContentsFilePath = @ContentsFilePath
    WHERE Id = @Id;
END
GO


-- =========================
-- DELETE
-- =========================

CREATE OR ALTER PROCEDURE dbo.Books_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Books
    WHERE Id = @Id;
END
GO


-- =========================
-- SELECT ALL
-- =========================

CREATE OR ALTER PROCEDURE dbo.Books_SelectAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Title,
        Author,
        PublicationYear,
        Description,
        ContentsXml,
        ContentsFilePath
    FROM dbo.Books
    ORDER BY Title;
END
GO


-- =========================
-- SELECT BY ID
-- =========================

CREATE OR ALTER PROCEDURE dbo.Books_SelectById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Title,
        Author,
        PublicationYear,
        Description,
        ContentsXml,
        ContentsFilePath
    FROM dbo.Books
    WHERE Id = @Id;
END
GO


-- =========================
-- SEARCH
-- =========================

CREATE OR ALTER PROCEDURE dbo.Books_Search
    @SearchText NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Title,
        Author,
        PublicationYear,
        Description,
        ContentsXml,
        ContentsFilePath
    FROM dbo.Books
    WHERE
        Title LIKE '%' + @SearchText + '%'
        OR Author LIKE '%' + @SearchText + '%'
        OR CONVERT(NVARCHAR(MAX), ContentsXml)
            LIKE '%' + @SearchText + '%'
    ORDER BY Title;
END
GO