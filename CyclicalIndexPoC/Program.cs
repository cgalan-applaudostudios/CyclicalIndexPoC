using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;


var symmary = BenchmarkRunner.Run<CyclicalIndex>();
return;
var cyclicalIndex = new CyclicalIndex();
Console.WriteLine(string.Join(", ", cyclicalIndex.WithQueue()));
Console.WriteLine(string.Join(", ", cyclicalIndex.WithIndex()));



[MemoryDiagnoser]
public class CyclicalIndex
{
    private List<int> Items = new List<int> { 1, 2, 3, 4, 5 };
    private Random random = new Random(420);
    private IReadOnlyList<int> BaseNumbers;

    [GlobalSetup]
    public void Setup()
    {
        BaseNumbers = Enumerable.Range(0, 100).Select(_ => random.Next(1, 100)).ToList();
    }

    [Benchmark]
    public List<int> WithQueue()
    {
        var queue = new Queue<int>(Items);
        var result = new List<int>();
        var numbers = new List<int>(BaseNumbers);

        while (queue.TryDequeue(out var item))
        {
            var number = GetNextNumber(numbers, item);
            if (number == default) continue;

            result.Add(number);
            queue.Enqueue(item);
        }

        return result;
    }

    [Benchmark]
    public List<int> WithIndex()
    {
        var itemsToSkip = new List<int>();
        var i = 0;
        var maxIndex = Items.Count - 1;
        var result = new List<int>();
        var numbers = new List<int>(BaseNumbers);

        while (itemsToSkip.Count < Items.Count)
        {
            var item = Items[i];
            if (itemsToSkip.Contains(item))
            {
                i = NextIndex(i, maxIndex);
                continue;
            }

            var number = GetNextNumber(numbers, item);

            if (number != default)
            {
                result.Add(number);
            }
            else
            {
                itemsToSkip.Add(item);
            }

            i = NextIndex(i, maxIndex);
        }

        return result;
    }

    static int NextIndex(int currentIndex, int maxIndex)
    {
        return currentIndex == maxIndex ? 0 : ++currentIndex;
    }
    static int GetNextNumber(List<int> numbers, int divisor)
    {
        var i = numbers.FindIndex(x => x % divisor == 0);

        var n = numbers[i];
        if (n != default) numbers.Remove(n);
        return n;
    }
}
