CREATE VIEW AuthorsByArtist
As
Select Artists.FirstName + ' ' + Artists.LastName as Artist,
	   Authors.FirstName + ' ' + Authors.LastName as Author
FROM ARTISTS LEFT JOIN
ArtistCover ON Artists.ArtistId = ArtistCover.ArtistsArtistId 
Left join Cover on ArtistCover.coversCoverId = Covers.CoverId left join
Books on Books.bookId = Covers.BookId left join
authors on books.AuthorId = Authors.AuthorId