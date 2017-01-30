CREATE TABLE [dbo].[PythagorasValues](
	[Id] [int] NOT NULL,
	[DB1Id] [int] NOT NULL,
	[A_Value] [float] NULL,
	[B_Value] [float] NULL,
	[C_Value] [float] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)