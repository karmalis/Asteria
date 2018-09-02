using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Asteria
{
    public class Processor
    {
        public string inputFile = "";
        public string outputFile = "";
        public int charsPerLine = 0;

        private List<string> errors = new List<string>();

        public Processor(string input, string output, string cnum)
        {
            this.inputFile = input;
            this.outputFile = output;
            bool parseResult = Int32.TryParse(cnum, out charsPerLine);

            if (!parseResult)
            {
                throw new InvalidCastException("Could not convert string to int");
            }
        }

        public Processor() : this("", "", "0") { }

        public bool Run()
        {            
            FileInfo fiIn = new FileInfo(this.inputFile);
            List<string> inputList = new List<string>();
            List<List<string>> outputList = new List<List<string>>();

            if (!fiIn.Exists) throw new FileNotFoundException("Could not find input file " + this.inputFile);

            // Read the input file
            using (StreamReader sr = fiIn.OpenText())
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    inputList.Add(line);
                }
            }

            if (inputList.Count == 0)
            {
                this.errors.Add("No valid lines in file " + this.inputFile);
                return false;
            }

            // Process the collected lines from the input file
            outputList = this.ArrangeWords(inputList);

            // Write out results to a dedicated file
            FileInfo fiOut = new FileInfo(this.outputFile);
            if (fiOut.Exists)
            {
                fiOut.Delete();
            }

            using (StreamWriter sw = fiOut.CreateText())
            {
                foreach(List<string> wordList in outputList)
                {
                    sw.WriteLine(String.Join(" ", wordList.ToArray()));
                }
            }

            return true;
        }

        private List<List<string>> ArrangeWords(List<string> input)
        {
            List<List<string>> resultList = new List<List<string>>();

            // Scroll through lines
            foreach (string line in input)
            {
                int lineLength = line.Length;
                if (lineLength > this.charsPerLine)
                {
                    string[] splitwords = line.Split(' ');

                    while (splitwords.Count() > 0)
                    {
                        List<string> adjusted = new List<string>();
                        List<string> used = new List<string>();
                        int newLength = 0;
                        int it = 0;                        

                        // Iterate through each word in a line
                        foreach (string word in splitwords)
                        {
                            // Suppose a word is longer than defined character length and 
                            // the adjusted word list is still empty. Break apart the word 
                            // and add the parts to the adjusted list. 
                            if (adjusted.Count() == 0 && word.Length > this.charsPerLine)
                            {
                                // Cursor starts at the beginning of the word
                                int curpos = 0;
                                // Cursor length depends on the word length
                                int curlen = word.Length > this.charsPerLine ? this.charsPerLine : word.Length;
                                while (curpos < word.Length)
                                {
                                    string breakword = word.Substring(curpos, curlen);

                                    adjusted.Add(breakword);
                                    curpos += curlen;

                                    // Readjust cursor length if required
                                    if ((curpos + curlen) > word.Length)
                                    {
                                        curlen = word.Length - curpos;
                                    }

                                    bool lastpart = curpos == word.Length || curlen == word.Length;
                                    // Check if the next word in the list would fit with the word ending including a space
                                    if (splitwords.Count() > 1)
                                    {
                                        string nextWord = splitwords[it+1];

                                        if (lastpart && (breakword.Length + nextWord.Length + 1) <= this.charsPerLine)
                                        {
                                            // Last part of the word and the next word fits into the line
                                            newLength += breakword.Length;
                                        } else
                                        {
                                            // Last part of the word and the next one does not fit
                                            resultList.Add(adjusted);
                                            adjusted = new List<string>();
                                        }
                                    } else
                                    {
                                        // No next word in line
                                        resultList.Add(adjusted);
                                        adjusted = new List<string>();
                                    }                                    
                                }

                                // Update iterator and add the used word to the used word list
                                it++;
                                used.Add(word);

                            } else
                            {
                                // The word being investigated fits into the limit
                                int futureLength = newLength + word.Length;
                                bool hasSpace = false;

                                // Check if we need to add a "space" character
                                if (it++ != splitwords.Length)
                                {
                                    futureLength++;
                                    hasSpace = true;
                                }

                                if (futureLength > this.charsPerLine)
                                {
                                    // Check if we added an extra space into the equation
                                    if (hasSpace && futureLength == this.charsPerLine + 1)
                                    {
                                        adjusted.Add(word);
                                        used.Add(word);
                                    }
                                    break;
                                }

                                adjusted.Add(word);
                                used.Add(word);
                                newLength = futureLength;
                            }
                        }

                        if (adjusted.Count() > 0)
                        {
                            resultList.Add(adjusted);
                        }

                        // Remove words that were used up
                        List<string> temp = splitwords.ToList();
                        temp.RemoveRange(0, used.Count());
                        splitwords = temp.ToArray();
                    }
                }
                else
                {
                    List<string> listOfWords = new List<string>();
                    listOfWords.Add(line);
                    resultList.Add(listOfWords);
                }
            }

            return resultList;
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
