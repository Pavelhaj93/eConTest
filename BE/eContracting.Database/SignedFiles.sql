CREATE TABLE [dbo].[SignedFiles]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Key] VARCHAR(255) NOT NULL, 
    [SessionId] VARCHAR(MAX) NOT NULL, 
    [Guid] VARCHAR(255) NOT NULL, 
    [FileId] INT NOT NULL, 
    CONSTRAINT [FK_SignedFiles_Files] FOREIGN KEY ([FileId]) REFERENCES [Files]([Id])
)
