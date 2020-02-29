using System;
using System.Data;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpFunctionalExtensions;
using System.IO;
using System.Threading;
using Scripting.Js.v1;

namespace Scripting.Tests
{
    [TestClass]
    public class Scripting_DebugCode
    {
        private (string ScriptsPath, ScriptingContext JsScriptingContext) InitWithRealFs()
        {
            Result<string> scriptsPath = FileIO.SearchAFolderAboveTheCurrentDirectoryOfTheApplication(Scripting_TestSettings.ScriptsPath_JsScripts); // find the folder with the scripts
            if (scriptsPath.IsFailure) throw new InvalidOperationException("scripts folder not found");
            //return (scriptsPath.Value, ScriptingContext.ScriptingContextWithInMemoryFs(InMemoryScripts.GetScripts()));
            return (scriptsPath.Value, ScriptingContext.ScriptingContextWithRealFs(scriptsPath.Value));
        }

        [TestMethod]
        public void Scripting_DebugWithVisualStudioCode()
        {
            Assert.Inconclusive(); return;

            Console.WriteLine("BEWARE: scripting projects and classes used by them CANNOT be obfuscated!!!"); Console.WriteLine();

            (string scriptsPath, ScriptingContext jsScriptingContext) = InitWithRealFs();

            JsScriptRunner jsScriptRunner = JsScriptRunner.RunnerWithContext(
                JsScriptRunnerType.ClearScriptDebugMode,
                jsScriptingContext,
                Scripting_TestSettings.ScriptingContextName);

            // notes from https://github.com/microsoft/ClearScript/issues/159 + https://github.com/microsoft/ClearScript/issues/24 
            // 0) setup Visual Studio Code(see "VII. Debugging with ClearScript and V8"  https://microsoft.github.io/ClearScript/Details/Build.html )
            // 1) start the V8 engine with V8ScriptEngineFlags.EnableDebugging + V8ScriptEngineFlags.AwaitDebuggerAndPauseOnStart option.
            //    it's already done by JsScriptRunner with 'type' parameter set to JsScriptRunnerType.ClearScriptDebugMode
            // 2) run the code from Visual Studio (in debug or not-debug mode, the code will always wait for Visual Studio Code)
            // 3) start VSCode Debug (menu | Debug | Start Debugging)
            // At this point VSCode connects and stops at "debugger;" command. All scripts are loaded and accessible.

            jsScriptRunner.Run("debugger;");  // PUT A STOP HERE AND START VSCODE NOW!
            jsScriptRunner.Run("var exports = {};");  // used for exports of 'main.js'
            jsScriptRunner.RunScriptFile(System.IO.Path.Combine(scriptsPath, "./lib/myrequire.js"));  // execute myrequire+fs.js script from real FS
            jsScriptRunner.RunScriptFile(System.IO.Path.Combine(scriptsPath, "main.js"));  // execute main.js script from real FS
            //jsScriptRunner.Run(JsScriptingContext.ReadFile("./main.js"));  // execute main.js script from virtual FS
            jsScriptRunner.Run("debugger;");  // execute main.js script from virtual FS

            Console.WriteLine("end of debug...");
        }
    }
}
