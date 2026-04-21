use [MssMesInterface]
go
set ansi_nulls on
go
set quoted_identifier on
go

---- ============================================================
---- Table: SHIP_HoldCode
---- ============================================================
--create table [SHIP_HoldCode]
--(
--    [ID]          integer     identity(1, 1) not null,
--    [HoldCode]    integer     not null,
--    [Description] varchar(50) not null,

--    constraint [PK_SHIP_HoldCode]
--        primary key clustered ([ID] asc)
--)
--go

--=======================================================================
--=======================================================================

CREATE TABLE [dbo].[SHIP_HoldCode](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[HoldCode] [int] NULL,
	[Description] [varchar](50) NULL,
 CONSTRAINT [PK_SHIP_HoldCode] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
