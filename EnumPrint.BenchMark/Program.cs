using BenchmarkDotNet.Running;
using EnumPrint.BenchMark;

_ = BenchmarkRunner.Run<EnumPrintBenchMark>();
Console.ReadLine();
