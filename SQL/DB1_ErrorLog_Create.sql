CREATE TABLE [dbo].[ErrorLog](
	[Id] [int] NOT NULL IDENTITY(1,1),
	[Timestamp] [datetime] NOT NULL,
	[Email] [nvarchar](50) NULL,
	[UserName] [nvarchar](max) NOT NULL,
	[Action] [nvarchar](50) NOT NULL,
	[ErrorMessage] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)


