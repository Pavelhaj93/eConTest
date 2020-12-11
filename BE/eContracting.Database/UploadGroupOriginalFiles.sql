CREATE TABLE [dbo].[UploadGroupOriginalFiles]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [GroupId] INT NOT NULL, 
    [FileId] INT NOT NULL 
)

GO

CREATE TRIGGER [dbo].[Trigger_UploadGroupOriginalFiles_DELETE]
    ON [dbo].[UploadGroupOriginalFiles]
    FOR DELETE
    AS
    BEGIN
        SET NOCOUNT ON;
        DELETE FROM [dbo].[Files] WHERE [dbo].[Files].[Id] IN (SELECT [deleted].[FileId] FROM [deleted]);
    END