using System;
using System.Data;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scripting.Js.v1;
using CSharpFunctionalExtensions;
using System.IO;
using System.Threading;
using Scripting.Js.v1;

namespace Scripting.Tests
{
    [TestClass]
    public class Scripting_TemporaryTests
    {
        private (string ScriptsPath, ScriptingContext JsScriptingContext) InitWithRealFs()
        {
            Result<string> scriptsPath = FileIO.SearchAFolderAboveTheCurrentDirectoryOfTheApplication(Scripting_TestSettings.ScriptsPath_JsScripts); // find the folder with the scripts
            if (scriptsPath.IsFailure) throw new InvalidOperationException("scripts folder not found");
            //return (scriptsPath.Value, ScriptingContext.ScriptingContextWithInMemoryFs(InMemoryScripts.GetScripts()));
            return (scriptsPath.Value, ScriptingContext.ScriptingContextWithRealFs(scriptsPath.Value));
        }

        [TestMethod]
        public void Scripting_TestCodeWithoutDebug()
        {
            Assert.Inconclusive(); return;

            Console.WriteLine("BEWARE: scripting projects and classes used by them CANNOT be obfuscated!!!"); Console.WriteLine();

            (string scriptsPath, ScriptingContext jsScriptingContext) = InitWithRealFs();

            JsScriptRunner jsScriptRunner = JsScriptRunner.RunnerWithContext(
                JsScriptRunnerType.ClearScript,
                jsScriptingContext,
                Scripting_TestSettings.ScriptingContextName);

            jsScriptRunner.Run("var exports = {};");  // used for exports of 'main.js'
            jsScriptRunner.RunScriptFile(System.IO.Path.Combine(scriptsPath, "./lib/myrequire.js"));  // execute myrequire.js script from real FS
            jsScriptRunner.RunScriptFile(System.IO.Path.Combine(scriptsPath, "main.js"));  // execute main.js script from real FS
            //jsScriptRunner.Run(jsScriptingContext.ReadFile("./main.js"));  // execute main.js script from virtual FS

            Console.WriteLine("end of execution...");
        }
    }
}
