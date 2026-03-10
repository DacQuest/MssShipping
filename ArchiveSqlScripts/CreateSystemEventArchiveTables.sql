USE [DecostarArchive]
GO

/****** Object:  Table [dbo].[SystemEventArchive]    Script Date: 06/09/2012 16:40:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SystemEventArchive](
	[Key] BIGINT IDENTITY(1,1) NOT NULL,
	[AckComputerName] NVARCHAR(100) NULL,
	[AckOSUserName] NVARCHAR(100) NULL,
	[AckAppUserName] NVARCHAR(100) NULL,
	[Guid] UNIQUEIDENTIFIER NULL,
	[DomainName] NVARCHAR(100) NULL,
	[ComputerName] NVARCHAR(100) NULL,
	[OSUserName] NVARCHAR(100) NULL,
	[ApplicationName] NVARCHAR(100) NULL,
	[InstanceName] NVARCHAR(100) NULL,
	[ThreadName] NVARCHAR(100) NULL,
	[AppUserName] NVARCHAR(100) NULL,
	[Timestamp] DATETIME NULL,
	[Context] NVARCHAR(100) NULL,
	[LevelText] NVARCHAR(100) NULL,
	[Level] SMALLINT NULL,
	[Message] NVARCHAR(max) NULL,
 CONSTRAINT [PK_SystemEventArchive] PRIMARY KEY CLUSTERED
(
	[Key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[SystemEventDetailsArchive]    Script Date: 06/09/2012 16:40:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SystemEventDetailsArchive](
	[Key] BIGINT NULL,
	[DetailSetIndex] INT NULL,
	[DetailIndex] INT NULL,
	[Name] NVARCHAR(250) NULL,
	[Value] NVARCHAR(max) NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SystemEventDetailsArchive]  WITH CHECK ADD  CONSTRAINT [FK_SystemEventDetailsArchive_SystemEventArchive] FOREIGN KEY([Key])
REFERENCES [dbo].[SystemEventArchive] ([Key])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SystemEventDetailsArchive] CHECK CONSTRAINT [FK_SystemEventDetailsArchive_SystemEventArchive]
GO

