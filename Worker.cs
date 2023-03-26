using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.ServiceProcess;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;


public class LogRhythmBackupService : BackgroundService
{
    private string IniFileName;
    private readonly ILogger<LogRhythmBackupService> _logger;

    public LogRhythmBackupService(ILogger<LogRhythmBackupService> logger)
    {
        _logger = logger;
        string serviceDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        IniFileName = Path.Combine(serviceDirectory, "config.ini");
        InitializeIniFile();
    }









    private const string SourceName = "LogRhythmBackupService_Log";
    // Import the GetPrivateProfileString function from the Windows API
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder result, int size, string filePath);


    protected void OnStart(string[] args)
    {
        // Create EventLog source if not exists
        if (!EventLog.SourceExists(SourceName))
        {
            EventLog.CreateEventSource(SourceName, "Application");
        }

        // Initialize INI file
        string serviceDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        IniFileName = Path.Combine(serviceDirectory, "config.ini");
        InitializeIniFile();

        // Read values from INI file
        var BackupTime = ReadIniValue("Settings", "BackupTime", "20:00");
        var BackupPath = ReadIniValue("Settings", "BackupPath", @"C:\LogRhythm_config_backups");
        var MaxBackups = int.Parse(ReadIniValue("Settings", "MaxBackups", "30"));

        // Schedule backup
        ScheduleBackup(BackupTime, BackupPath, MaxBackups);

        EventLog.WriteEntry(SourceName, "LogRhythmBackupService started", EventLogEntryType.Information);
    }
    private void InitializeIniFile()
    {
        if (!File.Exists(IniFileName))
        {
            using (StreamWriter writer = new StreamWriter(IniFileName))
            {
                writer.WriteLine("[Settings]");
                writer.WriteLine("; Backup time in 24-hour format (HH:mm). Default value: 20:00");
                writer.WriteLine("BackupTime=20:00");
                writer.WriteLine();
                writer.WriteLine("; Backup storage folder path. Default value: C:\\LogRhythm_config_backups\\");
                writer.WriteLine("BackupPath=C:\\LogRhythm_config_backups\\");
                writer.WriteLine();
                writer.WriteLine("; Maximum number of backups to keep. Default value: 30");
                writer.WriteLine("MaxBackups=30");
            }
        }
    }



    private string ReadIniValue(string section, string key, string defaultValue)
    {
        StringBuilder temp = new StringBuilder(255);
        int bytesRead = GetPrivateProfileString(section, key, defaultValue, temp, 255, IniFileName);

        string value = temp.ToString();

        // Remove comments starting with a semicolon
        int commentIndex = value.IndexOf(';');
        if (commentIndex >= 0)
        {
            value = value.Substring(0, commentIndex).Trim();
        }

        // Check for malicious content as before
        if (IsMalicious(value))
        {
            EventLog.WriteEntry(SourceName, $"Malicious value detected in the INI file for key '{key}' in section '{section}'. Using the default value.", EventLogEntryType.Warning);
            return defaultValue;
        }

        return value;
    }

    private bool IsMalicious(string value)
    {
        // Build checks if wished


        // If none of the checks above return true, the value is considered not malicious
        return false;
    }





    private Timer _BackupTimer;

    private void ScheduleBackup(string BackupTime, string BackupPath, int MaxBackups)
    {
        if (!TimeSpan.TryParse(BackupTime, out var scheduledTime))
        {
            EventLog.WriteEntry(SourceName, $"Invalid backup time format: {BackupTime}. Using default value of 20:00.", EventLogEntryType.Warning);
            scheduledTime = TimeSpan.Parse("20:00");
        }

        var currentTime = DateTime.Now.TimeOfDay;
        var timeUntilNextBackup = scheduledTime - currentTime;

        if (timeUntilNextBackup <= TimeSpan.Zero)
        {
            timeUntilNextBackup += TimeSpan.FromDays(1);
        }

        _BackupTimer = new Timer(BackupCallback, Tuple.Create(BackupPath, MaxBackups), timeUntilNextBackup, TimeSpan.FromDays(1));
    }

    private void BackupCallback(object state)
    {
        var parameters = (Tuple<string, int>)state;
        var BackupPath = parameters.Item1;
        var MaxBackups = parameters.Item2;

        PerformBackup(BackupPath, MaxBackups);
    }


    private void PerformBackup(string BackupPath, int MaxBackups)
    {
        try
        {
            var configFolderPath = GetConfigFolderPath();

            if (configFolderPath == null)
            {
                EventLog.WriteEntry(SourceName, "LogRhythm System Monitor installation folder not found.", EventLogEntryType.Error);
                return;
            }

            var configBackupPath = Path.Combine(configFolderPath, "config");
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"LogRhythmConfig_{timestamp}.zip";
            var backupFilePath = Path.Combine(BackupPath, backupFileName);

            if (!Directory.Exists(BackupPath))
            {
                Directory.CreateDirectory(BackupPath);
            }

            ZipFile.CreateFromDirectory(configBackupPath, backupFilePath);

            EventLog.WriteEntry(SourceName, $"Backup successful: {backupFilePath}", EventLogEntryType.Information);

            RemoveOldBackups(BackupPath, MaxBackups);
        }
        catch (Exception ex)
        {
            EventLog.WriteEntry(SourceName, $"Backup failed: {ex.Message}", EventLogEntryType.Error);
        }
    }



    private string GetConfigFolderPath()
    {
        const string registryKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\LogRhythm\scsm";
        const string configPathValueName = "CONFIGPATH";
        const string defaultFolderPath = @"C:\Program Files\LogRhythm\LogRhythm System Monitor";

        try
        {
            // Retrieve the installation folder path from the registry
            var configPath = Registry.GetValue(registryKeyPath, configPathValueName, null) as string;

            // If the path is found in the registry and the directory exists, return it
            if (!string.IsNullOrEmpty(configPath) && Directory.Exists(configPath))
            {
                return configPath;
            }
        }
        catch (Exception ex)
        {
            EventLog.WriteEntry(SourceName, $"Error accessing the registry: {ex.Message}", EventLogEntryType.Error);
        }

        // If the path is not found in the registry or the directory does not exist, check the default folder
        if (Directory.Exists(defaultFolderPath))
        {
            return defaultFolderPath;
        }

        // If neither the registry nor the default folder contain the installation folder, return null
        return null;
    }



    private void RemoveOldBackups(string BackupPath, int MaxBackups)
    {
        try
        {
            var backupFiles = Directory.GetFiles(BackupPath, "LogRhythmConfig_*.zip")
                                        .Select(file => new FileInfo(file))
                                        .OrderByDescending(fileInfo => fileInfo.CreationTime)
                                        .ToList();

            if (backupFiles.Count > MaxBackups)
            {
                EventLog.WriteEntry(SourceName, "Removing old backups", EventLogEntryType.Information);
                EventLog.WriteEntry(SourceName, $"Backup path: {BackupPath}", EventLogEntryType.Information);
                EventLog.WriteEntry(SourceName, $"Max backups: {MaxBackups}", EventLogEntryType.Information);
            }

            while (backupFiles.Count > MaxBackups)
            {
                var fileToRemove = backupFiles.Last();
                File.Delete(fileToRemove.FullName);
                backupFiles.RemoveAt(backupFiles.Count - 1);

                EventLog.WriteEntry(SourceName, $"Old backup file removed: {fileToRemove.FullName}", EventLogEntryType.Information);
            }
        }
        catch (Exception ex)
        {
            EventLog.WriteEntry(SourceName, $"Error removing old backups: {ex.Message}", EventLogEntryType.Error);
        }
    }


    protected void OnStop()
    {
        EventLog.WriteEntry(SourceName, "LogRhythmBackupService stopped", EventLogEntryType.Information);
    }











    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                string serviceDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                IniFileName = Path.Combine(serviceDirectory, "config.ini");
                InitializeIniFile();

                string BackupTime = ReadIniValue("Settings", "BackupTime", "20:00");
                string BackupPath = ReadIniValue("Settings", "BackupPath", @"C:\LogRhythm_config_backups");
                int MaxBackups = int.Parse(ReadIniValue("Settings", "MaxBackups", "30"));

                ScheduleBackup(BackupTime, BackupPath, MaxBackups);

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while scheduling the backup.");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

}
