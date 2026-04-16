USE [MesInterface]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================
-- SHIP Broadcast Tables
-- ============================================================

create table [SHIP_BroadcastHeader] (
    [HeaderID]      int             not null identity(1,1),
    [RotationNo]    int             not null,
    [VehicleSKU]    varchar(50)     not null,
    [VIN]           varchar(50)     not null,
    [PalletCount]   int             not null,   -- Number of records in detail table
    [EventDTTM]     datetime        not null,
    [ProdDate]      smalldatetime   not null,
    [Shift]         char(1)         not null,
    [Period]        int             not null,
    constraint [PK_SHIP_BroadcastHeader] primary key ([HeaderID])
);


create table [SHIP_BroadcastDetail] (
    [DetailID]          int             not null identity(1,1),
    [HeaderID]          int             not null,
    [VehicleRow]        int             not null,   -- 1=Front, 2=Rear
    [PalletSKU]         varchar(50)     not null,
    [PickModeStatus]    int             not null,
    [PickModeValue]     varchar(50)     null,       -- JobID or PalletID if PickModeStatus is not SKU
    [EventDTTM]         datetime        not null,
    [ProdDate]          smalldatetime   not null,
    [Shift]             char(1)         not null,
    [Period]            int             not null,
    constraint [PK_SHIP_BroadcastDetail]  primary key ([DetailID]),
    constraint [FK_BroadcastDetail_Header] foreign key ([HeaderID])
        references [SHIP_BroadcastHeader] ([HeaderID])
);


create table [SHIP_BroadcastQueue] (
    [QueueID]           int             not null identity(1,1),
    [HeaderID]          int             not null,
    [Processed]         bit             not null default 0,
    [ProcessedDTTM]     datetime        null,
    constraint [PK_SHIP_BroadcastQueue]   primary key ([QueueID]),
    constraint [FK_BroadcastQueue_Header] foreign key ([HeaderID])
        references [SHIP_BroadcastHeader] ([HeaderID])
);