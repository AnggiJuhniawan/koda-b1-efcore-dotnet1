IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [dbo].[Product] (
    [ProductCode] nvarchar(30) NOT NULL,
    [Name] nvarchar(120) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Stock] int NOT NULL,
    [Category] nvarchar(30) NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY ([ProductCode]),
    CONSTRAINT [CK_Product_Price] CHECK ([Price] >= 0),
    CONSTRAINT [CK_Product_Stock] CHECK ([Stock] >= 0)
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261109035919_InitialCreate', N'10.0.14');

COMMIT;
GO

