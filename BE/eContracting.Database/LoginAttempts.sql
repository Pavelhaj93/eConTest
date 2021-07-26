CREATE TABLE [dbo].[LoginAttempts]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[Time] DATETIME NOT NULL,
	[SessionId] VARCHAR(255) NOT NULL,
	[Guid] VARCHAR(255) NOT NULL,
	[IsBirthdateValid] BIT NOT NULL,
	[LoginTypeKey] NVARCHAR(255) NULL,
	[IsLoginValueValid] BIT NOT NULL,
	[BrowserAgent] NVARCHAR(MAX) NULL,
	[LoginState] NVARCHAR(255) NULL,
	[IsBlocking] BIT NOT NULL,
	[CampaignCode] NVARCHAR(255) NULL
)

GO

CREATE INDEX [IX_LoginAttempts_Guid] ON [dbo].[LoginAttempts] ([Guid])
