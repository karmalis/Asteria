using System;
using System.Collections.Generic;

namespace Asteria
{
    public class MissingArgumentException : System.Exception
    {
        public MissingArgumentException() : base() { }
        public MissingArgumentException(string message) : base(message) { }
        public MissingArgumentException(string message, System.Exception inner) : base(message, inner) { }
    }

    /*
     *  A simple command line parser 
     */
    public class CmdParser
    {
        private class OptionDesc
        {
            public string description;
            public bool hasInput;
            public bool required;

            public OptionDesc(string desc, bool input, bool required)
            {
                this.description = desc;
                this.hasInput = input;
                this.required = required;
            }

            public OptionDesc() : this("", false, false) { }
        }

        // Dict structure: <command, CommandDesc>
        // Commands are without hyphons
        private Dictionary<string, OptionDesc> options = new Dictionary<string, OptionDesc>();

        // Dict structure: <command, value>
        private Dictionary<string, string> parsedArgs = new Dictionary<string, string>();

        // Error List
        private List<string> errors = new List<string>();

        public CmdParser()
        {
            this.AddOption("help", "Displays the help screen");
        }

        // Add a command to the parser
        public void AddOption(string command, string description, bool input = false, bool required = false)
        {
            this.options.Add(command, new OptionDesc(description, input, required));
        }

        // Print help
        public void PrintHelp()
        {
            Console.WriteLine();
            Console.WriteLine("Command line parameters:");
            Console.WriteLine();

            foreach (KeyValuePair<string, OptionDesc> kvp in this.options)
            {
                if (!kvp.Value.hasInput)
                {
                    Console.WriteLine("\t /{0} - {1}", kvp.Key, kvp.Value.description);
                } else
                {
                    Console.WriteLine("\t /{0}=\"\" - {1}", kvp.Key, kvp.Value.description);
                }
            }

            Console.WriteLine();
        }

        public string GetArg(string key)
        {
            if (!this.parsedArgs.ContainsKey(key))
            {
                throw new MissingArgumentException("No such argument has been parsed: " + key);
            }

            return this.parsedArgs[key];
        }

        // Parse the arguments
        // Returns bool
        public bool ParseArgs(string[] args)
        {
            // Do the initial parsing
            foreach (string arg in args)
            {
                if (arg.Length > 0)
                {
                    switch (arg[0])
                    {
                        case '-':
                        case '/':
                            {
                                int endOfOption = arg.IndexOfAny(new char[] { ':', '=' }, 1);
                                string argLabel = arg.Substring(1, endOfOption == -1 ? arg.Length - 1 : endOfOption - 1);                                

                                if (!this.options.ContainsKey(argLabel))
                                {
                                    this.errors.Add(" No such command line argument: " + arg);
                                    continue;
                                }

                                if (this.options[argLabel].hasInput)
                                {
                                    if (endOfOption == -1 || (arg.Length == argLabel.Length + 1))
                                    {
                                        this.errors.Add(" Missing value for command line argument: " + argLabel);
                                        continue;
                                    }

                                    this.parsedArgs[argLabel] = arg.Substring(endOfOption + 1);

                                } else
                                {
                                    this.parsedArgs[argLabel] = "";
                                }

                            } break;
                        default:
                            {
                                this.errors.Add(" Invalid command line argument: " + arg);
                            } break;
                    }
                }            
            }

            // Check for any missing required options
            foreach (KeyValuePair<string, OptionDesc> kvp in this.options)
            {
                if (kvp.Value.required && !this.parsedArgs.ContainsKey(kvp.Key))
                {
                    this.errors.Add(" Missing command line argument: " + kvp.Key);
                }
            }

            if (this.parsedArgs.ContainsKey("help"))
            {
                this.PrintHelp();
            }

            return this.errors.Count == 0;
        }

        public List<string> getErrors()
        {
            return this.errors;
        }

        public void PrintErrors()
        {
            foreach (string error in this.errors)
            {
                Console.WriteLine(error);
            }
        }
    }
}
