/****** Object:  Table [dbo].[EntityEventSource]    Script Date: 06/14/2013 10:36:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EntityEventSource](
	[Id] [numeric](18, 0) NOT NULL,
	[SnapShot] [varbinary](max) NULL,
	[Version] [int] NOT NULL,
	[State] [varchar](100) NULL,
	[Status] [varchar](100) NULL,
	[ProcessingAt] [datetime] NULL,
	[ProcessingBy] [varchar](100) NULL,
 CONSTRAINT [PK_EntityEventSource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Events]    Script Date: 06/14/2013 10:36:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Events](
	[Id] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[Type] [varchar](500) NULL,
	[Data] [varbinary](max) NULL,
	[EntityId] [numeric](18, 0) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Default [DF_EntityEventSource_Version]    Script Date: 06/14/2013 10:36:29 ******/
ALTER TABLE [dbo].[EntityEventSource] ADD  CONSTRAINT [DF_EntityEventSource_Version]  DEFAULT ((0)) FOR [Version]
GO
/****** Object:  ForeignKey [FK_Events_EntityEventSource]    Script Date: 06/14/2013 10:36:29 ******/
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_EntityEventSource] FOREIGN KEY([EntityId])
REFERENCES [dbo].[EntityEventSource] ([Id])
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_EntityEventSource]
GO
