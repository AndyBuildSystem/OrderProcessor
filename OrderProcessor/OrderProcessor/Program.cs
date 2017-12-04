using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Timers;
using System.Xml;
using CommandLine;
using CommandLine.Text;
using OrderLib;

namespace OrderProcessor
{
    #region CommandLineOptionDefinitions

    public class Options
    {
        public List<string> OutputTypes = new List<string>() { "csv" };
        [Option('i', "input", Required = true,
            HelpText = "Input folder to be processed.")]
        public string InputDir { get; set; }

        [Option('o', "output", Required = true,
            HelpText = "Output folder to contain CSV orders.")]
        public string OutputDir { get; set; }

        [Option('t', "type", DefaultValue = "csv",
            HelpText = "Type of file to output e.g. csv")]
        public string OuptutType { get; set; }

        [Option('p', "interval", DefaultValue = 2000,
            HelpText = "Interval in ms in which to perfom scan")]
        public int Interval { get; set; }

        [Option('v', "verbose", DefaultValue = true,
            HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption('?', "help")]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

    #endregion

    public class Program
    {
        private static System.Timers.Timer _aTimer;
        private static readonly Options Options = new Options();
        private static readonly List<string> ProcessedFiles = new List<string>();
        public static int Main(string[] args)
        {
            //Process Options 
            if (CommandLine.Parser.Default.ParseArguments(args, Options))
            {
                if (!Options.OutputTypes.Contains(Options.OuptutType))
                {
                    Console.Write(Options.GetUsage());
                    return 1;
                }
                _aTimer = new System.Timers.Timer(Options.Interval);
                _aTimer.Elapsed += OnTimedEvent;
                _aTimer.Enabled = true;
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.WriteLine("Press the Enter key to exit the program... ");
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.ReadLine();
                if (Options.Verbose) Console.WriteLine("Terminating the application...");
                return 0;

            }
            //else will print valid options and return error code 1 if false;
            return 1;
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs eventARgs)
        {
            if(Monitor.TryEnter(ProcessedFiles, (Options.Interval))) //maintain a lock so the current file list can be fully processed before a new thread starts, could be optimised to async read files individually
            {
                System.Threading.Thread.Sleep(5000);//simulate delay
                try
                {
                    //Get list of files to process from input directory excluding ones already processed
                    var filesToProcess = Directory.GetFiles(Options.InputDir, "*.*", SearchOption.AllDirectories)
                        .ToList()
                        .Except(ProcessedFiles).ToList();
                    if (filesToProcess.Count > 0)
                    {
                        //Process Files
                        foreach (var filePath in filesToProcess)
                        {
                            switch (Path.GetExtension(filePath.ToLower()))
                            {
                                case (".json"):
                                    Console.WriteLine($@"Processing File: {filePath}");
                                    if (JsonProcessor.ProcessJsonOrderFile(filePath))
                                        ProcessedFiles.Add(filePath);
                                    continue;
                                default: //check the next file if it doesn't match our types for processing; Other file types and processors may be added later
                                    continue;
                            }
                        }

                        //Create output directory if it doesn't exist
                        if (!Directory.Exists(Options.OutputDir))
                        {
                            if (Options.Verbose) Console.WriteLine($"Creating output directory: {Options.OutputDir}");
                            try
                            {
                                Directory.CreateDirectory(Options.OutputDir);
                            }
                            catch (IOException e)
                            {
                                Console.Error.WriteLine("ERROR: Couldn't Create Output Directory");
                                Console.WriteLine(e);
                            }
                        }
                        //Write Output files if valid orders are found
                        switch (Options.OuptutType.ToLower())
                        {
                            case ("csv"):
                                OrderManager.WriteOrdersToCsv(Options.OutputDir);
                                return;
                            default: //Other output types may be added later
                                return;
                        }

                    }
                }
                finally
                {
                    Monitor.Exit(ProcessedFiles);
                }
            }
        }
    }
}
