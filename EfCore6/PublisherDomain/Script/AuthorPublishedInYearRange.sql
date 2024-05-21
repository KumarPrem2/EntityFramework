CREATE PROCEDURE [dbo].[AuthorsPublishedInYearRange]
	@yearstart int,
	@yearend int
As
Select Distinct Authors.* from Authors
Left join Books On Authors.AuthorId = Books.AuthorId
Where YEAR(Books.PublishDate) >= @yearstart and YEAR(books.PublishDate) <= @yearend