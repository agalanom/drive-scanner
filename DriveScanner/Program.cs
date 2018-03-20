using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace DriveScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Drive scanner";

            Console.WriteLine("Scanning system for disk drives...");
            var drives = FileSystemUtils.GetDiskDrives();
            foreach ((string name, string label) in drives)
            {
                Console.WriteLine($"{label} ({name.Trim(Path.DirectorySeparatorChar)})".Trim());
            }
            Console.WriteLine($"{drives.Count()} drives found.\n");
            Console.WriteLine("Please type a drive letter or press Enter to scan all drives:");

            var driveLetters = drives.Select(d => SanitizeDriveLetter(d.name)).ToList();
            string input = null;
            string validLetter = null;
            do
            {
                input = Console.ReadLine();
                validLetter = driveLetters.Find(d => d.Equals(SanitizeDriveLetter(input), StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(input) && validLetter == null)
                {
                    Console.WriteLine("Not a valid drive. Please try again.");
                }
            } while (!string.IsNullOrWhiteSpace(input) && validLetter == null);

            if (!string.IsNullOrWhiteSpace(input))
            {
                var drive = drives.Find(d => d.name.StartsWith(validLetter)).name;
                ScanMutipleDrives(drive);
            }
            else
            {
                ScanMutipleDrives(drives.Select(d => d.name).ToArray());
            }
        }

        static string SanitizeDriveLetter(string letter) =>
            // avoid using separators directly so the paths can be resolved cross-platform
            letter.Replace(Path.DirectorySeparatorChar.ToString(), "").Replace(":", "");

        public static void ScanMutipleDrives(params string[] paths)
        {
            var info = new List<(string path, long size)>();
            foreach (var path in paths)
            {
                Console.WriteLine($"Scanning drive {path.Trim(Path.DirectorySeparatorChar)}...");
                info.AddRange(FileSystemUtils.GetFolderInfo(path));
            }

            var outputFile = $"scan-results-{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";
            using (var file = new StreamWriter(outputFile))
            {
                foreach ((string p, long s) in info.OrderByDescending(i => i.size).ToList())
                {
                    file.WriteLine($"{p} - {s.ToString("N0")}");
                }
            }
            Console.WriteLine($"{info.Count()} folders found. Scan results saved in {outputFile}");
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }
    }
}
