USE [Liver_Cancer_Biomarker]
GO

DELETE [dbo].[PAIR];
GO

DELETE [dbo].[ENTITY];

GO
DELETE [dbo].[ANNOTATION];
GO

DELETE [dbo].[SENTENCE];
DBCC CHECKIDENT('SENTENCE', RESEED, 0);
GO


DELETE [dbo].[ARTICLE];
DBCC CHECKIDENT('ARTICLE', RESEED, 0);
