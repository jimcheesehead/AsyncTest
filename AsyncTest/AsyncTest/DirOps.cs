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
        [Flags]
        public enum Options
        {
            None = 0x00,
            DereferenceLinks = 0x01,
            OverWriteFiles = 0x02,
            TopDirectoryOnly = 0x04
        }

        public class DirInfo
        {
            public int totalFiles { get; set; }
            public long totalBytes { get; set; }
            public int totalDirs { get; set; }
            public List<string> badLinks = new List<string>();
            public Options dirOpts;
        }

        private static void InitializeInfo(DirInfo info, Options options)
        {
            info.totalDirs = 0; // Because we count the base directory
            info.totalFiles = 0;
            info.totalBytes = 0;
            info.dirOpts = options;
        }

        public static DirInfo GetDirInfo(string path, Options options = Options.None)
        {
            DirInfo info = new DirInfo();
            InitializeInfo(info, options);
            info = GetAllDirInfo(path, info);
            return info;
        }

        private static DirInfo GetAllDirInfo(string srcPath, DirInfo info)
        {
            DirInfo inf = info;

            // Get the subdirectories for the specified directory.
            var directories = new List<string>(Directory.GetDirectories(srcPath));
            //inf.totalDirs += directories.Count();

            var files = Directory.EnumerateFiles(srcPath);

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
                        //info.totalFiles++; // Count it anyway
                        continue;
                    }
                }

                info.totalFiles++;
                info.totalBytes += (long)currentFile.Length;

            }

            // Now do the subdirectories
            if (!info.dirOpts.HasFlag(Options.TopDirectoryOnly))
            {
                inf.totalDirs += directories.Count();
                foreach (string path in directories)
                {
                    //inf.totalFiles = countAllFiles(path, inf.totalFiles);
                    inf = GetAllDirInfo(path, inf);
                }
            }

            return inf;
        }

        public static async Task<DirInfo> AsyncDirectoryCopy(string srcPath, string dstPath,
            Action<DirInfo> progressCallback, Options options = Options.None)
        {
            DirInfo info = new DirInfo();
            InitializeInfo(info, options);
            info = await asyncDirectoryCopy(srcPath, dstPath, info, progressCallback);
            return info;
        }

        private static async Task<DirInfo> asyncDirectoryCopy(string srcDir, string dstDir, DirInfo info, 
            Action<DirInfo> progressCallback)
        {
            DirInfo inf = info;

            // Get the subdirectories for the specified directory.
            var directories = new List<string>(Directory.GetDirectories(srcDir));
            // inf.totalDirs += directories.Count();

            //var dirs = Directory.EnumerateDirectories(srcDir);
            var files = Directory.EnumerateFiles(srcDir);
            int n = files.Count();

            foreach (string filename in files)
            {

                string srcFile = filename;
                string dstFile;

                
                if (ShortcutHelper.IsShortcut(filename))
                {
                    srcFile = ShortcutHelper.ResolveShortcut(filename);
                    string extension = Path.GetExtension(srcFile);

                    if (extension == String.Empty)
                    {
                        MessageBox.Show(String.Format("File {0} is a linked directory", srcFile));
                        //string path = ShortcutHelper.ResolveShortcut(file);
                        directories.Add(srcFile);
                        continue;
                    }
                }

                if (!File.Exists(srcFile))
                {
                    // This file had a bad or missing target link
                    // MessageBox.Show(String.Format("File {0} has a bad link", filename));
                    info.badLinks.Add(filename);
                }
                else
                {
                    dstFile = dstDir + srcFile.Substring(srcFile.LastIndexOf('\\'));

                    // Check to see if destination file exists.
                    // If it does skip it unless overwrite option is true.
                    if (!File.Exists(dstFile) || info.dirOpts.HasFlag(Options.OverWriteFiles))
                    {
                        using (FileStream SourceStream = File.Open(srcFile, FileMode.Open))
                        {
                            using (FileStream DestinationStream = File.Create(dstDir + srcFile.Substring(srcFile.LastIndexOf('\\'))))
                            {
                                await SourceStream.CopyToAsync(DestinationStream);
                            }
                        }

                        FileInfo f = new FileInfo(dstFile);
                        info.totalBytes += f.Length;
                        info.totalFiles++;
                    }

                    //FileInfo f = new FileInfo(dstFile);
                    //info.totalBytes += f.Length;
                    //info.totalFiles++;
                }

                //info.totalFiles++;
                progressCallback(inf);
            }

            // Now copy the subdirectories recursively
            if (!info.dirOpts.HasFlag(Options.TopDirectoryOnly))
            {
                inf.totalDirs += directories.Count();
                foreach (string path in directories)
                {
                    string dirName = path.Substring(path.LastIndexOf('\\'));
                    string fullDirName = dstDir + dirName;

                    // If the subdirectory doesn't exist, create it.
                    if (!Directory.Exists(fullDirName))
                    {
                        Directory.CreateDirectory(fullDirName);
                    }

                    inf = await asyncDirectoryCopy(path, fullDirName, inf, progressCallback);
                }
            }

            return inf;
        }

        public static int CountFiles(string path)
        {
            int fCount = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
            return fCount;
        }

        public static DirInfo CountDirs(string path)
        {
            DirInfo info = new DirInfo();
            info.totalDirs = System.IO.Directory.GetDirectories(path, "*", SearchOption.AllDirectories).Length;
            info.totalFiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length;
            return info;
        }

    }
}
