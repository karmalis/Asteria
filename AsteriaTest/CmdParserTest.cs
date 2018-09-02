using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Asteria;

namespace AsteriaTest
{
    [TestClass]
    public class CmdParserTest
    {
        private CmdParser SpawnParser()
        {
            CmdParser cmdParser = new CmdParser();
            cmdParser.AddOption("t1", "Input Test 1", true, false);
            cmdParser.AddOption("t2", "Not required Test 2", false, false);
            cmdParser.AddOption("t3", "Required Input Test 3", true, true);

            return cmdParser;
        }

        [TestMethod]
        public void TestHelp()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                CmdParser cmdParser = this.SpawnParser();
                cmdParser.PrintHelp();

                string template = "{0}Command line parameters:{1}{2}" +
                    "\t /help - Displays the help screen{3}" +
                    "\t /t1=\"\" - Input Test 1{4}" +
                    "\t /t2 - Not required Test 2{5}" +
                    "\t /t3=\"\" - Required Input Test 3{6}{7}";
                string expected = string.Format(template,
                    Environment.NewLine,
                    Environment.NewLine,
                    Environment.NewLine,
                    Environment.NewLine,
                    Environment.NewLine,
                    Environment.NewLine,
                    Environment.NewLine,
                    Environment.NewLine);

                Assert.AreEqual<string>(expected, sw.ToString(), "Inputs have to be accepted as provided and the help screen should provide proper description");
            }
        }

        [TestMethod]
        public void TestMissingCommandLineArguments()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                CmdParser cmdParser = this.SpawnParser();

                string[] args = new string[]
                {
                    "/t1=\"in 1\"", "/t2",
                };

                bool result = cmdParser.ParseArgs(args);
                Assert.IsFalse(result, "Missing argument should return false on ParseArgs");

                cmdParser.PrintErrors();
                string expected = string.Format(" Missing command line argument: t3{0}", Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString(), "Missing arguments should provide accurate message");
            }
        }

        [TestMethod]
        public void TestInvalidCommandLineArguments()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                CmdParser cmdParser = this.SpawnParser();

                string[] args = new string[]
                {
                    "t1=\"in 1\"", "/t2", "/t3=\"in 3\""
                };

                bool result = cmdParser.ParseArgs(args);
                Assert.IsFalse(result, "Invalid argument should return false on ParseArgs");

                cmdParser.PrintErrors();
                string expected = string.Format(" Invalid command line argument: t1=\"in 1\"{0}", Environment.NewLine);
                Assert.AreEqual<string>(expected, sw.ToString(), " Invalid command line argument: t1=\"in 1\"");
            }
        }

        [TestMethod]
        public void TestValidCommandLineArguments()
        {
            CmdParser cmdParser = this.SpawnParser();

            string[] args = new string[]
            {
                "/t1=\"in 1\"", "/t2", "/t3=input3"
            };

            bool result = cmdParser.ParseArgs(args);
            Assert.IsTrue(result, "Valid argument parsing should return true for ParseArgs");

            Assert.AreEqual<string>("\"in 1\"", cmdParser.GetArg("t1"), "Correct argument should match by key and be return as quoted if entered as such");
            Assert.AreEqual<string>("", cmdParser.GetArg("t2"), "Argument value without input should simply be an empty string");
            Assert.AreEqual<string>("input3", cmdParser.GetArg("t3"), "Correct argument should match by key and be returned without quotes if none were provided");
        }

        [TestMethod]
        [ExpectedException(typeof(MissingArgumentException))]
        public void TestInvalidKey()
        {
            CmdParser cmdParser = this.SpawnParser();

            string[] args = new string[]
            {
                "/t1=\"in 1\"", "/t2", "/t3=input3"
            };

            bool result = cmdParser.ParseArgs(args);
            Assert.IsTrue(result, "Valid argument parsing should return true for ParseArgs");

            string falseValue = cmdParser.GetArg("i_do_not_exist");            
        }
    }
}
