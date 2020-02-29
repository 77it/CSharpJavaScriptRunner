# C# JavaScript Runner

This is a C# .NET Core project that shows usage of [ClearScript](https://github.com/microsoft/ClearScript/) and [Jint](https://github.com/sebastienros/jint) engines to run JavaScript code, using also other JavaScript libraries (as @hapi/formula etc).

Code is MIT licensed as ClearScript.

Highlights:
* starting from [some barebone code](https://github.com/musikele/require-example/) that mimic the functionality of require/CommonJs and his [explanation](https://michelenasti.com/2018/10/02/let-s-write-a-simple-version-of-the-require-function.html) I adapted it for this project [(myrequire.js)](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.JsScripts.v1/lib/myrequire.js)
* implemented optional use of [Base64 encoded zip files](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.Js.v1/Utils/InMemoryScript/InMemoryScript.cs) to store an entire folder of Js files and libraries in [a single C# string](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.Tests/InMemoryFiles%20(Zip)%20%2B%20RealFs%20tests/ClearScript%20ES6/ES6_scripts_to_zip_plus_to_encode_in_Base64.cs). [Working tests and assets](https://github.com/stefano77it/CSharpJavaScriptRunner/tree/master/Scripting.Tests/InMemoryFiles%20(Zip)%20%2B%20RealFs%20tests) provided.
* built a virtual filesystem (using simply a dictionary inside the [scripting context](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.Js.v1/ScriptingContext/ScriptingContext.cs)) to intercept requests of files stored in memory
* wrote a simple [JavaScript filesystem API layer](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.JsScripts.v1/lib/myfs.ts) that calls Node.js and C# methods to be able to read/write a file without worrying if the script is executed in memory or against a real fs, from ClearScript or Node

When possible I used TypeScript, to allow debug/static typing.


# Interesting files

## Scripting.Js.v1 project

Interesting files:
* [JavaScript code runner](https://github.com/stefano77it/CSharpJavaScriptRunner/tree/master/Scripting.Js.v1/JsScriptRunner)
* [scripting context](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.Js.v1/ScriptingContext/ScriptingContext.cs)
* [object to store the JavaScript scripts in memory](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.Js.v1/Utils/InMemoryScript/InMemoryScript.cs) (plain text, Base64 encoded text file or Zip file)

## Scripting.JsScripts.v1

Interesting files:
* [main JavaScript file](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.JsScripts.v1/main.ts) used to test `require`, filesystem and external libraries
* [custom require](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.JsScripts.v1/lib/myrequire.js)
* [custom filesystem API layer](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.JsScripts.v1/lib/myfs.ts)
* [flag file](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.JsScripts.v1/zzz%20flagfile) generated for testing purposes and [sample library to test exports](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.JsScripts.v1/lib/test.js)
* other files in the [lib folder](https://github.com/stefano77it/CSharpJavaScriptRunner/tree/master/Scripting.JsScripts.v1/lib) are external JavaScript libraries, adapted for execution with ES5 engines if needed

## Scripting.Tests

Interesting files:
* [tests of InMemoryFiles (Zip) and JavaScript stored in a real filesystem](https://github.com/stefano77it/CSharpJavaScriptRunner/tree/master/Scripting.Tests/InMemoryFiles%20(Zip)%20%2B%20RealFs%20tests); tested [ES6 code with ClearScript](https://github.com/stefano77it/CSharpJavaScriptRunner/tree/master/Scripting.Tests/InMemoryFiles%20(Zip)%20%2B%20RealFs%20tests/ClearScript%20ES6) and [ES5 with Jint](https://github.com/stefano77it/CSharpJavaScriptRunner/tree/master/Scripting.Tests/InMemoryFiles%20(Zip)%20%2B%20RealFs%20tests/Jint%20ES5)
* tests of [Main.js script](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.JsScripts.v1/main.ts) with both [ClearScript](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.Tests/Main.js/Scripting_Main_Tests_With_ClearScript.cs) and [Node.js exe](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.Tests/Main.js/Scripting_Main_Tests_With_Nodejs.cs). Node.js exe path can be set in [TestSettings](https://github.com/stefano77it/CSharpJavaScriptRunner/blob/master/Scripting.Tests/_TestSettings.cs)
