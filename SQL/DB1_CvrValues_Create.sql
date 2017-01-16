USE [DB1]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CvrValues](
	[Id] [int] NOT NULL IDENTITY(1,1),
	[CvrNumber] [int] NOT NULL,
	[OrganizationName] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
INSERT INTO [dbo].[CvrValues]([CvrNumber], [OrganizationName]) 
	VALUES (29188505, 'Slagelse Kommune')
INSERT INTO [dbo].[CvrValues]([CvrNumber], [OrganizationName]) 
	VALUES (29188475, 'Faxe Kommune') 
INSERT INTO [dbo].[CvrValues]([CvrNumber], [OrganizationName]) 
	VALUES (64942212, 'KMD dk') 