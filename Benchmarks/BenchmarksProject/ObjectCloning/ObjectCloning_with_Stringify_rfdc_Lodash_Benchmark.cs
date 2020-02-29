// #ObjectCloning-Benchmark_id_code

// RESULTS 
/*
Time for 10_000 cycle
|                                   Method |       Mean |     Error |    StdDev |     Median |       Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------------------------- |-----------:|----------:|----------:|-----------:|------------:|------:|------:|----------:|
| ClearScript_ObjectCloning_with_Stringify | 1,385.5 ms | 100.12 ms | 288.87 ms | 1,223.3 ms |   5000.0000 |     - |     - |   8.11 MB |
|    ClearScript_ObjectCloning_with_Lodash | 1,082.0 ms |  21.61 ms |  46.98 ms | 1,074.5 ms |   5000.0000 |     - |     - |   8.31 MB |
|      ClearScript_ObjectCloning_with_rfdc |   998.4 ms |  19.86 ms |  54.02 ms |   992.1 ms |   5000.0000 |     - |     - |   8.14 MB |
|        Jint_ObjectCloning_with_Stringify | 1,968.2 ms |  11.97 ms |   9.34 ms | 1,965.7 ms | 438000.0000 |     - |     - | 655.95 MB |
|             Jint_ObjectCloning_with_rfdc | 2,192.5 ms |   9.98 ms |   9.33 ms | 2,190.4 ms | 363000.0000 |     - |     - | 546.12 MB |

old run, Time for 10_000 cycle
|                                   Method |    Mean |    Error |   StdDev |       Gen 0 |     Gen 1 | Gen 2 | Allocated |
|----------------------------------------- |--------:|---------:|---------:|------------:|----------:|------:|----------:|
| ClearScript_ObjectCloning_with_Stringify | 1.261 s | 0.0551 s | 0.0824 s |   5000.0000 |         - |     - |   8.34 MB |
|    ClearScript_ObjectCloning_with_Lodash | 1.159 s | 0.0230 s | 0.0329 s |   5000.0000 |         - |     - |   8.59 MB |
|      ClearScript_ObjectCloning_with_rfdc | 1.032 s | 0.0163 s | 0.0127 s |   5000.0000 |         - |     - |   8.41 MB |
|        Jint_ObjectCloning_with_Stringify | 2.227 s | 0.0226 s | 0.0201 s | 436000.0000 | 1000.0000 |     - | 656.85 MB |
|             Jint_ObjectCloning_with_rfdc | 2.504 s | 0.0287 s | 0.0268 s | 362000.0000 | 1000.0000 |     - | 547.02 MB |

old run, Time for 10_000 cycle
|                                   Method |    Mean |    Error |   StdDev |  Median |       Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------------------------- |--------:|---------:|---------:|--------:|------------:|------:|------:|----------:|
| ClearScript_ObjectCloning_with_Stringify | 1.232 s | 0.0245 s | 0.0597 s | 1.211 s |   5000.0000 |     - |     - |   8.34 MB |
|    ClearScript_ObjectCloning_with_Lodash | 1.420 s | 0.1058 s | 0.3087 s | 1.368 s |   5000.0000 |     - |     - |    8.6 MB |
|        Jint_ObjectCloning_with_Stringify | 2.451 s | 0.1138 s | 0.3209 s | 2.376 s | 438000.0000 |     - |     - | 656.19 MB |
*/

using BenchmarkDotNet.Attributes;
using CSharpFunctionalExtensions;
using Scripting.Js.v1;
using System;

namespace BenchmarksProject
{
    // from https://benchmarkdotnet.org/articles/overview.html
    [MemoryDiagnoser]
    public class ObjectCloning_with_Stringify_rfdc_Lodash_Benchmark
    {
        private const int loopCycle = 10_000;

        private ScriptingContext jsScriptingContext { get; set; }

        private void Init()
        {
            Result<string> scriptsPath = FileIO.SearchAFolderAboveTheCurrentDirectoryOfTheApplication(Settings.ScriptsPath_JsScripts); // find the folder with the scripts
            if (scriptsPath.IsFailure) throw new InvalidOperationException("scripts folder not found");
            jsScriptingContext = ScriptingContext.ScriptingContextWithRealFs(scriptsPath.Value);
        }

        [Benchmark]
        public void ClearScript_ObjectCloning_with_Stringify()
        {
            Init();
            JsScriptRunner jsScriptRunner = JsScriptRunner.RunnerWithContext(JsScriptRunnerType.ClearScript, jsScriptingContext, Settings.ScriptingContextName);
            var testObj = new ObjectCloning(jsScriptRunner);

            for (int i = 0; i < loopCycle; i++)
            {
                testObj.ObjectCloning_with_Stringify();
            }
        }

        [Benchmark]
        public void ClearScript_ObjectCloning_with_Lodash()
        {
            Init();
            JsScriptRunner jsScriptRunner = JsScriptRunner.RunnerWithContext(JsScriptRunnerType.ClearScript, jsScriptingContext, Settings.ScriptingContextName);
            var testObj = new ObjectCloning(jsScriptRunner);

            testObj.DONT_WORK_WITH_JINT_ObjectCloning_with_Lodash__Init();

            for (int i = 0; i < loopCycle; i++)
            {
                testObj.DONT_WORK_WITH_JINT_ObjectCloning_with_Lodash__Run();
            }
        }

        [Benchmark]
        public void ClearScript_ObjectCloning_with_rfdc()
        {
            Init();
            JsScriptRunner jsScriptRunner = JsScriptRunner.RunnerWithContext(JsScriptRunnerType.ClearScript, jsScriptingContext, Settings.ScriptingContextName);
            var testObj = new ObjectCloning(jsScriptRunner);

            testObj.ObjectCloning_with_rfdc__Init();

            for (int i = 0; i < loopCycle; i++)
            {
                testObj.ObjectCloning_with_rfdc__Run();
            }
        }

        [Benchmark]
        public void Jint_ObjectCloning_with_Stringify()
        {
            Init();
            JsScriptRunner jsScriptRunner = JsScriptRunner.RunnerWithContext(JsScriptRunnerType.Jint, jsScriptingContext, Settings.ScriptingContextName);
            var testObj = new ObjectCloning(jsScriptRunner);

            for (int i = 0; i < loopCycle; i++)
            {
                testObj.ObjectCloning_with_Stringify();
            }
        }

        [Benchmark]
        public void Jint_ObjectCloning_with_rfdc()
        {
            Init();
            JsScriptRunner jsScriptRunner = JsScriptRunner.RunnerWithContext(JsScriptRunnerType.Jint, jsScriptingContext, Settings.ScriptingContextName);
            var testObj = new ObjectCloning(jsScriptRunner);

            testObj.ObjectCloning_with_rfdc__Init();

            for (int i = 0; i < loopCycle; i++)
            {
                testObj.ObjectCloning_with_rfdc__Run();
            }
        }
    }
}
