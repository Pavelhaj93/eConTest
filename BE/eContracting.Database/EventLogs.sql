CREATE TABLE [dbo].[EventLogs]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Time] DATETIME NOT NULL, 
    [SessionId] VARCHAR(255) NOT NULL, 
    [Guid] VARCHAR(255) NOT NULL, 
    [Event] VARCHAR(50) NOT NULL, 
    [Message] VARCHAR(MAX) NULL, 
    [Error] VARCHAR(MAX) NULL 
)

GO

CREATE INDEX [IX_EventLogs_Guid] ON [dbo].[EventLogs] ([Guid])

GO

CREATE INDEX [IX_EventLogs_Event] ON [dbo].[EventLogs] ([Event])
