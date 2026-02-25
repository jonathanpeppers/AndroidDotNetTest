# AndroidDotNetTest

A prototype Android "project template" for `dotnet test`.

You can currently do:

```bash
$ dotnet build -t:Install
...
$ adb shell am instrument -w com.companyname.AndroidDotNetTest/com.companyname.AndroidDotNetTest.TestInstrumentation
INSTRUMENTATION_RESULT: failed=1
INSTRUMENTATION_RESULT: passed=1
INSTRUMENTATION_RESULT: resultsPath=/storage/emulated/0/Android/data/com.companyname.AndroidDotNetTest/files/TestResults/_localhost_2026-02-25_20_09_00.2407809.trx
INSTRUMENTATION_RESULT: skipped=1
INSTRUMENTATION_CODE: -1
$ adb pull /storage/emulated/0/Android/data/com.companyname.AndroidDotNetTest/files/TestResults/_localhost_2026-02-25_20_09_00.2407809.trx
/storage/emulated/0/Android/data/com.companyname.AndroidDotNetTest/files/TestResults/_...-02-25_20_09_00.2407809.trx: 1 file pulled, 0 skipped. 1.0 MB/s (4644 bytes in 0.005s)
```
