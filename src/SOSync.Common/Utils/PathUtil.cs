using CliWrap;
using CliWrap.Buffered;
using CliWrap.EventStream;
using System.Diagnostics;
using System.Reflection;

namespace SOSync.Common.Utils;

public delegate void PathUtilLogEventHandler(object source, string logMessage);

public class PathUtil
{
    public event PathUtilLogEventHandler? OnLogInfo;
    public event PathUtilLogEventHandler? OnLogDebug;
    private static PathUtil? _instance;
    public static PathUtil Instance { get => _instance ??= new PathUtil(); }
    public static string AssemblyDirectory
    {
        get
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
    public static bool TimeBetween(DateTime datetime, TimeSpan start, TimeSpan end)
    {
        // convert datetime to a TimeSpan
        TimeSpan now = datetime.TimeOfDay;
        // see if start comes before end
        if (start < end)
            return start <= now && now <= end;
        // start is after end, so do the inverse comparison
        return !(end < now && now < start);
    }

    public static bool StartProcess(string filename, string args, string work_dir, bool WaitExit)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.CreateNoWindow = false;
        startInfo.WorkingDirectory = work_dir;
        startInfo.UseShellExecute = false;
        startInfo.FileName = filename;
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.Arguments = args;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        //Vista or higher check
        if (Environment.OSVersion.Version.Major >= 6)
        {
            startInfo.Verb = "runas";
        }
        try
        {
            // Start the process with the info we specified.
            // Call WaitForExit and then the using statement will close.
            using (Process exeProcess = Process.Start(startInfo))
            {
                if (WaitExit)
                {
                    exeProcess.WaitForExit();
                    string output = exeProcess.StandardOutput.ReadToEnd();
                    string error = exeProcess.StandardError.ReadToEnd();
                    Instance.OnLogInfo?.Invoke(Instance, $"Saida do processo : {exeProcess.Id} - {output}");
                    Instance.OnLogInfo?.Invoke(Instance, $"Erros no processo : {exeProcess.Id} - {error}");
                    Instance.OnLogDebug?.Invoke(Instance, $"Comando executado : {filename} {args} utilizando a pasta de trabalho : {work_dir}");
                }

                return true;
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Instance.OnLogInfo?.Invoke(Instance, e.Message);
            return false;
        }

    }
    public async static Task<bool> StartProcessAsync(string filename, string args, string work_dir, int WaitExit, bool buffered = true)
    {
        string logfile = Path.Combine(PathUtil.AssemblyDirectory, "manutencao.log");
        try
        {
            var cts = new CancellationTokenSource();

            // Cancel automatically after a timeout of 10 seconds
            cts.CancelAfter(TimeSpan.FromMinutes(WaitExit));
            // We're only interested in stdout
            var cmd = Cli.Wrap(filename)
                .WithArguments(args)
                .WithWorkingDirectory(work_dir)
                .WithStandardOutputPipe(PipeTarget.ToDelegate((message) => Instance.OnLogInfo?.Invoke(Instance, message)));
            Instance.OnLogDebug?.Invoke(Instance, $"Comando executado : {filename} {args} utilizando a pasta de trabalho : {work_dir}");

            if (buffered)
                await cmd.ExecuteBufferedAsync(cts.Token);
            else
            {
                await foreach (var cmdEvent in cmd.ListenAsync())
                {
                    switch (cmdEvent)
                    {
                        case StartedCommandEvent started:
                            return true;
                        case StandardOutputCommandEvent commandEvent:
                            return true;
                        case StandardErrorCommandEvent standardError:
                            throw new Exception(standardError.Text);
                        case ExitedCommandEvent exited:
                            throw new Exception("O processo foi finalizado!");

                        default:
                            throw new Exception("Erro não mapeado!");
                    }
                }
            }

            return true;
        }
        catch (Exception exc)
        {
            Instance.OnLogInfo?.Invoke(Instance, $"Erro no processo : {exc.Message}");
            return false;
        }
    }

    public static bool FileCompare(string file1, string file2)
    {
        int file1byte;
        int file2byte;
        FileStream fs1;
        FileStream fs2;

        // Determine if the same file was referenced two times.
        if (file1 == file2)
        {
            // Return true to indicate that the files are the same.
            return true;
        }

        // Open the two files.
        fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read);
        fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read);

        // Check the file sizes. If they are not the same, the files 
        // are not the same.
        if (fs1.Length != fs2.Length)
        {
            // Close the file
            fs1.Close();
            fs2.Close();

            // Return false to indicate files are different
            return false;
        }

        // Read and compare a byte from each file until either a
        // non-matching set of bytes is found or until the end of
        // file1 is reached.
        do
        {
            // Read one byte from each file.
            file1byte = fs1.ReadByte();
            file2byte = fs2.ReadByte();
        }
        while ((file1byte == file2byte) && (file1byte != -1));

        // Close the files.
        fs1.Close();
        fs2.Close();

        // Return the success of the comparison. "file1byte" is 
        // equal to "file2byte" at this point only if the files are 
        // the same.
        return ((file1byte - file2byte) == 0);
    }
}
