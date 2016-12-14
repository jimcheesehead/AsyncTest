using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;


namespace AsyncTest
{
    static class DirOps
    {
        public class DirInfo
        {
            public int totalFiles { get; set; }
            public int totalBytes { get; set; }
            public int totalDirs { get; set; }
        }

        private static void InitializeInfo(DirInfo info)
        {
            info.totalDirs = 0;
            info.totalFiles = 0;
            info.totalBytes = 0;
        }

        public static DirInfo GetDirInfo(string path)
        {
            DirInfo info = new DirInfo();
            InitializeInfo(info);

            //info.totalFiles = 0;
            //info.totalBytes = 0;
            info = GetDirInfo(path, info);


            // info.totalFiles = countAllFiles(path, 0);

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

    }
}
