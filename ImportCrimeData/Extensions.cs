using CrimeData.Entities;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

public static class Extensions
{
    public static async Task<Dictionary<string, short>> PopulateShort<TDbModel>(this DbContext dbContext,
        IEnumerable<string> items)
        where TDbModel : LookupTableBase<short>, new() => await dbContext.Populate<TDbModel, short>(items);

    public static async Task<Dictionary<string, int>> PopulateInt<TDbModel>(this DbContext dbContext,
        IEnumerable<string> items)
        where TDbModel : LookupTableBase<int>, new() => await dbContext.Populate<TDbModel, int>(items);

    public static async Task<Dictionary<string, TLookupType>> Populate<TDbModel, TLookupType>(this DbContext db,
        IEnumerable<string> items, int count = 1)
        where TDbModel : LookupTableBase<TLookupType>, new()
    {
        var output = await db.Set<TDbModel>().Where(w => items.Contains(w.Value))
            .GroupBy(w => w.Value)
            .ToDictionaryAsync(w => w.Key, w => w.First().Id);

        var all = items.Where(w => !output.ContainsKey(w)).Select(q => new TDbModel { Value = q }).ToArray();
        if (all.Any() && count < 2)
        {
            await db.BulkInsertAsync(all);
           
            return await db.Populate<TDbModel, TLookupType>(items, count + 1);
        }


        return output;
    }
}