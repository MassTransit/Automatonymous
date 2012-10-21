namespace System.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class ListExtensions
    {
        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> callback)
        {
            foreach (var item in list)
                await callback(item);
        }
    }
}