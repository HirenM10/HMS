using Clinical.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Clinical.UnitTests.Common;

public static class TestApplicationDbContext
{
    public static ApplicationDbContext Create(string databaseName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new ApplicationDbContext(options, new Mock<IPublisher>().Object);
    }
}
