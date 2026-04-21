use [MssMesInterface]
go
set ansi_nulls on
go
set quoted_identifier on
go

---- ============================================================
---- Table: SHIP_LoadHeader
---- ============================================================
--create table [SHIP_LoadHeader]
--(
--    [HeaderID]    integer      identity(1, 1) not null,
--    [LoadNo]      integer      not null,
--    [Slug]        char(1)      not null,
--    [StartedDTTM] datetime     not null,
--    [FinishedDTTM] datetime    not null,
--    [PalletCount] integer      not null,
--    [StartCSN]    varchar(25)  not null,
--    [StopCSN]     varchar(25)  not null,
--    [TrailerNo]   varchar(25)  not null,

--    constraint [PK_SHIP_LoadHeader]
--        primary key clustered ([HeaderID] asc)
--)
--go

---- ============================================================
---- Table: SHIP_LoadDetail
---- Foreign Key: HeaderID -> SHIP_LoadHeader.HeaderID
---- ============================================================
--create table [SHIP_LoadDetail]
--(
--    [DetailID]   integer     identity(1, 1) not null,
--    [HeaderID]   integer     not null,
--    [VIN]        varchar(50) not null,
--    [VehicleRow] integer     not null,
--    [PalletSKU]  varchar(50) not null,
--    [JobID]      varchar(50) not null,

--    constraint [PK_SHIP_LoadDetail]
--        primary key clustered ([DetailID] asc),

--    constraint [FK_SHIP_LoadDetail_LoadHeader]
--        foreign key ([HeaderID])
--        references [SHIP_LoadHeader] ([HeaderID])
--)
--go

---- ============================================================
---- Table: SHIP_LoadQueue
---- Foreign Key: HeaderID -> SHIP_LoadHeader.HeaderID
---- ============================================================
--create table [SHIP_LoadQueue]
--(
--    [QueueID]       integer  identity(1, 1) not null,
--    [HeaderID]      integer  not null,
--    [Processed]     bit      not null,
--    [ProcessedDTTM] datetime null,

--    constraint [PK_SHIP_LoadQueue]
--        primary key clustered ([QueueID] asc),

--    constraint [FK_SHIP_LoadQueue_LoadHeader]
--        foreign key ([HeaderID])
--        references [SHIP_LoadHeader] ([HeaderID])
--)
--go

--=======================================================================
--=======================================================================

CREATE TABLE [dbo].[SHIP_LoadDetail](
	[DetailID] [int] IDENTITY(1,1) NOT NULL,
	[HeaderID] [int] NULL,
	[VIN] [varchar](50) NULL,
	[VehicleRow] [int] NULL,
	[PalletSKU] [varchar](50) NULL,
	[JobID] [varchar](50) NULL,
 CONSTRAINT [PK_SHIP_LoadDetail] PRIMARY KEY CLUSTERED 
(
	[DetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SHIP_LoadHeader](
	[HeaderID] [int] IDENTITY(1,1) NOT NULL,
	[LoadNo] [int] NULL,
	[Slug] [varchar](10) NULL,
	[StartedDTTM] [datetime] NULL,
	[FinishedDTTM] [datetime] NULL,
	[PalletCount] [int] NULL,
	[StartCSN] [varchar](25) NULL,
	[StopCSN] [varchar](25) NULL,
	[TrailerNo] [varchar](25) NULL,
 CONSTRAINT [PK_SHIP_LoadHeader] PRIMARY KEY CLUSTERED 
(
	[HeaderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SHIP_LoadQueue](
	[QueueID] [int] IDENTITY(1,1) NOT NULL,
	[HeaderID] [int] NULL,
	[Processed] [bit] NULL,
	[ProcessedDTTM] [datetime] NULL,
 CONSTRAINT [PK_SHIP_LoadQueue] PRIMARY KEY CLUSTERED 
(
	[QueueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

