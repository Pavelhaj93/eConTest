CREATE TABLE [dbo].[UploadGroups]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Key] VARCHAR(255) NOT NULL, 
    [SessionId] VARCHAR(255) NOT NULL, 
    [Guid] VARCHAR(255) NOT NULL, 
    [OutputFileId] INT NOT NULL, 
    CONSTRAINT [FK_UploadGroups_Files] FOREIGN KEY ([OutputFileId]) REFERENCES [Files]([Id])
)
