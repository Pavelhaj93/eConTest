﻿CREATE TABLE [dbo].[FileAttributes]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FileId] INT NOT NULL, 
    [Name] VARCHAR(255) NOT NULL, 
    [Value] VARCHAR(MAX) NULL 
)

GO
