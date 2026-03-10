USE [DecostarSystem]
GO

/****** Object:  Table [dbo].[PalletDataExchange]    Script Date: 1/28/2026 3:31:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

declare @dropTable bit
declare @createTable bit
declare @expandTable bit
declare @TableName varchar(20)
declare @SqlDrop varchar(MAX)
declare @SqlCreate varchar(MAX)
declare @SqlExpand varchar(MAX)
declare @TargetRecordCount int;

-- table name
set @TableName = 'Pallets'

-- Set this to the desired record count
-- 0 means no change
set @TargetRecordCount = 2000;

-- switches
set @dropTable   = 0
set @createTable = 0
set @expandTable = 0

set @SqlDrop = 'DROP TABLE ['+@TableName+'];'

set @SqlCreate = '
  CREATE TABLE [dbo].['+@TableName+'](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [PalletID] [int] default 0 NOT NULL,
    [Sku] [varchar](50) default ''EMPTY_PALLET'' NOT NULL,
    [JobID] [varchar](20) NULL,
    [BuiltOn] [datetime] NULL,
    [Location] [varchar](10) default '''' NOT NULL,
    [Damaged] [bit] default 0 NOT NULL
   CONSTRAINT [PK_'+@TableName+'] PRIMARY KEY CLUSTERED 
  (
      [ID] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
  ) ON [PRIMARY];'

set @SqlExpand = '
  DECLARE @CurrentRecordCount INT;
  SET @CurrentRecordCount = (SELECT COUNT(*) FROM ['+@TableName+']);
  WHILE (@CurrentRecordCount < '+cast(@TargetRecordCount as varchar(10))+')
  BEGIN
      SET @CurrentRecordCount = @CurrentRecordCount + 1;
      INSERT INTO ['+@TableName+'] ([PalletID])
      VALUES (@CurrentRecordCount);
  END'

if (@dropTable = 1)
begin
  exec(@SqlDrop)
end

if (@createTable = 1)
begin
  exec(@SqlCreate)
end

if (@expandTable = 1)
begin
  exec(@SqlExpand)
end
