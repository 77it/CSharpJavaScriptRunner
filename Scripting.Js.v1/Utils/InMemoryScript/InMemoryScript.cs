using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Scripting.Js.v1
{
    // intructions on Base64 encoding
    /*
    how to encode:
    * convert Js to a Base64 encoded string with https://base64.guru/converter/encode/file
    * test conversion of Base64 encoded string back to Js with https://base64.guru/converter/decode/file

    base64 info:
    * is a file format with only letters, numbers, +, /, =
    * https://en.wikipedia.org/wiki/Base64
    * https://en.wikipedia.org/wiki/Binary-to-text_encoding
    */
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class InMemoryScript
    {
        private InMemoryScriptTypes Type { get; }
        private string ScriptPath { get; }
        private string ScriptValue { get; }

        public static InMemoryScript FromPlainString(string path, string contents)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException($"{nameof(path)} can't be null or whiteSpace");
            if (contents is null) throw new ArgumentNullException(nameof(contents));
            return new InMemoryScript(InMemoryScriptTypes.PlainString, path, contents);
        }

        public static InMemoryScript FromTextFileEncodedInBase64(string path, string contents)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException($"{nameof(path)} can't be null or whiteSpace");
            if (contents is null) throw new ArgumentNullException(nameof(contents));
            return new InMemoryScript(InMemoryScriptTypes.TextFileEncodedInBase64, path, contents);
        }

        /// <summary>
        /// The path paramater (can be also "") is used to prepend a path to the returned paths taken from the zip file
        /// </summary>
        /// <returns></returns>
        public static InMemoryScript FromZipFileEncodedInBase64(string path, string contents)
        {
            if (path is null) throw new ArgumentNullException(nameof(path));
            if (contents is null) throw new ArgumentNullException(nameof(contents));
            return new InMemoryScript(InMemoryScriptTypes.ZipFileEncodedInBase64, path, contents);
        }

        private InMemoryScript(InMemoryScriptTypes type, string scriptPath, string scriptValue)  // init only with static methods
        {
            Type = type;
            ScriptPath = scriptPath;
            ScriptValue = scriptValue;
        }

        public IEnumerable<KeyValuePair<string, string>> GetScript()
        {
            if (Type == InMemoryScriptTypes.PlainString)
            {
                return new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(ScriptPath, ScriptValue) };
            }
            else if (Type == InMemoryScriptTypes.TextFileEncodedInBase64)
            {
                // see https://stackoverflow.com/questions/7134837/how-do-i-decode-a-base64-encoded-string
                byte[] data = Convert.FromBase64String(ScriptValue);
                return new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(ScriptPath, Encoding.UTF8.GetString(data)) };
            }
            else if (Type == InMemoryScriptTypes.ZipFileEncodedInBase64)
            {
                var retList = new List<KeyValuePair<string, string>>();
                using MemoryStream stream = new MemoryStream(Convert.FromBase64String(ScriptValue));  // decode the Base64 string to a stream, in memory  // see https://stackoverflow.com/questions/25919387/converting-file-into-base64string-and-back-again
                using ZipArchive zip = new ZipArchive(stream);  // open the zip file  // see https://stackoverflow.com/a/22605118/5288052
                foreach (ZipArchiveEntry entry in zip.Entries)  // loop zip file entries, saving only files (entry.Name not empty)
                {
                    if (!(string.IsNullOrEmpty(entry.Name)))  // see https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchiveentry.name
                    {
                        using StreamReader sr = new StreamReader(entry.Open());
                        string zipContent = sr.ReadToEnd();
                        retList.Add(new KeyValuePair<string, string>($"{ScriptPath}{entry.FullName}", zipContent));  // save path and content from the zip file, prepending 'ScriptPath' to the path
                    }
                }
                return retList;
            }
            else
                throw new InvalidOperationException($"{nameof(Type)} not recognized");
        }

        public enum InMemoryScriptTypes
        {
            PlainString = 0,
            TextFileEncodedInBase64,
            ZipFileEncodedInBase64
        }
    }
}