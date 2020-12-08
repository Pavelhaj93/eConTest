CREATE TABLE [dbo].[Files]
(
    [Id] INT NOT NULL PRIMARY KEY, 
    [FileName] VARCHAR(255) NOT NULL, 
    [FileExtension] VARCHAR(10) NOT NULL, 
    [MimeType] VARCHAR(30) NOT NULL, 
    [Size] INT NOT NULL, 
    [Content] VARBINARY(MAX) NOT NULL
)
