CREATE TABLE [dbo].[SignedFiles]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Key] VARCHAR(255) NOT NULL, 
    [SessionId] VARCHAR(MAX) NOT NULL, 
    [Guid] VARCHAR(255) NOT NULL, 
    [FileId] INT NOT NULL 
)

GO

CREATE TRIGGER [dbo].[Trigger_SignedFiles_DELETE]
    ON [dbo].[SignedFiles]
    FOR DELETE
    AS
    BEGIN
        SET NOCOUNT ON;
        DELETE FROM [dbo].[Files] WHERE [dbo].[Files].[Id] IN (SELECT [deleted].[Id] FROM [deleted]);
    END