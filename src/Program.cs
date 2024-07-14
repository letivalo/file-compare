using System;
using System.IO;
using Newtonsoft.Json;
using FileCompareProject.src.data;
using FileCompareProject.src.business;

namespace FileCompareProject
{
    class Program
    {
        static void Main()
        {
            try
            {
                InitializeConsole();

                string configPath = GetConfigPath();
                Config config = LoadConfig(configPath);

                ValidateFolders(config);

                var simulation = new Simulation(config);
                simulation.RunSimulation();

                DisplayConfigInfo(configPath, config, simulation.EstimatedSize);

                if (UserConfirmation("Do you want to proceed with the file operations? (Y/N)"))
                {
                    WriteLN();
                    bool moveFiles = UserSelection("Do you want to (1) Copy the smaller files or (2) Move the smaller files?") == "2";
                    var fileOps = new FileOperations(config);
                    WriteLN();

                    // Subscribe to events
                    fileOps.FileOpsStart += OnFileOpsStart;
                    fileOps.FileOpsDone += OnFileOpsDone;

                    fileOps.ExecuteFileOperations(moveFiles);

                    if (moveFiles && UserConfirmation("Do you want to delete the original folders? (Y/N)"))
                    {
                        DeleteOriginalFolders(config);
                        WriteLN();
                    }
                }
                else
                {
                    Console.WriteLine("Operation cancelled by the user.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static void InitializeConsole()
        {
            Console.Clear();
            WriteWS("File Comparison v0.0.0 by Letivalo");
        }

        static string GetConfigPath()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Path.Combine(baseDirectory, "..", "..", "..");
            projectRoot = Path.GetFullPath(projectRoot);
            return Path.Combine(projectRoot, "config.json");
        }

        static Config LoadConfig(string configPath)
        {
            if (!File.Exists(configPath))
                throw new FileNotFoundException("Configuration file not found!");

            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
            if (config == null)
                throw new InvalidDataException("Configuration file is empty or corrupted!");

            return config;
        }

        static void ValidateFolders(Config config)
        {
            if (!Directory.Exists(config.SourceFolder1) || !Directory.EnumerateFileSystemEntries(config.SourceFolder1).Any())
                throw new DirectoryNotFoundException($"Source folder 1 ({config.SourceFolder1}) does not exist or is empty.");

            if (!Directory.Exists(config.SourceFolder2) || !Directory.EnumerateFileSystemEntries(config.SourceFolder2).Any())
                throw new DirectoryNotFoundException($"Source folder 2 ({config.SourceFolder2}) does not exist or is empty.");

            if (!Directory.Exists(config.DestinationFolder))
            {
                Directory.CreateDirectory(config.DestinationFolder);
            }
        }

        static void DisplayConfigInfo(string configPath, Config config, double estimatedSize)
        {
            Console.WriteLine($"Config file path: {configPath}");
            Console.WriteLine($"Source Folder 1: {config.SourceFolder1} - Size: {GetDirectorySize(config.SourceFolder1)} MB");
            Console.WriteLine($"Source Folder 2: {config.SourceFolder2} - Size: {GetDirectorySize(config.SourceFolder2)} MB");
            WriteWS($"Destination Folder: {config.DestinationFolder}");
            Console.WriteLine($"Estimated final size of destination folder: {estimatedSize} MB");
        }

        static double GetDirectorySize(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            long size = files.Sum(file => new FileInfo(file).Length);
            return Math.Round(size / (1024.0 * 1024.0), 2); // Size in MB
        }

        static bool UserConfirmation(string message)
        {
            Console.WriteLine(message);
            var input = Console.ReadLine();
            return input != null && input.ToUpper() == "Y";
        }

        static string UserSelection(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }

        static void ExecuteFileOperations(Config config, bool moveFiles)
        {
            var fileOps = new FileOperations(config);
            fileOps.ExecuteFileOperations(moveFiles);
        }

        static void DeleteOriginalFolders(Config config)
        {
            Directory.Delete(config.SourceFolder1, true);
            Directory.Delete(config.SourceFolder2, true);
            Console.WriteLine("Original folders deleted.");
        }

        static void WriteLN()
        {
            int width = Console.WindowWidth - 1;
            string separatorLine = new string('-', width);
            Console.WriteLine(separatorLine);
        }

        static void WriteWS(string text)
        {
            Console.WriteLine(text);
            WriteLN();
        }

        // Event handlers
        static void OnFileOpsStart()
        {
            Console.WriteLine("Beginning file operations, this may take a moment...");
        }

        static void OnFileOpsDone()
        {
            Console.WriteLine("File operations completed. Program will now exit.");
        }
    }
}
