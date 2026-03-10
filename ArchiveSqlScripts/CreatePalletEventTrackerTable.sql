USE [DecostarArchive]
GO

/****** Object:  Table [dbo].[PalletEventTracker]    Script Date: 11/17/2025 1:34:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PalletEventTracker](
  [ID] [int] IDENTITY(1,1) NOT NULL,
  [OccurredOn] [datetime] NOT NULL,
  [PalletID] [varchar](4) NOT NULL,
  [Sku] [varchar](50) NOT NULL,
  [JobID] [int] NOT NULL,
  [PalletStatus] [int] NOT NULL,
  [PalletStatusText] [varchar](20) NOT NULL,
  [OperationCode] [int] NOT NULL,
  [OperationCodeText] [varchar](10) NOT NULL,
  [PalletEvent] [int] NOT NULL,
  [PalletEventText] [varchar](20) NOT NULL,
  [PalletEvent] [int] NOT NULL,
  [MoveCommand] [int] NOT NULL,
  [Comment] [varchar](50) NOT NULL
) ON [PRIMARY]
GO

