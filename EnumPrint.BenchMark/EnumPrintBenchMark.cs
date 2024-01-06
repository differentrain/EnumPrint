using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace EnumPrint.BenchMark
{
    [SimpleJob(RuntimeMoniker.Net461)]
    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class EnumPrintBenchMark
    {
        private static readonly Random s_rnd = new();
        private readonly DayOfWeek[] _test01;
        private readonly FileAttributes[] _test02;

        public EnumPrintBenchMark()
        {
            _test01 = new DayOfWeek[10];
            _test02 = new FileAttributes[10];
            for (int i = 0; i < 10; i++)
            {
                _test01[i] = (DayOfWeek)s_rnd.Next(0, 8);
                _test02[i] = (FileAttributes)s_rnd.Next(0, 0x40001);
            }
            _test01[0].Print();
            _test02[0].Print();
            _test01[0].ToString();
            _test02[0].ToString();
        }

        [Benchmark]
        public void TestPrintG()
        {
            for (int i = 0; i < 10; i++)
            {
                _test01[i].Print('G');
                _test02[i].Print('G');
            }
        }

        [Benchmark]
        public void TestToStringG()
        {
            for (int i = 0; i < 10; i++)
            {
                _test01[i].ToString("G");
                _test02[i].ToString("G");
            }
        }

        [Benchmark]
        public void TestPrintF()
        {
            for (int i = 0; i < 10; i++)
            {
                _test01[i].Print('F');
                _test02[i].Print('F');
            }
        }

        [Benchmark]
        public void TestToStringF()
        {
            for (int i = 0; i < 10; i++)
            {
                _test01[i].ToString("F");
                _test02[i].ToString("F");
            }
        }


        [Benchmark]
        public void TestPrintD()
        {
            for (int i = 0; i < 10; i++)
            {
                _test01[i].Print('D');
                _test02[i].Print('D');
            }
        }

        [Benchmark]
        public void TestToStringD()
        {
            for (int i = 0; i < 10; i++)
            {
                _test01[i].ToString("D");
                _test02[i].ToString("D");
            }
        }


        [Benchmark]
        public void TestPrintX()
        {
            for (int i = 0; i < 10; i++)
            {
                _test01[i].Print('X');
                _test02[i].Print('X');
            }
        }

        [Benchmark]
        public void TestToStringX()
        {
            for (int i = 0; i < 10; i++)
            {
                _test01[i].ToString("X");
                _test02[i].ToString("X");
            }
        }
    }
}
