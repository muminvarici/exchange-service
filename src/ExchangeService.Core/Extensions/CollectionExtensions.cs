namespace ExchangeService.Core.Extensions;

public static class CollectionExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> task)
    {
        return (await task).ToList();
    }

    public static async Task<T[]> ToArrayAsync<T>(this Task<IEnumerable<T>> task)
    {
        return (await task).ToArray();
    }
}