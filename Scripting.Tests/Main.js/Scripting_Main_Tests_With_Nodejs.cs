using System;
using System.Data;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.Js.v1;
using CSharpFunctionalExtensions;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Scripting.Tests
{
    [TestClass]
    public class Scripting_Main_Tests_With_Nodejs
    {
        private (string ScriptsPath, ScriptingContext JsScriptingContext) InitWithRealFs()
        {
            Result<string> scriptsPath = FileIO.SearchAFolderAboveTheCurrentDirectoryOfTheApplication(Scripting_TestSettings.ScriptsPath_JsScripts); // find the folder with the scripts
            if (scriptsPath.IsFailure) throw new InvalidOperationException("scripts folder not found");
            return (scriptsPath.Value, ScriptingContext.ScriptingContextWithRealFs(scriptsPath.Value));
        }

        [TestMethod]
        public void Scripting_Main_Test_With_Nodejs()
        {
            (string scriptsPath, ScriptingContext jsScriptingContext) = InitWithRealFs();

            JsScriptRunner jsScriptRunner = JsScriptRunner.RunnerWithContext(
                JsScriptRunnerType.ClearScript,
                jsScriptingContext,
                Scripting_TestSettings.ScriptingContextName);

            CallNodeJs("main.js", scriptsPath);  // execute main.js script from real FS with node.js
            Assert.AreEqual("flag value", jsScriptingContext.ReadFile("zzz flagfile"));  // test output of Js scripts

            static void CallNodeJs(string mainScriptName, string scriptsPath)  // inspired to https://www.dotnetperls.com/process
            {
                if (Scripting_TestSettings.SkipNodeTests)
                {
                    Assert.Inconclusive();
                    return;
                }

                // Part 1: use ProcessStartInfo class.
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = Scripting_TestSettings.NodeExeCmdLine;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.WorkingDirectory = scriptsPath;

                // Part 2: set arguments.
                startInfo.Arguments = System.IO.Path.Combine(scriptsPath, mainScriptName);

                // Part 3: start with the info we specified.
                // ... Call WaitForExit.
                using Process exeProcess = Process.Start(startInfo);
                exeProcess.WaitForExit();
            }
        }
    }
}
