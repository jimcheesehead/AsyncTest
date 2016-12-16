using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using FileShortcutHelper;

using System.Windows.Forms; // Only for MessageBox

namespace AsyncTest
{
    static class DirOps
    {
        public class DirInfo
        {
            public int totalFiles { get; set; }
            public long totalBytes { get; set; }
            public int totalDirs { get; set; }
            public List<string> badLinks = new List<string>();

        }

        private static void InitializeInfo(DirInfo info)
        {
            info.totalDirs = 1; // Because we count the base directory
            info.totalFiles = 0;
            info.totalBytes = 0;
        }

        public static DirInfo GetDirInfo(string path)
        {
            DirInfo info = new DirInfo();
            InitializeInfo(info);
            info = GetAllDirInfo(path, info);
            return info;
        }

        private static DirInfo GetAllDirInfo(string srcPath, DirInfo info)
        {
            DirInfo inf = info;

            // Get the subdirectories for the specified directory.
            var directories = new List<string>(Directory.GetDirectories(srcPath));
            inf.totalDirs += directories.Count();

            var files = Directory.EnumerateFiles(srcPath);
            //inf.totalFiles += files.Count();

            foreach (var file in files)
            {
                FileInfo currentFile = new FileInfo(file);
                if (currentFile.Extension == @".lnk")
                {
                    // Change the current file info to the linked target file
                    currentFile = new FileInfo(ShortcutHelper.ResolveShortcut(file));

                    // Check to see if file is a directory
                    if (currentFile.Extension == String.Empty)
                    {
                        MessageBox.Show(String.Format("File {0} is a linked directory", file));
                        if (true)
                        {
                            string path = ShortcutHelper.ResolveShortcut(file);
                            directories.Add(path);
                            continue;
                        }
                    }

                    if (!currentFile.Exists)
                    {
                        // This file had a bad or missing target link
                        // MessageBox.Show(String.Format("File {0} does not exist", currentFile.FullName));
                        info.badLinks.Add(file);
                        info.totalFiles++; // Count it anyway
                        continue;
                    }
                }

                info.totalFiles++;
                info.totalBytes += (long)currentFile.Length;

            }

            // Now do the subdirectories
            foreach (string path in directories)
            {
                //inf.totalFiles = countAllFiles(path, inf.totalFiles);
                inf = GetAllDirInfo(path, inf);
            }

            return inf;
        }

        public static async Task<DirInfo> AsyncDirectoryCopy(string srcPath, string dstPath, Action<DirInfo> progressCallback)
        {
            DirInfo info = new DirInfo();
            InitializeInfo(info);
            info = await asyncDirectoryCopy(srcPath, dstPath, info, progressCallback);
            return info;
        }

        private static async Task<DirInfo> asyncDirectoryCopy(string srcDir, string dstDir, DirInfo info, Action<DirInfo> progressCallback)
        {
            DirInfo inf = info;

            var dirs = Directory.EnumerateDirectories(srcDir);
            var files = Directory.EnumerateFiles(srcDir);

            // Does not process linked subdirectories yet!

            foreach (string filename in files)
            {

                string srcFile = filename;
                string dstFile;

                if (ShortcutHelper.IsShortcut(filename))
                {
                    srcFile = ShortcutHelper.ResolveShortcut(filename);
                }

                if (!File.Exists(srcFile))
                {
                    // This file had a bad or missing target link
                    //MessageBox.Show(String.Format("File {0} does not exist", srcFile));
                    info.badLinks.Add(filename);
                    info.totalFiles++; // Count it anyway
                    progressCallback(inf);

                    continue;
                }
                dstFile = dstDir + srcFile.Substring(srcFile.LastIndexOf('\\'));

                // Check to see if destination file exists.
                // If it does skip it unless overwrite option is true.
                if (!File.Exists(dstFile))
                {
                    using (FileStream SourceStream = File.Open(srcFile, FileMode.Open))
                    {
                        using (FileStream DestinationStream = File.Create(dstDir + srcFile.Substring(srcFile.LastIndexOf('\\'))))
                        {
                            await SourceStream.CopyToAsync(DestinationStream);
                        }
                    }
                }

                info.totalFiles++;
                progressCallback(inf);
            }

            // Now copy the subdirectories recursively
            foreach (string path in dirs)
            {
                string dirName = path.Substring(path.LastIndexOf('\\'));
                string fullDirName = dstDir + dirName;
                // string tmpPath = Path.Combine(dstPath, path.Substring(path.LastIndexOf('\\')));


                // If the subdirectory doesn't exist, create it.
                if (!Directory.Exists(fullDirName))
                {
                    Directory.CreateDirectory(fullDirName);
                }

                await asyncDirectoryCopy(path, fullDirName, inf, progressCallback);
            }

            return inf;
        }



        /***********************************************************
         * NO LONGER USED
         **********************************************************/

        private static int countAllFiles(string srcPath, int count)
        {
            int totalFiles = count;

            var dirs = Directory.EnumerateDirectories(srcPath);
            //totalDirs += dirs.Count();

            var files = Directory.EnumerateFiles(srcPath);
            totalFiles += files.Count();

            // Now do the subdirectories
            foreach (string path in dirs)
            {
                totalFiles = countAllFiles(path, totalFiles);
            }

            return totalFiles;

        }

        private static DirInfo GetDirInfo(string srcPath, DirInfo info)
        {
            DirInfo inf = info;

            var dirs = Directory.EnumerateDirectories(srcPath);
            inf.totalDirs += dirs.Count();

            var files = Directory.EnumerateFiles(srcPath);
            inf.totalFiles += files.Count();

            // Now do the subdirectories
            foreach (string path in dirs)
            {
                inf.totalFiles = countAllFiles(path, inf.totalFiles);
            }

            return inf;
        }

    }
}
