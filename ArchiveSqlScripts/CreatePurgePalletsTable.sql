USE [MssShippingArchive]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurgePallets](
  [ID] [int] IDENTITY(1,1) NOT NULL,
  [PurgedOn] datetime NOT NULL,
  [PalletID] varchar(10) NOT NULL,
  [Sku] varchar(50) NOT NULL,
  [JobID] varchar(50) NOT NULL,
  [HoldCode] int NOT NULL,
  [BuiltOn] datetime NOT NULL,
  [Comment] varchar(50) NOT NULL,
) ON [PRIMARY]
GO

