using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace AndroidDotNetTest;

class TestResultConsumer : IDataConsumer
{
    public int Passed { get; private set; }
    public int Failed { get; private set; }
    public int Skipped { get; private set; }

    // IExtension
    public string Uid => "test-result-consumer";
    public string DisplayName => nameof(TestResultConsumer);
    public string Description => "";
    public string Version => "1.0";
    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    // IDataConsumer
    public Type[] DataTypesConsumed => [typeof(TestNodeUpdateMessage)];

    public Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        if (value is TestNodeUpdateMessage { TestNode: var node })
        {
            var property = node.Properties.SingleOrDefault<TestNodeStateProperty>();
            if (property is PassedTestNodeStateProperty)
                Passed++;
            else if (property is FailedTestNodeStateProperty or ErrorTestNodeStateProperty or TimeoutTestNodeStateProperty or CancelledTestNodeStateProperty)
                Failed++;
            else if (property is SkippedTestNodeStateProperty)
                Skipped++;
        }
        return Task.CompletedTask;
    }
}
