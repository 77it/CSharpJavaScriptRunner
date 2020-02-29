using CSharpFunctionalExtensions;
using Jint;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripting.Js.v1
{
    public class JsScriptRunner
    {
        public Maybe<object> ScriptingContext { get; }
        private Engine JintEngine { get; }
        private V8ScriptEngine ClearScriptEngine { get; }
        private JsScriptRunnerType Type { get; }

        public static JsScriptRunner RunnerWithoutContext(JsScriptRunnerType type)
        {
            return new JsScriptRunner(type: type, scriptingContext: null, contextName: null);
        }

        public static JsScriptRunner RunnerWithContext(JsScriptRunnerType type, object scriptingContext, string contextName)
        {
            if (scriptingContext is null) throw new ArgumentNullException(nameof(scriptingContext));
            if (contextName is null) throw new ArgumentNullException(nameof(contextName));

            return new JsScriptRunner(type: type, scriptingContext: scriptingContext, contextName: contextName);
        }

        private JsScriptRunner(JsScriptRunnerType type, object scriptingContext = null, string contextName = null)
        {
            if (!(scriptingContext is null) && contextName is null) throw new ArgumentNullException(nameof(contextName));

            ScriptingContext = scriptingContext;
            Type = type;

            switch (type)
            {
                case JsScriptRunnerType.Jint:
                    {
                        // engine settings:
                        // strict mode   https://stackoverflow.com/a/34302448   https://stackoverflow.com/questions/34301881/should-i-use-strict-for-every-single-javascript-function-i-write 
                        JintEngine = new Engine(cfg => cfg.Strict(true));
                        //JintEngine = new Engine();
                        if (!(scriptingContext is null))
                            JintEngine.SetValue(contextName, scriptingContext);  // pass 'scriptingConnector' to Js
                        break;
                    }
                case JsScriptRunnerType.ClearScript:
                    {
                        //ClearScriptEngine = new V8ScriptEngine();
                        //ClearScriptEngine = new V8ScriptEngine(V8ScriptEngineFlags.EnableDateTimeConversion);
                        ClearScriptEngine = new V8ScriptEngine(V8ScriptEngineFlags.EnableDateTimeConversion | V8ScriptEngineFlags.DisableGlobalMembers);
                        ClearScriptEngine.AllowReflection = false;
                        //ClearScriptEngine.DefaultAccess = Microsoft.ClearScript.ScriptAccess.Full;
                        //ClearScriptEngine.DefaultAccess = Microsoft.ClearScript.ScriptAccess.None;
                        if (!(scriptingContext is null))
                            ClearScriptEngine.AddHostObject(contextName, scriptingContext);  // pass 'scriptingConnector' to Js
                        break;
                    }
                case JsScriptRunnerType.ClearScriptDebugMode:
                    {
                        ClearScriptEngine = new V8ScriptEngine(V8ScriptEngineFlags.EnableDateTimeConversion
                            | V8ScriptEngineFlags.DisableGlobalMembers
                            | V8ScriptEngineFlags.EnableDebugging
                            | V8ScriptEngineFlags.AwaitDebuggerAndPauseOnStart);
                        /* ClearScriptEngine = new V8ScriptEngine(V8ScriptEngineFlags.EnableDateTimeConversion
                            | V8ScriptEngineFlags.DisableGlobalMembers
                            | V8ScriptEngineFlags.EnableDebugging
                            | V8ScriptEngineFlags.EnableRemoteDebugging
                            | V8ScriptEngineFlags.AwaitDebuggerAndPauseOnStart);*/
                        ClearScriptEngine.AllowReflection = false;
                        if (!(scriptingContext is null))
                            ClearScriptEngine.AddHostObject(contextName, scriptingContext);  // pass 'scriptingConnector' to Js
                        break;
                    }
            }
        }

        public void AddHostObject(object hostObject, string name)
        {
            switch (Type)
            {
                case JsScriptRunnerType.Jint:
                    {
                        JintEngine.SetValue(name, hostObject);  // pass 'scriptingConnector' to Js
                        break;
                    }
                case JsScriptRunnerType.ClearScriptDebugMode:  // fallback to JsScriptRunnerType.ClearScript
                case JsScriptRunnerType.ClearScript:
                    {
                        ClearScriptEngine.AddHostObject(name, hostObject);  // pass 'scriptingConnector' to Js
                        break;
                    }
            }
        }

        /// <summary>
        /// Load a folder content and run all scripts inside it.
        /// sort files with .OrderBy(x => x, StringComparer.OrdinalIgnoreCase) before executing them
        /// </summary>
        public void RunScriptFilesFromAFolder(string path)
        {
            foreach (string file in FileIO.ListAllFilesInAPathRecursively(path).OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                RunScriptFile(file);
        }

        /// <summary>
        /// Run a script from a file
        /// </summary>
        public void RunScriptFile(string file)
        {
            if (file is null) throw new ArgumentNullException(nameof(file));
            Run(System.IO.File.ReadAllText(file, System.Text.Encoding.UTF8));
        }

        public void RunScriptsTexts(IEnumerable<string> scripts)
        {
            if (scripts is null) throw new ArgumentNullException(nameof(scripts));
            foreach (var script in scripts)
                Run(script);
        }

        public void Run(string script, bool discardFromDebugView = false)
        {
            if (script is null) throw new ArgumentNullException(nameof(script));
            switch (Type)
            {
                case JsScriptRunnerType.Jint:
                    {
                        try
                        {
                            JintEngine.Execute(script);
                            break;
                        }
                        catch (Esprima.ParserException ex)
                        {
                            throw new ApplicationException($"{ex.Error} (Line {ex.LineNumber}, Column {ex.Column}), -> {ReadLine(script, ex.LineNumber)}", ex);
                        }
                        catch (Jint.Runtime.JavaScriptException ex)  // from https://github.com/sebastienros/jint/issues/112
                        {
                            throw new ApplicationException($"{ex.Error} (Line {ex.LineNumber}, Column {ex.Column}), -> {ReadLine(script, ex.LineNumber)}", ex);
                        }
                    }
                case JsScriptRunnerType.ClearScriptDebugMode:
                    {
                        try
                        {
                            if (discardFromDebugView)
                                // see https://microsoft.github.io/ClearScript/Reference/html/M_Microsoft_ClearScript_ScriptEngine_Execute_2.htm
                                ClearScriptEngine.Execute(string.Empty, true, script);
                            else
                                ClearScriptEngine.Execute(ReadAndNormalizeFirstNonEmptyLineOfAScript(script), false, script);
                            break;
                        }
                        catch (Microsoft.ClearScript.ScriptEngineException ex)  // from https://github.com/microsoft/ClearScript/issues/16
                        {
                            throw new ApplicationException($"{ex.ErrorDetails}", ex);
                        }
                    }
                case JsScriptRunnerType.ClearScript:
                    {
                        try
                        {
                            // see https://microsoft.github.io/ClearScript/Reference/html/M_Microsoft_ClearScript_ScriptEngine_Execute_2.htm
                            ClearScriptEngine.Execute(script);
                            break;
                        }
                        catch (Microsoft.ClearScript.ScriptEngineException ex)  // from https://github.com/microsoft/ClearScript/issues/16
                        {
                            throw new ApplicationException($"{ex.ErrorDetails}", ex);
                        }
                    }
            }
        }

        /// <summary>
        /// Call a Js function by string, passing a single string argument to it (the argument can be a Json object parsed inside the function)
        /// </summary>
        public void JsRun(string function, string argument = null)
        {
            if (function is null) throw new ArgumentNullException(nameof(function));
            switch (Type)
            {
                case JsScriptRunnerType.Jint:
                    {
                        try
                        {
                            JintEngine.Invoke(function, argument);  // execute the function passing the argument
                            break;
                        }
                        catch (Esprima.ParserException ex)
                        {
                            throw new ApplicationException($"{ex.Error} (Line {ex.LineNumber}, Column {ex.Column})", ex);
                        }
                        catch (Jint.Runtime.JavaScriptException ex)  // from https://github.com/sebastienros/jint/issues/112
                        {
                            throw new ApplicationException($"{ex.Error} (Line {ex.LineNumber}, Column {ex.Column})", ex);
                        }
                    }
                case JsScriptRunnerType.ClearScriptDebugMode:  // fallback to JsScriptRunnerType.ClearScript
                case JsScriptRunnerType.ClearScript:
                    {
                        try
                        {
                            ClearScriptEngine.Invoke(function, argument);  // execute the function passing the argument
                            break;
                        }
                        catch (Microsoft.ClearScript.ScriptEngineException ex)  // from https://github.com/microsoft/ClearScript/issues/16
                        {
                            throw new ApplicationException($"{ex.ErrorDetails}", ex);
                        }
                    }
            }
        }

        /// <summary>
        /// Evaluate a code and return a value
        /// </summary>
        /// <param name="discardFromDebugView">true to discard from Debug View, in Debug mode (used to prevent pollution of executed modules in Visual Studio Code)</param>
        public object Evaluate(string script, bool discardFromDebugView = false)
        {
            if (script is null) throw new ArgumentNullException(nameof(script));
            switch (Type)
            {
                case JsScriptRunnerType.Jint:
                    {
                        try
                        {
                            return JintEngine.Execute(script)  // from https://github.com/sebastienros/jint
                                .GetCompletionValue()  // get the latest statement completion value
                                .ToObject();  // converts the value to .NET
                        }
                        catch (Esprima.ParserException ex)
                        {
                            throw new ApplicationException($"{ex.Error} (Line {ex.LineNumber}, Column {ex.Column}), -> {ReadLine(script, ex.LineNumber)}", ex);
                        }
                        catch (Jint.Runtime.JavaScriptException ex)  // from https://github.com/sebastienros/jint/issues/112
                        {
                            throw new ApplicationException($"{ex.Error} (Line {ex.LineNumber}, Column {ex.Column}), -> {ReadLine(script, ex.LineNumber)}", ex);
                        }
                    }
                case JsScriptRunnerType.ClearScriptDebugMode:
                    {
                        try
                        {
                            if (discardFromDebugView)
                                return ClearScriptEngine.Evaluate(string.Empty, true, script);
                            else
                                // see https://microsoft.github.io/ClearScript/Reference/html/M_Microsoft_ClearScript_ScriptEngine_Evaluate_2.htm
                                return ClearScriptEngine.Evaluate(ReadAndNormalizeFirstNonEmptyLineOfAScript(script), false, script);
                        }
                        catch (Microsoft.ClearScript.ScriptEngineException ex)  // from https://github.com/microsoft/ClearScript/issues/16
                        {
                            throw new ApplicationException($"{ex.ErrorDetails}", ex);
                        }
                    }
                case JsScriptRunnerType.ClearScript:
                    {
                        try
                        {
                            // see https://microsoft.github.io/ClearScript/Reference/html/M_Microsoft_ClearScript_ScriptEngine_Evaluate_2.htm
                            return ClearScriptEngine.Evaluate(script);
                        }
                        catch (Microsoft.ClearScript.ScriptEngineException ex)  // from https://github.com/microsoft/ClearScript/issues/16
                        {
                            throw new ApplicationException($"{ex.ErrorDetails}", ex);
                        }
                    }
            }
            throw new InvalidOperationException("unexpected case");
        }

        public void CollectGarbage(bool exhaustive = false)
        {
            switch (Type)
            {
                case JsScriptRunnerType.Jint:
                    {
                        break;  // do nothing, garbage collection not implemeted in Jint
                    }
                case JsScriptRunnerType.ClearScriptDebugMode:  // fallback to JsScriptRunnerType.ClearScript
                case JsScriptRunnerType.ClearScript:
                    {
                        ClearScriptEngine.CollectGarbage(exhaustive);  // see https://microsoft.github.io/ClearScript/Reference/html/M_Microsoft_ClearScript_V8_V8ScriptEngine_CollectGarbage.htm
                        break;
                    }
            }
        }

        // used to build the description/title of a script run in debug mode
        private static string ReadAndNormalizeFirstNonEmptyLineOfAScript(string text)
        {
            using var reader = new System.IO.StringReader(text);

            string line;

            do
            {
                line = reader.ReadLine();
                if (!(string.IsNullOrWhiteSpace(line)))
                    return Normalize(line);
            }
            while (line != null);

            return string.Empty;

            // from https://stackoverflow.com/a/3210462/5288052 + https://stackoverflow.com/questions/3210393/how-do-i-remove-all-non-alphanumeric-characters-from-a-string-except-dash
            string Normalize(string stringToNormalize)
            {
                char[] arr = stringToNormalize.ToCharArray();
                arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c)
                                                  || char.IsWhiteSpace(c)
                                                  || c == '-'
                                                  || c == '_'
                                                  || c == '{'
                                                  || c == '}'
                                                  || c == '['
                                                  || c == ']'
                                                  || c == '('
                                                  || c == ')'
                                                  )));
                return new string(arr);
            }
        }

        // used to build error messages
        private static string ReadLine(string text, int lineNumber)
        {
            using var reader = new System.IO.StringReader(text);

            string line;
            int currentLineNumber = 0;

            do
            {
                currentLineNumber += 1;
                line = reader.ReadLine();
            }
            while (line != null && currentLineNumber < lineNumber);

            return (currentLineNumber == lineNumber) ? line : string.Empty;
        }
    }
}


// [1] ClearScript, code to Tr Catch from JavaScript, from https://github.com/microsoft/ClearScript/issues/39 
/*
You can catch the object in JavaScript code and return it to the host.

Here's a sample extension method that runs arbitrary code and returns either its return value or any exception object thrown:

    public static class ScriptEngineExtensions {
        public static object TryEvaluate(this ScriptEngine engine, string code) {
            object tryCatch = engine.Script.__tryCatch;
            if (tryCatch is Undefined) {
                tryCatch = engine.Evaluate(@"
                    __tryCatch = function (code) {
                        try {
                            return { succeeded: true, returnValue: eval(code) };
                        }
                        catch (exception) {
                            return { succeeded: false, exception: exception };
                        }
                    }
                ");
            }
            return ((dynamic)tryCatch)(code);
        }
    }

And here's how you might use it:

    dynamic result = engine.TryEvaluate("Math.PI");
    Console.WriteLine(result.succeeded); // True
    Console.WriteLine(result.returnValue); // 3.14159265358979

    result = engine.TryEvaluate("throw { foo: 123, bar: 456 }");
    Console.WriteLine(result.succeeded); // False
    Console.WriteLine(result.exception.foo); // 123
    Console.WriteLine(result.exception.bar); // 456

This of course is just one possibility. Hopefully it gives you an idea of how the host and script engine can work together.

*/
