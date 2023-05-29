using Microsoft.EntityFrameworkCore;

namespace EasyDesk.RebusCompanions.EfCore.Initializer;

internal class EmptyContext : DbContext
{
    public EmptyContext(DbContextOptions<EmptyContext> options) : base(options)
    {
    }
}
