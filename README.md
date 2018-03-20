# Drive Scanner

A simple console application in C# that exports to a text file all folders and files on the machine it is running on and displays the total combined size of the files contained within each folder. The text file output is ordered by folder size with the largest at the top.

The application asks for a drive letter to be selected or scans all drives otherwise and outputs the results in a text file called `scan-results` followed by the current timestamp.

It's written using .Net Core 2.0 and should work on unix-based systems though it has only been tested on Windows. Publish profiles with VS for Windows, Mac and Ubuntu self-contained executables are included.

## Build and Run

To build, change to the `DriveScanner` directory and type the following two commands:

```bash
dotnet restore
dotnet build
```

After that, type this command to run the output executable:

```bash
dotnet run
```

## Publish
To publish the self-contained executable itself can be used instead which can be found under the output folder `bin\Release\PublishOutput` and the respective platform folder which would be `osx`, `ubuntu` or `win`.
