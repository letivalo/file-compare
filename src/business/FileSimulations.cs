using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileCompareProject.src.business
{
    public class Simulation
    {
        private Config _config;
        public double EstimatedSize {  get; private set; }

        public Simulation(Config config)
        {
            _config = config;
        }

        public void RunSimulation()
        {
            Logger.Initialize(_config.LogFile); // Ensure Logger is initialized

            Logger.WriteLog("Starting simulation...");

            var source1Files = Directory.GetFiles(_config.SourceFolder1, "*", SearchOption.AllDirectories);
            var source2Files = Directory.GetFiles(_config.SourceFolder2, "*", SearchOption.AllDirectories);

            var estimatedSize = SimulateFileOperations(source1Files, source2Files);
            EstimatedSize = Math.Round(estimatedSize / (1024.0 * 1024.0), 2); // Store size in MB

            Logger.WriteLog($"Estimated final size of destination folder: {estimatedSize / (1024 * 1024)} MB");
        }

        private long SimulateFileOperations(string[] source1Files, string[] source2Files)
        {
            var fileSizes = new Dictionary<string, long>();
            var processedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var file in source1Files)
            {
                var relativePath = GetRelativePath(file, _config.SourceFolder1);
                processedFiles.Add(relativePath);
                var correspondingFile = source2Files.FirstOrDefault(f => GetRelativePath(f, _config.SourceFolder2) == relativePath);
                if (correspondingFile != null)
                {
                    // If both files exist, take the smaller one
                    fileSizes[relativePath] = Math.Min(new FileInfo(file).Length, new FileInfo(correspondingFile).Length);
                }
                else
                {
                    // If the file only exists in source1, take its size
                    fileSizes[relativePath] = new FileInfo(file).Length;
                }
            }

            foreach (var file in source2Files)
            {
                var relativePath = GetRelativePath(file, _config.SourceFolder2);
                if (!processedFiles.Contains(relativePath))
                {
                    // If the file only exists in source2, take its size
                    fileSizes[relativePath] = new FileInfo(file).Length;
                }
            }

            return fileSizes.Values.Sum();
        }

        private string GetRelativePath(string filePath, string folder)
        {
            return filePath.Substring(folder.Length + 1);
        }
    }
}