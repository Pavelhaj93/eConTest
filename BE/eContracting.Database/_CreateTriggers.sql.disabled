USE [InnogyCZ.eContracting]
GO
/****** Object:  Trigger [dbo].[Trigger_UploadGroups_DELETE]    Script Date: 10.12.2020 18:35:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trigger_UploadGroups_DELETE]'))
DROP TRIGGER [dbo].[Trigger_UploadGroups_DELETE]

GO

CREATE TRIGGER [dbo].[Trigger_UploadGroups_DELETE]
    ON [dbo].[UploadGroups]
    FOR DELETE
    AS
    BEGIN
        SET NOCOUNT ON;
        DELETE FROM [dbo].[UploadGroupOriginalFiles] WHERE [dbo].[UploadGroupOriginalFiles].[GroupId] IN (SELECT [deleted].[Id] FROM [deleted]);
    END
GO

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trigger_SignedFiles_DELETE]'))
DROP TRIGGER [dbo].[Trigger_SignedFiles_DELETE]

GO

CREATE TRIGGER [dbo].[Trigger_SignedFiles_DELETE]
    ON [dbo].[SignedFiles]
    FOR DELETE
    AS
    BEGIN
        SET NOCOUNT ON;
        DELETE FROM [dbo].[Files] WHERE [dbo].[Files].[Id] IN (SELECT [deleted].[Id] FROM [deleted]);
    END

GO

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trigger_Files_DELETE]'))
DROP TRIGGER [dbo].[Trigger_Files_DELETE]

GO

CREATE TRIGGER [dbo].[Trigger_Files_DELETE]
    ON [dbo].[Files]
    FOR DELETE
    AS
    BEGIN
        SET NOCOUNT ON;
        DELETE FROM [dbo].[FileAttributes] WHERE [dbo].[FileAttributes].[FileId] IN (SELECT [deleted].[Id] FROM [deleted]);
    END
GO

IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trigger_UploadGroupOriginalFiles_DELETE]'))
DROP TRIGGER [dbo].[Trigger_UploadGroupOriginalFiles_DELETE]

GO

CREATE TRIGGER [dbo].[Trigger_UploadGroupOriginalFiles_DELETE]
    ON [dbo].[UploadGroupOriginalFiles]
    FOR DELETE
    AS
    BEGIN
        SET NOCOUNT ON;
        DELETE FROM [dbo].[Files] WHERE [dbo].[Files].[Id] IN (SELECT [deleted].[FileId] FROM [deleted]);
    END