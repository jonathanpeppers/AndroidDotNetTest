using Android.Runtime;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace AndroidDotNetTest;

[Instrumentation(Name = "com.companyname.AndroidDotNetTest.TestInstrumentation")]
public class TestInstrumentation : Instrumentation, IDataConsumer
{
    protected TestInstrumentation(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership) { }

    int _passed, _failed, _skipped;

    public override void OnCreate(Bundle? arguments)
    {
        base.OnCreate(arguments);

        Start();
    }

    public override async void OnStart()
    {
        base.OnStart();

        var bundle = new Bundle();

        try
        {
            ITestApplicationBuilder builder = await TestApplication.CreateBuilderAsync([]);
            SelfRegisteredExtensions.AddSelfRegisteredExtensions(builder, []);
            builder.TestHost.AddDataConsumer(_ => this);

            using ITestApplication app = await builder.BuildAsync();
            int exitCode = await app.RunAsync();

            bundle.PutInt("passed", _passed);
            bundle.PutInt("failed", _failed);
            bundle.PutInt("skipped", _skipped);
            bundle.PutString("summary", $"Passed: {_passed}, Failed: {_failed}, Skipped: {_skipped}");

            Finish(exitCode == 0 ? Result.Ok : Result.Canceled, bundle);
        }
        catch (Exception ex)
        {
            bundle.PutString("error", ex.ToString());
            Finish(Result.Canceled, bundle);
        }
    }

    // IExtension
    public string Uid => "test-instrumentation";
    public string DisplayName => GetType().Name;
    public string Description => "";
    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public string Version
    {
        get
        {
            var context = Context;
            ArgumentNullException.ThrowIfNull(context);
            return context.PackageManager?.GetPackageInfo(context.PackageName!, 0)?.VersionName ?? "unknown";
        }
    }

    // IDataConsumer
    public Type[] DataTypesConsumed => [typeof(TestNodeUpdateMessage)];

    public Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        if (value is TestNodeUpdateMessage { TestNode: var node })
        {
            var property = node.Properties.SingleOrDefault<TestNodeStateProperty>();
            if (property is PassedTestNodeStateProperty)
            {
                _passed++;
            }
            else if (property is FailedTestNodeStateProperty or ErrorTestNodeStateProperty or TimeoutTestNodeStateProperty or CancelledTestNodeStateProperty)
            {
                _failed++;
            }
            else if (property is SkippedTestNodeStateProperty)
            {
                _skipped++;
            }
        }
        return Task.CompletedTask;
    }
}