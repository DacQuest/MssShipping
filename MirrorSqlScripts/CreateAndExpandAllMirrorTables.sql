USE [MssShippingMirrors]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

declare @TableTypes table(
  TableTypeName varchar(20)
  ,HeaderColumns varchar(1000)
  ,ExpansionScript varchar (5000)
  )

insert into @TableTypes values
  ('Array',
    '[ID] INT NOT NULL PRIMARY KEY
    ,[NodeIndex] INT NOT NULL
    ,[Timestamp] DATETIME NOT NULL',
    '
    DECLARE @TargetItemCount INT;
    DECLARE @CurrentItemCount INT;
    DECLARE @Now DATETIME;
    -- Set this to the desired value
    -- 0 means no change
    SET @TargetItemCount = <<TARGET_ITEM_COUNT>>;
    SET @CurrentItemCount = (SELECT COUNT(*) FROM <<TABLE_NAME>>);
    SET @Now = GetDate();
    WHILE (@CurrentItemCount < @TargetItemCount)
    BEGIN
      INSERT INTO <<TABLE_NAME>> ([ID],[NodeIndex],[Timestamp])
      VALUES (@CurrentItemCount+1,@CurrentItemCount,@Now);
      SET @CurrentItemCount = @CurrentItemCount + 1;
    END
    ')
  ,('Dictionary',
    '[ID] INT NOT NULL PRIMARY KEY
    ,[NodeIndex] INT NOT NULL
    ,[Timestamp] DATETIME NOT NULL
    ,[Active] BIT NOT NULL',
    '
    DECLARE @TargetItemCount INT;
    DECLARE @CurrentItemCount INT;
    DECLARE @Now DATETIME;
    -- Set this to the desired value
    -- 0 means no change
    SET @TargetItemCount = <<TARGET_ITEM_COUNT>>;
    SET @CurrentItemCount = (SELECT COUNT(*) FROM <<TABLE_NAME>>);

    SET @Now = GetDate();
    WHILE (@CurrentItemCount < @TargetItemCount)
    BEGIN
      INSERT INTO <<TABLE_NAME>> ([ID],[NodeIndex],[Timestamp],[Active])
      VALUES (@CurrentItemCount+1,@CurrentItemCount,@Now,0);
      SET @CurrentItemCount = @CurrentItemCount + 1;
    END
    ')
  ,('List',
    '[ID] INT NOT NULL PRIMARY KEY
    ,[PrevNodeIndex] INT DEFAULT -1 NOT NULL
    ,[NodeIndex] INT NOT NULL
    ,[NextNodeIndex] INT DEFAULT -1 NOT NULL
    ,[Timestamp] DATETIME NOT NULL
    ,[Active] BIT NOT NULL',
    '
    DECLARE @TargetItemCount INT;
    DECLARE @CurrentItemCount INT;
    DECLARE @Now DATETIME;
    -- Set this to the desired value
    -- 0 means no change
    SET @TargetItemCount = <<TARGET_ITEM_COUNT>>;
    SET @CurrentItemCount = (SELECT COUNT(*) FROM <<TABLE_NAME>>);
    SET @Now = GetDate();
    WHILE (@CurrentItemCount < @TargetItemCount)
    BEGIN
      INSERT INTO <<TABLE_NAME>> ([ID],[PrevNodeIndex],[NodeIndex],[NextNodeIndex],[Timestamp],[Active])
      VALUES (@CurrentItemCount+1,-1,@CurrentItemCount,-1,@Now,0);
      SET @CurrentItemCount = @CurrentItemCount + 1;
    END
    ')
  ,('Child',
    '[ID] INT NOT NULL PRIMARY KEY
    ,[ParentID] INT NOT NULL
    ,[NodeIndex] INT NOT NULL
    ,[ParentArrayIndex] INT NOT NULL
    ,[ArrayIndex] INT NOT NULL
    ,[Timestamp] DATETIME NOT NULL',
    '
    DECLARE @TargetItemCount INT;
    DECLARE @TargetParentArraySize INT;
    DECLARE @TargetArraySize INT;
    DECLARE @Now DATETIME;
    DECLARE @CurrentRecordCount INT;
    DECLARE @CurrentItemCount INT;
    DECLARE @CurrentArraySize INT;
    DECLARE @CurrentParentArraySize INT;
    DECLARE @ItemCounter INT;
    DECLARE @IDCounter INT;
    DECLARE @ParentID INT;
    DECLARE @ParentArrayIndex INT;
    DECLARE @ArrayIndex INT;
    DECLARE @MaxParentArraySize INT;
    DECLARE @MaxArraySize INT;
    -- Set these to the desired values
    -- 0 means no change
    SET @TargetItemCount = <<TARGET_ITEM_COUNT>>;
    SET @TargetParentArraySize = <<TARGET_PARENT_ARRAY_SIZE>>;
    SET @TargetArraySize = <<TARGET_ARRAY_SIZE>>;
    SET @Now = GetDate();
    SET @CurrentRecordCount = (SELECT COUNT(*) FROM <<TABLE_NAME>>);
    IF @CurrentRecordCount = 0
      SET @CurrentItemCount = 0
    ELSE
      SET @CurrentItemCount = @CurrentRecordCount / (SELECT COUNT(*)
      FROM <<TABLE_NAME>>
      WHERE [NodeIndex] = 0);
    IF @CurrentRecordCount = 0
      SET @CurrentParentArraySize = 0
    ELSE
      SET @CurrentParentArraySize = @CurrentRecordCount / (SELECT COUNT(*)
      FROM <<TABLE_NAME>>
      WHERE [ParentArrayIndex] = 0);
    IF @CurrentRecordCount = 0
      SET @CurrentArraySize = 0
    ELSE
      SET @CurrentArraySize = @CurrentRecordCount / (SELECT COUNT(*)
      FROM <<TABLE_NAME>>
      WHERE [ArrayIndex] = 0);
    SET @IDCounter = @CurrentRecordCount + 1;
    SET @ParentID = (@CurrentRecordCount/@TargetArraySize) + 1;
    IF (@CurrentArraySize > 0 AND @CurrentArraySize < @TargetArraySize)
    BEGIN
      SET @ItemCounter = 0;
      WHILE (@ItemCounter < @CurrentItemCount)
      BEGIN
      SET @ParentArrayIndex = @CurrentParentArraySize;
      WHILE (@ParentArrayIndex < @TargetParentArraySize)
      BEGIN
        SET @ArrayIndex = @CurrentArraySize;
        WHILE (@ArrayIndex < @TargetArraySize)
        BEGIN
        INSERT INTO <<TABLE_NAME>> ([ID],[ParentID],[NodeIndex],[ParentArrayIndex],[ArrayIndex],[Timestamp])
          VALUES (@IDCounter,@ParentID,@ItemCounter,@ParentArrayIndex,@ArrayIndex,@Now)
        SET @IDCounter = @IDCounter + 1;
        SET @ArrayIndex = @ArrayIndex + 1;
        END
        SET @ParentID = @ParentID + 1;
        SET @ParentArrayIndex = @ParentArrayIndex + 1;
      END
      SET @ItemCounter = @ItemCounter + 1;
      END
    END
    IF (@TargetParentArraySize >= @CurrentParentArraySize)
      SET @MaxParentArraySize = @TargetParentArraySize;
    ELSE
      SET @MaxParentArraySize = @CurrentParentArraySize;
    IF (@TargetArraySize >= @CurrentArraySize)
      SET @MaxArraySize = @TargetArraySize;
    ELSE
      SET @MaxArraySize = @CurrentArraySize;
    WHILE (@CurrentItemCount < @TargetItemCount)
    BEGIN
      SET @ParentArrayIndex = 0;
      WHILE (@ParentArrayIndex < @MaxParentArraySize)
      BEGIN
      SET @ArrayIndex = 0;
      WHILE (@ArrayIndex < @MaxArraySize)
      BEGIN
        INSERT INTO <<TABLE_NAME>> ([ID],[ParentID],[NodeIndex],[ParentArrayIndex],[ArrayIndex],[Timestamp])
        VALUES (@IDCounter,@ParentID,@CurrentItemCount,@ParentArrayIndex,@ArrayIndex,@Now)
        SET @ArrayIndex = @ArrayIndex + 1;
        SET @IDCounter = @IDCounter + 1;
      END
      SET @ParentID = @ParentID + 1;
      SET @ParentArrayIndex = @ParentArrayIndex + 1;
      END
      SET @CurrentItemCount = @CurrentItemCount + 1;
    END
    ')

----------------------------------------------------------------------
----------------------------------------------------------------------
----------------------------------------------------------------------

declare @DropAllTables bit
declare @CreateAllTables bit
declare @ExpandAllTables bit

----------------------------------------------------------------------

declare @ColumnSets table(
  ColumnSetName varchar(256)
  ,ColumnSet varchar(8000)
  )

insert into @ColumnSets values

    ('PalletItem','
    ,[PalletID] VARCHAR(10) DEFAULT '''' NOT NULL
    ,[Status] BIGINT DEFAULT 0 NOT NULL
    ,[Sku] VARCHAR(50) DEFAULT '''' NOT NULL
    ,[JobID] VARCHAR(50) DEFAULT '''' NOT NULL
    ,[Comment] VARCHAR(50) DEFAULT '''' NOT NULL
    ,[BuiltOn] DATETIME DEFAULT ''12/31/1999'' NOT NULL
    ,[IsStack] BIT DEFAULT 0 NOT NULL
    ,[VehicleRow] BIGINT DEFAULT 0 NOT NULL
    ,[HoldCode] INT DEFAULT 0 NOT NULL
    '),
    ('BinItem','
    ,[BinStatus] BIGINT DEFAULT 1 NOT NULL
    ,[BinSize] BIGINT DEFAULT 0 NOT NULL
    ,[Audit] BIT DEFAULT 0 NOT NULL
    ,[NotUsable] BIT DEFAULT 0 NOT NULL
    ,[Disabled] BIT DEFAULT 0 NOT NULL
    ,[PickOnly] BIT DEFAULT 0 NOT NULL
    ,[StoredOn] DATETIME DEFAULT ''12/31/1999'' NOT NULL
    ,[Location] INT DEFAULT 0 NOT NULL
    '),
    ('PitItem','
    ,[Level] BIGINT DEFAULT 0 NOT NULL
    ,[PalletID] VARCHAR(10) DEFAULT '''' NOT NULL
    ,[PitCode] BIGINT DEFAULT 0 NOT NULL
    ,[SetOn] DATETIME DEFAULT ''12/31/1999'' NOT NULL
    '),
    ('HoldCodeItem','
    ,[HoldCode] INT DEFAULT 0 NOT NULL
    ,[Description] VARCHAR(50) DEFAULT '''' NOT NULL
    '),
    ('LoadItemA','
    ,[Status] BIGINT DEFAULT 0 NOT NULL
    ,[Crane] BIGINT DEFAULT 0 NOT NULL
    ,[PickedOn] DATETIME DEFAULT ''12/31/1999'' NOT NULL
    ,[Transferring] BIT DEFAULT 0 NOT NULL
    ,[SlugLetter] BIGINT DEFAULT 1 NOT NULL
    '),
    ('LoadItemB','
    ,[Status] BIGINT DEFAULT 0 NOT NULL
    ,[Crane] BIGINT DEFAULT 0 NOT NULL
    ,[PickedOn] DATETIME DEFAULT ''12/31/1999'' NOT NULL
    ,[Transferring] BIT DEFAULT 0 NOT NULL
    ,[SlugLetter] BIGINT DEFAULT 2 NOT NULL
    '),
    ('BroadcastItem','
    ,[Status] BIGINT DEFAULT 0 NOT NULL
    ,[Csn] VARCHAR(20) DEFAULT '''' NOT NULL
    ,[VehicleSku] VARCHAR(50) DEFAULT '''' NOT NULL
    ,[Sku] VARCHAR(50) DEFAULT '''' NOT NULL
    ,[Vin] VARCHAR(20) DEFAULT '''' NOT NULL
    ,[PickMode] BIGINT DEFAULT 0 NOT NULL
    ,[PickModeKey] VARCHAR(50) DEFAULT '''' NOT NULL
    ,[ReceivedOn] DATETIME DEFAULT ''12/31/1999'' NOT NULL
    ,[VehicleRowCount] INT DEFAULT 0 NOT NULL
    ,[Shortage] BIT DEFAULT 0 NOT NULL
    '),
    ('SystemSettingsItem','
    ,[PreferredSlug] BIGINT DEFAULT 1 NOT NULL
    ,[AutoReleaseBroadcastEnabled] BIT DEFAULT 0 NOT NULL
    ,[AutoAcceptLoadsEnabled] BIT DEFAULT 0 NOT NULL
    ,[ForceMesPalletDataQueryOnAudit] BIT DEFAULT 1 NOT NULL
    ,[FifoMode] BIGINT DEFAULT 0 NOT NULL
    ,[LargestRotationReceived] INT DEFAULT 0 NOT NULL
    ,[LastCsnReleased] VARCHAR(20) DEFAULT '''' NOT NULL
    ,[SlugPickPriority] BIGINT DEFAULT 0 NOT NULL
    ,[SlugAEnabled] BIT DEFAULT 0 NOT NULL
    ,[SlugALoadNumber] INT DEFAULT 0 NOT NULL
    ,[SlugALoadStartedOn] DATETIME DEFAULT ''12/31/1999'' NOT NULL
    ,[SlugALoadCompletedOn] DATETIME DEFAULT ''12/31/1999'' NOT NULL
    ,[SlugBEnabled] BIT DEFAULT 0 NOT NULL
    ,[SlugBLoadNumber] INT DEFAULT 0 NOT NULL
    ,[SlugBLoadStartedOn] DATETIME DEFAULT ''12/31/1999'' NOT NULL
    ,[SlugBLoadCompletedOn] DATETIME DEFAULT ''12/31/1999'' NOT NULL
    ,[LoadPickPriority] BIGINT DEFAULT 0 NOT NULL
    ,[CraneMode] BIGINT DEFAULT 0 NOT NULL
    ,[LoadAID] INT DEFAULT 0 NOT NULL
    ,[LoadBID] INT DEFAULT 0 NOT NULL
    ,[LoadBEnabled] BIT DEFAULT 0 NOT NULL
    ,[LoadBStartedOn] DATETIME DEFAULT ''12/31/1999'' NOT NULL
    ,[LoadBCompletedOn] DATETIME DEFAULT ''12/31/1999'' NOT NULL
    ,[UpperLevelInboundEnabled] BIT DEFAULT 0 NOT NULL
    ,[LowerLevelInboundEnabled] BIT DEFAULT 0 NOT NULL
    ,[LoadDirectorMode] BIGINT DEFAULT 0 NOT NULL
    '),
    ('SystemSettingsItem_PrioritizeAuditPicks','
    ,[PrioritizeAuditPicks] BIT DEFAULT 0 NOT NULL
    '),
    ('SystemSettingsItem_LowerInboundsEnabled','
    ,[LowerInboundsEnabled] BIT DEFAULT 0 NOT NULL
    '),
    ('SystemSettingsItem_UpperInboundsEnabled','
    ,[UpperInboundsEnabled] BIT DEFAULT 0 NOT NULL
    '),
    ('SystemSettingsItem_LowerOutboundsEnabled','
    ,[LowerOutboundsEnabled] BIT DEFAULT 0 NOT NULL
    '),
    ('SystemSettingsItem_UpperOutboundsEnabled','
    ,[UpperOutboundsEnabled] BIT DEFAULT 0 NOT NULL
    '),
    ('SystemSettingsItem_LoadPicksEnabled','
    ,[LoadPicksEnabled] BIT DEFAULT 0 NOT NULL
    '),
    ('SystemSettingsItem_StackPicksEnabled','
    ,[StackPicksEnabled] BIT DEFAULT 0 NOT NULL
    '),
    ('SystemSettingsItem_PurgePicksEnabled','
    ,[PurgePicksEnabled] BIT DEFAULT 0 NOT NULL
    '),
    ('SystemSettingsItem_AuditPicksEnabled','
    ,[AuditPicksEnabled] BIT DEFAULT 0 NOT NULL
    '),
    ('SystemSettingsItem_AutoCompactStorageEnabled','
    ,[AutoCompactStorageEnabled] BIT DEFAULT 0 NOT NULL
    '),
    ('SystemSettingsItem_CraneTelemetryEnabled','
    ,[CraneTelemetryEnabled] BIT DEFAULT 0 NOT NULL
    '),
    ('SystemSettingsItem_StoresEnabled','
    ,[StoresEnabled] BIT DEFAULT 0 NOT NULL
    ')

declare @TableDefs table(
  ID int identity(1,1)
  ,DropTable bit
  ,CreateTable bit
  ,ExpandTable bit
  ,TableName varchar(256)
  ,TableTypeName varchar(20)
  ,ColumnSetName varchar(50)
  ,TargetItemCount varchar(5)
  ,TargetArraySize varchar(3)
  ,TargetParentArraySize varchar(3)
  )

----------------------------------------------------------------------

set @DropAllTables = 0
set @CreateAllTables = 0
set @ExpandAllTables = 0

----------------------------------------------------------------------

-- Table Definitions (The first 3 values are Drop, Create, Expand, respectively)
insert into @TableDefs values
  (0,0,0,'[dbo].[Broadcast]','Dictionary','BroadcastItem','2000','1','1')

  ,(0,0,0,'[dbo].[HoldCodes]','Dictionary','HoldCodeItem','300','1','1')

  ,(0,0,0,'[dbo].[Storage]','Array','BinItem','1104','1','1')
  ,(0,0,0,'[dbo].[Storage_Pallet]','Child','PalletItem','1104','1','1')

  ,(0,0,0,'[dbo].[UpperPit]','Dictionary','PitItem','100','1','1')
  ,(0,0,0,'[dbo].[UpperPit_Pallet]','Child','PalletItem','100','1','1')

  ,(0,0,0,'[dbo].[LowerPit]','Dictionary','PitItem','100','1','1')
  ,(0,0,0,'[dbo].[LowerPit_Pallet]','Child','PalletItem','100','1','1')

  ,(0,0,0,'[dbo].[AssignmentPit]','Dictionary','PitItem','50','1','1')
  ,(0,0,0,'[dbo].[AssignmentPit_Pallet]','Child','PalletItem','50','1','1')

  ,(0,0,0,'[dbo].[UpperRecircBuffer]','Dictionary','PalletItem','10','1','1')

  ,(0,0,0,'[dbo].[LowerRecircBuffer]','Dictionary','PalletItem','10','1','1')

  ,(0,0,0,'[dbo].[SlugA]','Array','LoadItemA','54','1','1')
  ,(0,0,0,'[dbo].[SlugA_Broadcast]','Child','BroadcastItem','54','1','1')
  ,(0,0,0,'[dbo].[SlugA_Pallet]','Child','PalletItem','54','1','1')

  ,(0,0,0,'[dbo].[SlugB]','Array','LoadItemB','54','1','1')
  ,(0,0,0,'[dbo].[SlugB_Broadcast]','Child','BroadcastItem','54','1','1')
  ,(0,0,0,'[dbo].[SlugB_Pallet]','Child','PalletItem','54','1','1')

  ,(0,0,0,'[dbo].[SystemSettings]','Array','SystemSettingsItem','1','1','1')
  ,(0,0,0,'[dbo].[SystemSettings_PrioritizeAuditPicks]','Child','SystemSettingsItem_PrioritizeAuditPicks','1','5','1')
  ,(0,0,0,'[dbo].[SystemSettings_UpperInboundsEnabled]','Child','SystemSettingsItem_UpperInboundsEnabled','1','5','1')
  ,(0,0,0,'[dbo].[SystemSettings_LowerInboundsEnabled]','Child','SystemSettingsItem_LowerInboundsEnabled','1','5','1')
  ,(0,0,0,'[dbo].[SystemSettings_StoresEnabled]','Child','SystemSettingsItem_StoresEnabled','1','5','1')
  ,(0,0,0,'[dbo].[SystemSettings_UpperOutboundsEnabled]','Child','SystemSettingsItem_UpperOutboundsEnabled','1','5','1')
  ,(0,0,0,'[dbo].[SystemSettings_LowerOutboundsEnabled]','Child','SystemSettingsItem_LowerOutboundsEnabled','1','5','1')
  ,(0,0,0,'[dbo].[SystemSettings_PurgePicksEnabled]','Child','SystemSettingsItem_PurgePicksEnabled','1','5','1')
  ,(0,0,0,'[dbo].[SystemSettings_AutoCompactStorageEnabled]','Child','SystemSettingsItem_AutoCompactStorageEnabled','1','5','1')
  ,(0,0,0,'[dbo].[SystemSettings_AuditPicksEnabled]','Child','SystemSettingsItem_AuditPicksEnabled','1','5','1')
  ,(0,0,0,'[dbo].[SystemSettings_CraneTelemetryEnabled]','Child','SystemSettingsItem_CraneTelemetryEnabled','1','5','1')
  ,(0,0,0,'[dbo].[SystemSettings_LoadPicksEnabled]','Child','SystemSettingsItem_LoadPicksEnabled','1','5','1')
  ,(0,0,0,'[dbo].[SystemSettings_StackPicksEnabled]','Child','SystemSettingsItem_StackPicksEnabled','1','5','1')



----------------------------------------------------------------------

declare @index int
declare @count int

declare @dropTable bit
declare @createTable bit
declare @expandTable bit
declare @tableName varchar(256)
declare @tableTypeName varchar(20)
declare @columnSetName varchar(50)
declare @targetItemCount varchar(5)
declare @targetArraySize varchar(3)
declare @targetParentArraySize varchar(3)
declare @columnSet varchar(4000)
declare @headerColumns varchar(500)
declare @expansionScript varchar(4000)
declare @cmdString varchar(5000);

select @index = min(ID) - 1, @count = max(ID) from @TableDefs
while @index < @count
begin
  set @index = @index + 1
  select
  @dropTable = DropTable
  ,@createTable = CreateTable
  ,@expandTable = ExpandTable
  ,@tableName = TableName
  ,@tableTypeName = TableTypeName
  ,@columnSetName = ColumnSetName
  ,@targetItemCount = TargetItemCount
  ,@targetArraySize = TargetArraySize
  ,@targetParentArraySize = TargetParentArraySize
  from @TableDefs where ID = @index
  select
  @headerColumns = HeaderColumns
  ,@expansionScript = ExpansionScript
  from @TableTypes where TableTypeName = @tableTypeName
  select
  @columnSet = ColumnSet
  from @ColumnSets where ColumnSetName = @columnSetName


  if (@DropAllTables = 1 OR @dropTable = 1)
  begin
  set @cmdString = 'DROP TABLE '+@tableName
  exec (@cmdString)
  end

  if (@CreateAllTables = 1 OR @createTable = 1)
  begin
  set @cmdString = 'CREATE TABLE '+@tableName+' ('+@headerColumns+@columnSet+' ) ON [PRIMARY]'
  exec (@cmdString)
  end

  if (@ExpandAllTables = 1 OR @expandTable = 1)
  begin
  set @cmdString = @expansionScript
  set @cmdString = REPLACE(@cmdString, '<<TABLE_NAME>>', @tableName)
  set @cmdString = REPLACE(@cmdString, '<<TARGET_ITEM_COUNT>>', @targetItemCount)
  set @cmdString = REPLACE(@cmdString, '<<TARGET_ARRAY_SIZE>>', @targetArraySize)
  set @cmdString = REPLACE(@cmdString, '<<TARGET_PARENT_ARRAY_SIZE>>', @targetParentArraySize)
  exec (@cmdString)
  end

end --while loop
