// #ObjectCloning-Benchmark_id_code

// RESULTS 
/*
Time for 10_000 cycle
|                                   Method |    Mean |    Error |   StdDev |  Median |       Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------------------------- |--------:|---------:|---------:|--------:|------------:|------:|------:|----------:|
| ClearScript_ObjectCloning_with_Stringify | 1.876 s | 0.1915 s | 0.5647 s | 1.562 s |   5000.0000 |     - |     - |   8.12 MB |
|    ClearScript_ObjectCloning_with_Lodash | 1.438 s | 0.0770 s | 0.2247 s | 1.373 s |   5000.0000 |     - |     - |   8.33 MB |
|      ClearScript_ObjectCloning_with_rfdc | 1.684 s | 0.1191 s | 0.3511 s | 1.636 s |   5000.0000 |     - |     - |   8.11 MB |
|        Jint_ObjectCloning_with_Stringify | 2.895 s | 0.2476 s | 0.7105 s | 2.524 s | 438000.0000 |     - |     - | 655.95 MB |
|             Jint_ObjectCloning_with_rfdc | 2.706 s | 0.0532 s | 0.0444 s | 2.703 s | 363000.0000 |     - |     - | 546.12 MB |

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
using System;
using System.Data;
using System.Collections.Immutable;
using Scripting.Js.v1;
using System.IO;
using Scripting;

namespace Scripting.Tests
{
    // from https://benchmarkdotnet.org/articles/overview.html
    [MemoryDiagnoser]
    public class ObjectCloning_with_Stringify_rfdc_Lodash_Benchmark
    {
        private const int loopCycle = 10000;

        private ScriptingContext jsScriptingContext { get; set; }

        private void Init()
        {
            Result<string> scriptsPath = FileIO.SearchAFolderAboveTheCurrentDirectoryOfTheApplication(Scripting_TestSettings.ScriptsPath_JsScripts); // find the folder with the scripts
            if (scriptsPath.IsFailure) throw new InvalidOperationException("scripts folder not found");
            jsScriptingContext = ScriptingContext.ScriptingContextWithRealFs(scriptsPath.Value);
        }

        [Benchmark]
        public void ClearScript_ObjectCloning_with_Stringify()
        {
            Init();
            JsScriptRunner jsScriptRunner = JsScriptRunner.RunnerWithContext(JsScriptRunnerType.ClearScript, jsScriptingContext, Scripting_TestSettings.ScriptingContextName);
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
            JsScriptRunner jsScriptRunner = JsScriptRunner.RunnerWithContext(JsScriptRunnerType.ClearScript, jsScriptingContext, Scripting_TestSettings.ScriptingContextName);
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
            JsScriptRunner jsScriptRunner = JsScriptRunner.RunnerWithContext(JsScriptRunnerType.ClearScript, jsScriptingContext, Scripting_TestSettings.ScriptingContextName);
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
            JsScriptRunner jsScriptRunner = JsScriptRunner.RunnerWithContext(JsScriptRunnerType.Jint, jsScriptingContext, Scripting_TestSettings.ScriptingContextName);
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
            JsScriptRunner jsScriptRunner = JsScriptRunner.RunnerWithContext(JsScriptRunnerType.Jint, jsScriptingContext, Scripting_TestSettings.ScriptingContextName);
            var testObj = new ObjectCloning(jsScriptRunner);

            testObj.ObjectCloning_with_rfdc__Init();

            for (int i = 0; i < loopCycle; i++)
            {
                testObj.ObjectCloning_with_rfdc__Run();
            }
        }
    }
}
