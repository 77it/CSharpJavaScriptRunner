using Scripting.Js.v1;
using System.Collections.Immutable;

namespace Scripting.Tests
{
    // this file contains the Base64 encoding of the zip file 'ES6_scripts_to_zip_plus_to_encode_in_Base64.zip'.
    // the Base64 encoded .zip file is stored in the local variable 'encodedZip'.
    //
    // intructions on Base64 encoding
    /*
    how to encode:
    * convert Js zip files to a Base64 encoded string with https://base64.guru/converter/encode/file
    * test conversion of Base64 encoded string back to Js with https://base64.guru/converter/decode/file

    base64 info:
    * is a file format with only letters, numbers, +, /, =
    * https://en.wikipedia.org/wiki/Base64
    * https://en.wikipedia.org/wiki/Binary-to-text_encoding
    */
    public static class ES6_scripts_to_zip_plus_to_encode_in_Base64
    {
        public static ImmutableDictionary<string, string> GetScripts()
        {
            var builder = ImmutableDictionary.CreateBuilder<string, string>();

            // scripts file with content of file "./InMemoryFiles/Scripts to zip + encoded in Base64.zip"
            var encodedZip = InMemoryScript.FromZipFileEncodedInBase64(
                path: "",
                contents: @"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"
            );

            // load scripts in the dictionary
            builder.AddRange(encodedZip.GetScript());

            return builder.ToImmutable();
        }
    }
}