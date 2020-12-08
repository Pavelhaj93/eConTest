CREATE TABLE [dbo].[FileAttributes]
(
    [Id] INT NOT NULL PRIMARY KEY, 
    [FileId] INT NOT NULL, 
    [Name] VARCHAR(255) NOT NULL, 
    [Value] VARCHAR(MAX) NULL, 
    CONSTRAINT [FK_FileAttributes_Files] FOREIGN KEY ([FileId]) REFERENCES [Files]([Id])
)

GO
