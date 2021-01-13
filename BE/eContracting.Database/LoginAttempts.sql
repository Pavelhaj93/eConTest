CREATE TABLE [dbo].[LoginAttempts]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Time] DATETIME NOT NULL, 
    [SessionId] VARCHAR(255) NOT NULL, 
    [Guid] VARCHAR(255) NOT NULL, 
    [WrongBirthdate] SMALLINT NOT NULL DEFAULT 0, 
    [WrongValue] SMALLINT NOT NULL DEFAULT 0
)

GO

CREATE INDEX [IX_LoginAttempts_Guid] ON [dbo].[LoginAttempts] ([Guid])
