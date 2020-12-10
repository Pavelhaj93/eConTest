CREATE TABLE [dbo].[UploadGroupOriginalFiles]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [GroupId] INT NOT NULL, 
    [FileId] INT NOT NULL, 
    CONSTRAINT [FK_UploadGroupOriginalFiles_UploadGroups] FOREIGN KEY ([GroupId]) REFERENCES [UploadGroups]([Id]), 
    CONSTRAINT [FK_UploadGroupOriginalFiles_Files] FOREIGN KEY ([FileId]) REFERENCES [Files]([Id]) ON DELETE CASCADE
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