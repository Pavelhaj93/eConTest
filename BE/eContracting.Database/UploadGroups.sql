CREATE TABLE [dbo].[UploadGroups]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Key] VARCHAR(255) NOT NULL, 
    [SessionId] VARCHAR(255) NOT NULL, 
    [Guid] VARCHAR(255) NOT NULL, 
    [OutputFileId] INT NOT NULL,
    [CreateDate] DATETIME NULL,
)

GO

--CREATE TRIGGER [dbo].[Trigger_UploadGroups_DELETE]
--    ON [dbo].[UploadGroups]
--    FOR DELETE
--    AS
--    BEGIN
--        SET NOCOUNT ON;
--        DELETE FROM [dbo].[UploadGroupOriginalFiles] WHERE [dbo].[UploadGroupOriginalFiles].[GroupId] IN (SELECT [deleted].[Id] FROM [deleted]);
--    END