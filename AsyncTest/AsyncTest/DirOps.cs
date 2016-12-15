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

        private static DirInfo GetAllDirInfo(string srcPath, DirInfo info)
        {
            DirInfo inf = info;

            var dirs = Directory.EnumerateDirectories(srcPath);
            var directories = new List<string>(Directory.GetDirectories(srcPath));

            if (false)
            {
                inf.totalDirs += dirs.Count();

            } else
            {
                // Get the subdirectories for the specified directory.
                inf.totalDirs += directories.Count();
            }

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
                        if (false)
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
                inf.totalFiles = countAllFiles(path, inf.totalFiles);
            }

            return inf;
        }

    }
}
