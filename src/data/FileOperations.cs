using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileCompareProject.src.data
{
    public class FileOperations
    {
        private Config _config;

        public FileOperations(Config config)
        {
            _config = config;
            Logger.Initialize(config.LogFile);

            // Ensure the destination directory exists
            var destDir = Path.GetFullPath(config.DestinationFolder);
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
        }

        public event Action FileOpsStart;
        public event Action FileOpsDone;

        public void ExecuteFileOperations(bool moveFiles)
        {
            FileOpsStart?.Invoke();
            Logger.WriteLog("Starting file operations...");

            var source1Files = Directory.GetFiles(_config.SourceFolder1, "*", SearchOption.AllDirectories);
            var source2Files = Directory.GetFiles(_config.SourceFolder2, "*", SearchOption.AllDirectories);

            var processedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            ProcessFiles(source1Files, source2Files, moveFiles, processedFiles, _config.SourceFolder1);
            ProcessFiles(source2Files, source1Files, moveFiles, processedFiles, _config.SourceFolder2);

            Logger.WriteLog("File operations completed.");
            FileOpsDone?.Invoke();
        }

        private void ProcessFiles(string[] sourceFiles, string[] otherSourceFiles, bool moveFiles, HashSet<string> processedFiles, string sourceFolder)
        {
            foreach (var file in sourceFiles)
            {
                var relativePath = GetRelativePath(file, sourceFolder);
                if (processedFiles.Contains(relativePath)) continue;

                var destPath = Path.Combine(_config.DestinationFolder, relativePath);
                var destDirectory = Path.GetDirectoryName(destPath);

                if (!Directory.Exists(destDirectory))
                {
                    Directory.CreateDirectory(destDirectory);
                }

                var correspondingFile = otherSourceFiles.FirstOrDefault(f => GetRelativePath(f, sourceFolder) == relativePath);

                if (correspondingFile != null)
                {
                    // If both files exist, move/copy the smaller one
                    var sourceFileInfo = new FileInfo(file);
                    var correspondingFileInfo = new FileInfo(correspondingFile);

                    var fileToMoveOrCopy = sourceFileInfo.Length < correspondingFileInfo.Length ? file : correspondingFile;
                    if (moveFiles)
                    {
                        File.Move(fileToMoveOrCopy, destPath);
                    }
                    else
                    {
                        File.Copy(fileToMoveOrCopy, destPath);
                    }
                }
                else
                {
                    // If the file only exists in the source folder, move/copy it
                    if (moveFiles)
                    {
                        File.Move(file, destPath);
                    }
                    else
                    {
                        File.Copy(file, destPath);
                    }
                }

                processedFiles.Add(relativePath);
            }
        }

        private string GetRelativePath(string filePath, string folder)
        {
            return filePath.Substring(folder.Length + 1);
        }
    }
}
