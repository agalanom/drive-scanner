using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DriveScanner
{
    public static class FileSystemUtils
    {
        public static List<(string name, string label)> GetDiskDrives()
        {
            try
            {
                return DriveInfo.GetDrives()
                    .Where(d => d.DriveType == DriveType.Fixed)
                    .Select(d => (d.Name, d.VolumeLabel))
                    .ToList();
            }
            catch (IOException)
            {
                Console.WriteLine("I/O error occured.");
                return new List<(string name, string label)>();
            }
            catch (System.Security.SecurityException)
            {
                Console.WriteLine("You do not have the required permission.");
                return new List<(string name, string label)>();
            }
        }

        public static List<(string path, long size)> GetFolderInfo(string path)
        {
            var info = new List<(string, long)>();
            info.Add((path, GetSizeOfFilesInFolder(path)));

            var directories = GetDirectories(path);

            foreach (var dir in directories)
            {
                info.AddRange(GetFolderInfo(dir));
            }
            return info;
        }

        private static List<string> GetDirectories(string path)
        {
            try
            {
                return !IsSymbolicDirectory(path)
                    ? Directory.EnumerateDirectories(path).ToList()
                    : new List<string>();
            }
            catch (UnauthorizedAccessException)
            {
                return new List<string>();
            }
            catch (PathTooLongException)
            {
                return new List<string>();
            }
            catch (DirectoryNotFoundException)
            {
                return new List<string>();
            }
        }

        private static long GetSizeOfFilesInFolder(string path) =>
            GetFiles(path).Select(GetFileSize).Aggregate((long)0, (size, acc) => acc += size);

        private static List<string> GetFiles(string path)
        {
            try
            {
                return !IsSymbolicFile(path)
                    ? Directory.EnumerateFiles(path).ToList()
                    : new List<string>();
            }
            catch (UnauthorizedAccessException)
            {
                return new List<string>();
            }
            catch (PathTooLongException)
            {
                return new List<string>();
            }
            catch (DirectoryNotFoundException)
            {
                return new List<string>();
            }
        }

        private static long GetFileSize(string path)
        {
            try
            {
                return new FileInfo(path).Length;
            }
            catch (UnauthorizedAccessException)
            {
                return 0;
            }
            catch (PathTooLongException)
            {
                return 0;
            }
        }

        // this might give false positives if it encounters a real file with reparse points
        // apparently it's really difficult to tell symlinks apart on Windows...
        private static bool IsSymbolicFile(string path)
        {
            var fireInfo = new FileInfo(path);
            return fireInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }

        private static bool IsSymbolicDirectory(string path)
        {
            var pathInfo = new DirectoryInfo(path);
            return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }
    }
}
