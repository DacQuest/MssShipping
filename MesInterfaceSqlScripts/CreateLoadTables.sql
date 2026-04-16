USE [MesInterface]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================
-- SHIP Load Tables
-- ============================================================

create table [SHIP_LoadHeader] (
    [HeaderID]          int             not null identity(1,1),
    [LoadNo]            int             not null,
    [Slug]              varchar(10)     not null,
    [StartedDTTM]       datetime        null,
    [FinishedDTTM]      datetime        null,
    [PalletCount]       int             not null,
    [StartCSN]          varchar(25)     null,
    [StopCSN]           varchar(25)     null,
    [TrailerID]         varchar(25)     null,
    constraint [PK_SHIP_LoadHeader] primary key ([HeaderID])
);


create table [SHIP_LoadDetail] (
    [DetailID]          int             not null identity(1,1),
    [HeaderID]          int             not null,
    [VIN]               varchar(50)     not null,
    [VehicleRow]        int             not null,
    [PalletSKU]         varchar(50)     not null,
    [JobID]             varchar(50)     null,
    constraint [PK_SHIP_LoadDetail] primary key ([DetailID]),
    constraint [FK_LoadDetail_Header] foreign key ([HeaderID])
        references [SHIP_LoadHeader] ([HeaderID])
);


create table [SHIP_LoadQueue] (
    [QueueID]           int             not null identity(1,1),
    [HeaderID]          int             not null,
    [Processed]         bit             not null default 0,
    [ProcessedDTTM]     datetime        null,
    constraint [PK_SHIP_LoadQueue] primary key ([QueueID]),
    constraint [FK_LoadQueue_Header] foreign key ([HeaderID])
        references [SHIP_LoadHeader] ([HeaderID])
);
