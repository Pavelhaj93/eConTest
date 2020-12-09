CREATE TABLE [dbo].[UploadGroupOriginalFiles]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [GroupId] INT NOT NULL, 
    [FileId] INT NOT NULL, 
    CONSTRAINT [FK_UploadGroupOriginalFiles_UploadGroups] FOREIGN KEY ([GroupId]) REFERENCES [UploadGroups]([Id]), 
    CONSTRAINT [FK_UploadGroupOriginalFiles_Files] FOREIGN KEY ([FileId]) REFERENCES [Files]([Id])
)
