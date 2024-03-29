Latest release (compiled and ready to use):
https://github.com/SaintPaddy/LogRhythmConfigBackup/releases/tag/v1.0.0

<br /><br /><br />

# LogRhythm Config Backup

The 'LogRhythm Config Backup' is a Windows service that performs daily backups of the LogRhythm System Monitor configuration files.

# Overview

This application is designed to provide a simple and efficient solution for backing up LogRhythm System Monitor INI files without the need to involve additional support teams. It is compatible with any Windows version 2012 or above, and supports both x86 and x64 processor architectures.

The installation size of the application is only 4 MB, and each backup requires only about 200 KB of disk space. While running, the application takes up around 11 MB of memory.

To use this application, you'll need to have the .NET Framework 4.7.2 installed. This dependency is also required for the LogRhythm System Monitor, so it should already be installed on your system.

## Installation

To install the service, simply download the latest release from the releases page. The installation process is straightforward and should only take a few seconds to complete. Note that the installation folder is fixed and cannot be changed. 
The application will be installed in the following directory: `%ProgramFiles(x86)%\LogRhythm_Config_Backup\`

There are two ways to install the MSI:
* Double-click the MSI file and follow the prompts.
* Use an elevated command prompt to install the MSI using the following command: `msiexec /i LogRhythm_Config_Backup_v1.0.msi`

After installation, the service will be automatically started and configured to run daily backups of the LogRhythm System Monitor INI files.

## Configuration

The service can be configured by editing the config.ini file that is located in the same folder as the executable. 
The following settings can be customized:
<br />`BackupTime`: The time of day when the backup should be performed. Format: HH:mm. Default: 20:00.
<br />`BackupPath`: The folder where the backups should be stored. Default: C:\LogRhythm_config_backups.
<br />`MaxBackups`: The number of backups to keep. Default: 30.
<br />Make sure to save the config.ini file after making any changes. You have to restart the Windows Service for them to take effect.
<br />
<br />This service will log its actions in the 'Application' Windows Event Log for easy tracking and troubleshooting purposes.

## Support
If you encounter any issues while using this application, please don't hesitate to reach out for support. You can submit a support request through the GitHub Issues page, and I'll do my best to help you as quickly as possible.
<br />
<br />
<br />
<br />
# Sidenote
## Purpose

The LogRhythm System Monitor is an important agent that we use to collect logs from our customers. However, during agent upgrades, we have noticed that the settings in the INI files can disappear, causing us to lose important Office365 connections. Unfortunately, customers often do not perform backups of these INI files, which is why I created this small program. The LogRhythm Backup Service automatically performs daily backups of the INI files located on the same server as the LogRhythm System Monitor, ensuring that important settings are never lost. 
**Why are the backups on the same server?** So we don't have to contact other teams to assist us in restoring our INI files. 

## Source Code

If you're interested in viewing the source code for this application, it's available in this repository. Please note that the source code is provided for informational purposes only and is not intended to be used as-is. In order to use the application, you'll need to compile the source code in Visual Studio and (in you require an installer) wrap it in an MSI installer.

To ensure a smooth installation process, it's recommended to use the MSI installer provided on the Releases page rather than attempting to compile the source code yourself. The Releases page contains only the compiled MSI installer and does not include any source code.

## Personal note
As someone who has previously focused on developing small PowerShell scripts that run as Windows Services and create their own wrappers, this project marks my first venture into C# programming. Despite consulting ChatGPT for guidance at times, the development process still required an entire weekend to complete to my satisfaction.

To create the Windows Service, the included ZIP file contains NSSM (https://nssm.cc/), as it proved essential in enabling the service to function correctly.

My aim in creating this tool was to provide a simple yet effective solution for backing up LogRhythm System Monitor INI files, thus sparing us the need to involve additional support teams to restore backups. I hope that this tool proves beneficial to anyone encountering similar challenges.
