using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// Install-Package BenchmarkDotNet -Version 0.13.0
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace TestConsole9
{    
    public class BenchmarkTests {
        public static void Run() {
            var summary = BenchmarkRunner.Run<BenchmarkClass>();
        }
    }

    [MemoryDiagnoser]
    public class BenchmarkClass
    {
        private string data;

        [GlobalSetup]
        public void Setup()
        {
            data = "2021-07-14";
        }

        [Benchmark]
        public void Test1()
        {
            var year = this.data.Split('-')[0];            
        }

        [Benchmark]
        public void Test2()
        {
            var year = this.data.Substring(0, 4);            
        }

        [Benchmark]
        public void Test3()
        {
            var match = Regex.Match(this.data, @"(?<y>\d{4})-(?<m>\d{2})-(?<d>\d{2})");
            var year = match.Groups["y"].Value;            
        }

        [Benchmark]
        public void Test4()
        {
            var date = DateTime.Parse(this.data);
            var year = date.Year;
        }
    }
}
