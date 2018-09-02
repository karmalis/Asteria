using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Asteria;

namespace AsteriaTest
{
    [TestClass]
    public class ProcessorTest
    {
        private readonly string mockInputFile = "mock_input.txt";
        private readonly string mockOutputFile = "mock_output.txt";

        void CreateMockInputFile(string content)
        {
            FileInfo fiIn = new FileInfo(this.mockInputFile);
            if (fiIn.Exists)
            {
                fiIn.Delete();
            }

            using (StreamWriter sw = fiIn.CreateText())
            {
                sw.Write(content);
            }
        }

        string GetOutputFileContent()
        {
            FileInfo fiOut = new FileInfo(this.mockOutputFile);
            string result = "";

            if (fiOut.Exists)
            {
                using (StreamReader sr = fiOut.OpenText())
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }

        [TestCleanup]
        public void Cleanup()
        {
            FileInfo fiIn = new FileInfo(this.mockInputFile);
            FileInfo fiOut = new FileInfo(this.mockOutputFile);

            if (fiIn.Exists)
            {
                fiIn.Delete();
            }

            if (fiOut.Exists)
            {
                fiOut.Delete();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestConstructorAndCasting()
        {
            Processor processor = new Processor("inFile", "outFile", "42");

            Assert.AreEqual<string>("inFile", processor.inputFile, "Input file assigned correctly");
            Assert.AreEqual<string>("outFile", processor.outputFile, "Output file assigned correctly");
            Assert.AreEqual<int>(42, processor.charsPerLine, "Characters per line assigned and parsed correctly");

            Processor tofail = new Processor("inFile", "outFile", "This is not a number");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestVoidInputFile()
        {
            Processor processor = new Processor("inFile", "outFile", "42");
            processor.Run();
        }

        // Test cases for given task
        [TestMethod]
        public void TestExampleCaseOne()
        {
            this.CreateMockInputFile("žodis žodis žodis");
            Processor processor = new Processor(this.mockInputFile, this.mockOutputFile, "13");
            processor.Run();

            string expected = string.Format("žodis žodis{0}žodis{1}", 
                Environment.NewLine,
                Environment.NewLine
                );            
            string result = this.GetOutputFileContent();
            Assert.AreEqual<string>(expected, result, "First Example Case");
        }

        [TestMethod]
        public void TestExampleCaseTwo()
        {
            this.CreateMockInputFile("šiuolaikiškas ir mano žodis");
            Processor processor = new Processor(this.mockInputFile, this.mockOutputFile, "7");
            processor.Run();

            string expected = string.Format("šiuolai{0}kiškas{1}ir mano{2}žodis{3}", 
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine
                );
            string result = this.GetOutputFileContent();
            Assert.AreEqual<string>(expected, result, "Second Example Case");
        }

        // Other possible test cases
        [TestMethod]
        public void TestFullLineFit()
        {
            this.CreateMockInputFile("šiuolaikiškas ir mano žodis");
            Processor processor = new Processor(this.mockInputFile, this.mockOutputFile, "27");
            processor.Run();

            string expected = string.Format("šiuolaikiškas ir mano žodis{0}",
                Environment.NewLine
                );
            string result = this.GetOutputFileContent();
            Assert.AreEqual<string>(expected, result, "Full fit of text should stay as is");
        }

        [TestMethod]
        public void TestCaseWithBrokenWordAndExtraFit()
        {
            this.CreateMockInputFile("šiuolaikiškas ir mano žodis");
            Processor processor = new Processor(this.mockInputFile, this.mockOutputFile, "8");
            processor.Run();

            string expected = string.Format("šiuolaik{0}iškas ir{1}mano{2}žodis{3}",
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine
                );
            string result = this.GetOutputFileContent();
            Assert.AreEqual<string>(expected, result, "A broken up word ending can fit extra words if limit allows for it");
        }

        [TestMethod]
        public void TestCaseWithExactFit()
        {
            this.CreateMockInputFile("šiuolaikiškas ir mano žodis");
            Processor processor = new Processor(this.mockInputFile, this.mockOutputFile, "13");
            processor.Run();

            string expected = string.Format("šiuolaikiškas{0}ir mano žodis{1}",
                Environment.NewLine,
                Environment.NewLine
                );
            string result = this.GetOutputFileContent();
            Assert.AreEqual<string>(expected, result, "Exact fit for a word should not add extra spaces or line breaks");
        }

        [TestMethod]
        public void TestMultiLineFile()
        {
            string fileContent = "šiuolaikiškas ir mano žodis{0}" +
                "va dabar jau ir multi-line{1}" +
                "su extra eilutėmis";
            fileContent = string.Format(fileContent,
                Environment.NewLine,
                Environment.NewLine
                );

            this.CreateMockInputFile(fileContent);
            Processor processor = new Processor(this.mockInputFile, this.mockOutputFile, "6");
            processor.Run();

            string expected = "šiuola{0}ikiška{1}s ir{2}mano{3}žodis{4}" +
                "va{5}dabar{6}jau ir{7}multi-{8}line{9}" +
                "su{10}extra{11}eilutė{12}mis{13}";
            expected = string.Format(expected,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine,
                Environment.NewLine
                );
            
            string result = this.GetOutputFileContent();
            Assert.AreEqual<string>(expected, result, "Multiple lines in a file should work as expected");
            
        }

    }
}
