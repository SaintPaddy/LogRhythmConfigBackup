# LogRhythm Config Backup

The 'LogRhythm Config Backup' is a Windows service that performs daily backups of the LogRhythm System Monitor configuration files.

## Purpose

The LogRhythm System Monitor is an important agent that we use to collect logs from our customers. However, during agent upgrades, we have noticed that the settings in the INI files can disappear, causing us to lose important Office365 connections. Unfortunately, customers often do not perform backups of these INI files, which is why we created this small program. The LogRhythm Backup Service automatically performs daily backups of the INI files located on the same server as the LogRhythm System Monitor, ensuring that important settings are never lost. 
**Why are the backups on the same server?** So we don't have to contact other teams to assist us in restoring our INI files. 
Rest assured, this application consumes very little disk space, typically only a few KB per backup and 4 MB for the installation.


## Installation

To install the service, simply download the latest release from the releases page and install the MSI. There are two ways to install the MSI:

* Double-click the MSI file and follow the prompts.
* Use the command line to install the MSI using the `msiexec` command. Please note that the command must be run as administrator:
```
msiexec /i LogRhythm_Config_Backup_v1.0.msi
```
After installation, the service will be automatically started and configured to run daily backups of the LogRhythm System Monitor INI files.

## Configuration

The service can be configured by editing the config.ini file that is located in the same folder as the executable. The following settings can be customized:

BackupTime: The time of day when the backup should be performed. Format: HH:mm. Default: 20:00.
BackupPath: The folder where the backups should be stored. Default: C:\LogRhythm_config_backups.
MaxBackups: The number of backups to keep. Default: 30.
Make sure to save the config.ini file after making any changes.
<br />
<br />
<br />
<br />This service will log its actions in the Windows Event Log for easy tracking and troubleshooting purposes.


## Sidenote

As someone who has previously focused on developing small PowerShell scripts that run as Windows Services and create their own wrappers, this project marks my first venture into C# programming. Despite consulting ChatGPT for guidance at times, the development process still required an entire weekend to complete to my satisfaction.

To create the Windows Service, the included ZIP file contains NSSM (https://nssm.cc/), as it proved essential in enabling the service to function correctly.

My aim in creating this tool was to provide a simple yet effective solution for backing up LogRhythm System Monitor INI files, thus sparing us the need to involve additional support teams to restore backups. I hope that this tool proves beneficial to anyone encountering similar challenges.
