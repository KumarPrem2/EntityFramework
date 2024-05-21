CREATE PROCEDURE DeleteCover
				@coverid int
As
Delete from covers where CoverId = @coverid