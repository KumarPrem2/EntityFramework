using Microsoft.EntityFrameworkCore;
using PublisherData;
using PublisherDomain.Models;

namespace PublisherAppTest
{
    [TestClass]
    public class InMemoryTest
    {
        [TestMethod]
        public void CanInsertAuthorIntoDatabase()
        {
            var builder = new DbContextOptionsBuilder<PubContext>();
            builder.UseInMemoryDatabase("CanInsertAuthorIntoDatabase");

            using (var context = new PubContext(builder.Options))
            {
                Author author = new Author { FirstName = "Prem", LastName = "Shankar" };
                context.Authors.Add(author);
                Assert.AreEqual(EntityState.Added, context.Entry(author).State);
            }
        }
    }
}