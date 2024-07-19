namespace RepoStats.Generator;

public class StatisticsGeneratorOptions
{
    public int MaxParallelismDegree { get; set; } = Environment.ProcessorCount;
    public int BufferSize { get; set; } = 1024;
}