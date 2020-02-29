using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using CSharpFunctionalExtensions;
using Scripting.Js.v1;

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