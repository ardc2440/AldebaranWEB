using Microsoft.EntityFrameworkCore.ChangeTracking;

internal static class CounterValueGeneratorHelpers
{
    private static int GetRecordCount<T>(EntityEntry entry) where T : class
    {
        var dbContext = entry.Context;
        return dbContext.Set<T>().Count();
    }
}