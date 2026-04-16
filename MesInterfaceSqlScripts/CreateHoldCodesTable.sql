USE [MesInterface]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ============================================================
-- SHIP Hold Code Table
-- ============================================================

create table [SHIP_HoldCode] (
    [ID]            int             not null identity(1,1),
    [HoldCode]      int             not null,
    [Description]   varchar(50)     not null,
    constraint [PK_SHIP_HoldCode] primary key ([ID])
);
