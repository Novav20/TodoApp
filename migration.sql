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
CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
    [Username] nvarchar(450) NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NULL,
    [Bio] nvarchar(max) NULL,
    [ProfilePictureUrl] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [LastLogin] datetime2 NULL,
    [TotalTasks] int NOT NULL,
    [CompletedTasks] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [TodoItems] (
    [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
    [Title] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [IsCompleted] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [CompletedAt] datetime2 NULL,
    [DueDate] datetime2 NULL,
    [Priority] int NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_TodoItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TodoItems_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_TodoItems_UserId] ON [TodoItems] ([UserId]);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241124123658_InitialCreate', N'9.0.0');

COMMIT;
GO

