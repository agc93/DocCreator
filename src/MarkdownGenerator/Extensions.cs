using System;
using System.IO.Abstractions;

namespace MarkdownGenerator
{
    public static class Extensions
    {
        public static void CopyDirectory(this DirectoryInfoBase source, DirectoryInfoBase target, bool verbose = false)
        {
            Copy(source.FullName, target.FullName, verbose);
        }

        private static void Copy(string sourceDirectory, string targetDirectory, bool verbose = false, IFileSystem fs = null)
        {
            fs = fs ?? new FileSystem();
            DirectoryInfoBase diSource = fs.DirectoryInfo.FromDirectoryName(sourceDirectory);
            DirectoryInfoBase diTarget = fs.DirectoryInfo.FromDirectoryName(targetDirectory);
            CopyAll(diSource, diTarget, verbose);
        }

        private static void CopyAll(DirectoryInfoBase source, DirectoryInfoBase target, bool verbose = true, IFileSystem fs = null)
        {
            fs = fs ?? new FileSystem();
            fs.Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfoBase fi in source.GetFiles())
            {
                if (verbose) Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(fs.Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfoBase diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfoBase nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
