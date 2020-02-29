using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using Scripting.Tests;
using BenchmarkDotNet.Reports;

namespace StartupProject
{
    class Program
    {
        static void Main(string[] args)
        {
            // STEPS TO RUN THE BENCHMARK:
            // * write the code to benchmark in the class "ClassToBenchmark"
            // * set STARTUP PROJECT to this project
            // * SET BUILD TYPE to "RELEASE" 
            // * RUN THE PROJECT from VISUAL STUDIO *WITHOUT DEBUG* (with CTRL + F5)

            // docs
            // from https://benchmarkdotnet.org/articles/overview.html
            // https://github.com/dotnet/BenchmarkDotNet/issues/1073
            // https://benchmarkdotnet.org/api/BenchmarkDotNet.Configs.ConfigOptions.html

            Console.WriteLine("starting");

            // Run Benchmark WITH OPTIMIZATION VALIDATOR
            //var summary = BenchmarkRunner.Run<ClassToBenchmark>();

            // Run Benchmark WITHOUT OPTIMIZATION VALIDATOR (WHATEVER IT MEANS)
            var config = DefaultConfig.Instance.With(ConfigOptions.DisableOptimizationsValidator);
            Summary summary;
            summary = BenchmarkRunner.Run<ObjectCloning_with_Stringify_rfdc_Lodash_Benchmark>(config);
        }
    }
}
