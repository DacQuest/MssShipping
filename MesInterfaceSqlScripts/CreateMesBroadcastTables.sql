use [MssMesInterface]
go
set ansi_nulls on
go
set quoted_identifier on
go

---- ============================================================
---- Table: SHIP_BroadcastHeader
---- ============================================================
--create table [SHIP_BroadcastHeader]
--(
--    [HeaderID]    integer       identity(1, 1) not null,
--    [RotationNo]  integer       not null,
--    [VehicleSKU]  varchar(50)   not null,
--    [VIN]         varchar(50)   not null,
--    [PalletCount] integer       not null,
--    [EventDTTM]   datetime      null,
--    [ProdDate]    smalldatetime null,
--    [Shift]       char(1)       null,
--    [Period]      integer       null,

--    constraint [PK_SHIP_BroadcastHeader]
--        primary key clustered ([HeaderID] asc),
--)
--go

---- ============================================================
---- Table: SHIP_BroadcastDetail
---- Foreign Key: HeaderID -> SHIP_BroadcastHeader.HeaderID
---- ============================================================
--create table [SHIP_BroadcastDetail]
--(
--    [DetailID]       integer       identity(1, 1) not null,
--    [HeaderID]       integer       not null,
--    [VehicleRow]     integer       not null,
--    [PalletSKU]      varchar(50)   not null,
--    [PickModeStatus] integer       not null,
--    [PickModeValue]  varchar(50)   not null,
--    [EventDTTM]      datetime      null,
--    [ProdDate]       smalldatetime null,
--    [Shift]          char(1)       null,
--    [Period]         integer       null,

--    constraint [PK_SHIP_BroadcastDetail]
--        primary key clustered ([DetailID] asc),

--    constraint [FK_SHIP_BroadcastDetail_BroadcastHeader]
--        foreign key ([HeaderID])
--        references [SHIP_BroadcastHeader] ([HeaderID])
--)
--go

---- ============================================================
---- Table: SHIP_BroadcastQueue
---- Foreign Key: HeaderID -> SHIP_BroadcastHeader.HeaderID
---- ============================================================
--create table [SHIP_BroadcastQueue]
--(
--    [QueueID]       integer  identity(1, 1) not null,
--    [HeaderID]      integer  not null,
--    [Processed]     bit      not null,
--    [ProcessedDTTM] datetime null,

--    constraint [PK_SHIP_BroadcastQueue]
--        primary key clustered ([QueueID] asc),

--    constraint [FK_SHIP_BroadcastQueue_BroadcastHeader]
--        foreign key ([HeaderID])
--        references [SHIP_BroadcastHeader] ([HeaderID])
--)
--go

--=======================================================================
--=======================================================================

CREATE TABLE [dbo].[SHIP_BroadcastHeader](
	[HeaderID] [int] IDENTITY(1,1) NOT NULL,
	[RotationNo] [int] NULL,
	[VehicleSKU] [varchar](50) NULL,
	[VIN] [varchar](50) NULL,
	[PalletCount] [int] NULL,
	[EventDTTM] [datetime] NULL,
	[ProdDate] [smalldatetime] NULL,
	[Shift] [char](1) NULL,
	[Period] [smallint] NULL,
 CONSTRAINT [PK_SHIP_BroadcastHeader] PRIMARY KEY CLUSTERED 
(
	[HeaderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SHIP_BroadcastDetail](
	[DetailID] [int] IDENTITY(1,1) NOT NULL,
	[HeaderID] [int] NULL,
	[VehicleRow] [int] NULL,
	[PalletSKU] [varchar](50) NULL,
	[PickModeStatus] [int] NULL,
	[PickModeValue] [varchar](50) NULL,
 CONSTRAINT [PK_SHIP_BroadcastDetail] PRIMARY KEY CLUSTERED 
(
	[DetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SHIP_BroadcastQueue](
	[QueueID] [int] IDENTITY(1,1) NOT NULL,
	[HeaderID] [int] NULL,
	[Processed] [bit] NULL,
	[ProcessedDTTM] [datetime] NULL,
	[EventDTTM] [datetime] NULL,
	[ProdDate] [smalldatetime] NULL,
	[Shift] [char](1) NULL,
	[Period] [smallint] NULL,
 CONSTRAINT [PK_SHIP_BroadcastQueue] PRIMARY KEY CLUSTERED 
(
	[QueueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


