# AndroidDotNetTest

A prototype Android "project template" for `dotnet test`.

You can currently do:

```bash
$ dotnet build -t:Install
...
$ adb shell am instrument -w com.companyname.AndroidDotNetTest/com.companyname.AndroidDotNetTest.TestInstrumentation
INSTRUMENTATION_RESULT: failed=1
INSTRUMENTATION_RESULT: passed=1
INSTRUMENTATION_RESULT: resultsPath=/data/user/0/com.companyname.AndroidDotNetTest/cache/TestResults
INSTRUMENTATION_RESULT: skipped=1
INSTRUMENTATION_CODE: -1
```
