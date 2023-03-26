# LogRhythm Config Backup

The 'LogRhythm Config Backup' is a Windows service that performs daily backups of the LogRhythm System Monitor configuration files.

## Installation

To install the service, simply download the latest release from the releases page and install the MSI. There are two ways to install the MSI:

* Double-click the MSI file and follow the prompts.
* Use the command line to install the MSI using the `msiexec` command. Please note that the command must be run as administrator:
bash
```
msiexec /i LogRhythm_Config_Backup_v1.0.msi
```
After installation, the service will be automatically started and configured to run daily backups of the LogRhythm System Monitor INI files.


## Configuration

The service can be configured by editing the config.ini file that is located in the same folder as the executable. The following settings can be customized:

BackupTime: The time of day when the backup should be performed. Format: HH:mm. Default: 20:00.
BackupLocation: The folder where the backups should be stored. Default: C:\LogRhythm_config_backups.
NumBackupsToKeep: The number of backups to keep. Default: 30.
Make sure to save the config.ini file after making any changes.

## Purpose

The LogRhythm System Monitor is an important agent that we use to collect logs from our customers. However, during agent upgrades, we have noticed that the settings in the INI files can disappear, causing us to lose important Office365 connections. Unfortunately, customers often do not perform backups of these INI files, which is why we created this small program. The LogRhythm Backup Service automatically performs daily backups of the INI files located on the same server as the LogRhythm System Monitor, ensuring that your important settings are never lost.
