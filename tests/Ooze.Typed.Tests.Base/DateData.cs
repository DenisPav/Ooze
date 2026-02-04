using System.Collections;

namespace Ooze.Typed.Tests.Base;

class DateData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var dateChunks = Enumerable.Range(1, 101)
            .Select(x => new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(x))
            .Chunk(25);

        foreach (var chunk in dateChunks)
        {
            yield return chunk.Cast<object>().ToArray();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}