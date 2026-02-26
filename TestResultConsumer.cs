using Android.App;
using Android.OS;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace AndroidDotNetTest;

class TestResultConsumer(Instrumentation instrumentation) : IDataConsumer
{
    public int Passed { get; private set; }
    public int Failed { get; private set; }
    public int Skipped { get; private set; }
    public string? TrxReportPath { get; private set; }

    // IExtension
    public string Uid => "test-result-consumer";
    public string DisplayName => nameof(TestResultConsumer);
    public string Description => "";
    public string Version => "1.0";
    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    // IDataConsumer
    public Type[] DataTypesConsumed => [typeof(TestNodeUpdateMessage), typeof(SessionFileArtifact)];

    public Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        if (value is SessionFileArtifact artifact)
        {
            TrxReportPath = artifact.FileInfo.FullName;
        }
        else if (value is TestNodeUpdateMessage { TestNode: var node })
        {
            var property = node.Properties.SingleOrDefault<TestNodeStateProperty>();
            if (property is PassedTestNodeStateProperty)
            {
                Passed++;
                SendStatus(node, "passed");
            }
            else if (property is FailedTestNodeStateProperty or ErrorTestNodeStateProperty or TimeoutTestNodeStateProperty or CancelledTestNodeStateProperty)
            {
                Failed++;
                SendStatus(node, "failed");
            }
            else if (property is SkippedTestNodeStateProperty)
            {
                Skipped++;
                SendStatus(node, "skipped");
            }
        }
        return Task.CompletedTask;
    }

    void SendStatus(TestNode node, string outcome)
    {
        var bundle = new Bundle();
        var id = node.Properties.SingleOrDefault<TestMethodIdentifierProperty>();
        bundle.PutString("test", id is not null
            ? $"{id.Namespace}.{id.TypeName}.{id.MethodName}"
            : node.DisplayName);
        bundle.PutString("outcome", outcome);
        instrumentation.SendStatus(0, bundle);
    }
}
