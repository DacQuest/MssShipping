use [MssMesInterface]
go
set ansi_nulls on
go
set quoted_identifier on
go

---- ============================================================
---- Table: SHIP_StatusChange
---- ============================================================
--create table [SHIP_StatusChange]
--(
--    [ChangeID]     integer       identity(1, 1) not null,
--    [PalletID]     varchar(10)   not null,
--    [JobID]        varchar(50)   not null,
--    [PalletStatus] integer       not null,
--    [HoldCode]     integer       not null,
--    [Comment]      varchar(50)   not null,
--    [EventDTTM]    datetime      null,
--    [ProdDate]     smalldatetime null,
--    [Shift]        char(1)       null,
--    [Period]       integer       null,

--    constraint [PK_SHIP_StatusChange]
--        primary key clustered ([ChangeID] asc)
--)
--go

---- ============================================================
---- Table: SHIP_StatusChangeQueue
---- Foreign Key: ChangeID -> SHIP_StatusChange.ChangeID
---- ============================================================
--create table [SHIP_StatusChangeQueue]
--(
--    [QueueID]       integer       identity(1, 1) not null,
--    [ChangeID]      integer       not null,
--    [Processed]     bit           not null,
--    [ProcessedDTTM] datetime      null,
--    [EventDTTM]     datetime      null,
--    [ProdDate]      smalldatetime null,
--    [Shift]         char(1)       null,
--    [Period]        integer       null,

--    constraint [PK_SHIP_StatusChangeQueue]
--        primary key clustered ([QueueID] asc),

--    constraint [FK_SHIP_StatusChangeQueue_StatusChange]
--        foreign key ([ChangeID])
--        references [SHIP_StatusChange] ([ChangeID])
--)
--go

--=======================================================================
--=======================================================================

CREATE TABLE [dbo].[SHIP_StatusChange](
	[ChangeID] [int] IDENTITY(1,1) NOT NULL,
	[PalletID] [varchar](10) NULL,
	[JobID] [varchar](50) NULL,
	[PalletStatus] [int] NULL,
	[HoldCode] [int] NULL,
	[Comment] [varchar](50) NULL,
	[EventDTTM] [datetime] NULL,
	[ProdDate] [smalldatetime] NULL,
	[Shift] [char](1) NULL,
	[Period] [smallint] NULL,
 CONSTRAINT [PK_SHIP_StatusChange] PRIMARY KEY CLUSTERED 
(
	[ChangeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SHIP_StatusChangeQueue](
	[QueueID] [int] IDENTITY(1,1) NOT NULL,
	[ChangeID] [int] NULL,
	[Processed] [bit] NULL,
	[ProcessedDTTM] [datetime] NULL,
	[Error] [varchar](50) NULL,
	[EventDTTM] [datetime] NULL,
	[ProdDate] [smalldatetime] NULL,
	[Shift] [char](1) NULL,
	[Period] [smallint] NULL,
 CONSTRAINT [PK_SHIP_StatuChangeQueue] PRIMARY KEY CLUSTERED 
(
	[QueueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
