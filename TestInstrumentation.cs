using Android.OS;
using Android.Runtime;
using Microsoft.Testing.Platform.Builder;

namespace AndroidDotNetTest;

[Instrumentation(Name = "com.companyname.AndroidDotNetTest.TestInstrumentation")]
public class TestInstrumentation : Instrumentation
{
    protected TestInstrumentation(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership) { }

    public override void OnCreate(Bundle? arguments)
    {
        base.OnCreate(arguments);
        Start();
    }

    public override void OnStart()
    {
        base.OnStart();

        Task.Run(async () =>
        {
            var consumer = new TestResultConsumer();
            var bundle = new Bundle();
            try
            {
                var resultsPath = Path.Combine(Path.GetTempPath(), "TestResults");
                var builder = await TestApplication.CreateBuilderAsync([
                    "--results-directory", resultsPath,
                    "--report-trx-filename", "TestResults.trx"
                ]);
                builder.AddMSTest(() => [GetType().Assembly]);
                builder.TestHost.AddDataConsumer(_ => consumer);

                using ITestApplication app = await builder.BuildAsync();
                int exitCode = await app.RunAsync();

                bundle.PutInt("passed", consumer.Passed);
                bundle.PutInt("failed", consumer.Failed);
                bundle.PutInt("skipped", consumer.Skipped);
                bundle.PutString("resultsPath", Path.Combine(resultsPath, "TestResults.trx"));
                Finish(Result.Ok, bundle);
            }
            catch (Exception ex)
            {
                bundle.PutString("error", ex.ToString());
                Finish(Result.Canceled, bundle);
            }
        });
    }
}