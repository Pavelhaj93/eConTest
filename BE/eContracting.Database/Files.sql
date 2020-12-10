CREATE TABLE [dbo].[Files]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FileName] VARCHAR(255) NOT NULL, 
    [FileExtension] VARCHAR(10) NOT NULL, 
    [MimeType] VARCHAR(30) NOT NULL, 
    [Size] INT NOT NULL, 
    [Content] VARBINARY(MAX) NOT NULL
)

GO

CREATE TRIGGER [dbo].[Trigger_Files_DELETE]
    ON [dbo].[Files]
    FOR DELETE
    AS
    BEGIN
        SET NOCOUNT ON;
        DELETE FROM [dbo].[FileAttributes] WHERE [dbo].[FileAttributes].[FileId] IN (SELECT [deleted].[Id] FROM [deleted]);
    END