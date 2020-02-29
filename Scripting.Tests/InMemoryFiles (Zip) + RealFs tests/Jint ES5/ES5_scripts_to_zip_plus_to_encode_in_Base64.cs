using Scripting.Js.v1;
using System.Collections.Immutable;

namespace Scripting.Tests
{
    // this file contains the Base64 encoding of the zip file 'ES5_scripts_to_zip_plus_to_encode_in_Base64.zip'.
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
    public static class ES5_scripts_to_zip_plus_to_encode_in_Base64
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
