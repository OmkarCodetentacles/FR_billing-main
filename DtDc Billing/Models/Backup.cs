using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

public class BackupDbTask
{


    private string _sqlserverUser;
    public string SQLServerUser
    {
        get { return _sqlserverUser; }
        set { _sqlserverUser = value; }
    }
    private string _sqlserverPassword;
    public string SQLServerPassword
    {
        get { return _sqlserverPassword; }
        set { _sqlserverPassword = value; }
    }
    private string _sqlserverInstanceName;
    public string SqlServerInstanceName
    {
        get { return _sqlserverInstanceName; }
        set { _sqlserverInstanceName = value; }
    }
    private string _sqlserverDBName;
    public string SqlServerDBName
    {
        get { return _sqlserverDBName; }
        set { _sqlserverDBName = value; }
    }

    private string _databaseBackupFolder;

    public string DatabaseBackupFolder
    {
        get { return _databaseBackupFolder; }
        set { _databaseBackupFolder = value; }
    }

    private string _backupFile;

    public string BackupFile
    {
        get { return _backupFile; }
        set { _backupFile = value; }
    }


    public void Execute()
    {

        // Backup Db
        try
        {
            ServerConnection conn = new ServerConnection();
            conn.LoginSecure = false;
            conn.DatabaseName = "DtdcBilling";
            conn.ServerInstance = "43.255.152.26";
            conn.Login = "DtdcBilling";
            conn.Password = "Billing@123";

            Server svr = new Server(conn);
            Database BuildDB = svr.Databases[this.SqlServerDBName];

            string dbbackupfile = this.DatabaseBackupFolder + @"\" + this.SqlServerDBName + ".bak"; ;


            Backup backup = new Backup();
            backup.Database = this.SqlServerDBName;
            backup.MediaName = "FileSystem";
            BackupDeviceItem bkpDeviceItem = new BackupDeviceItem();
            bkpDeviceItem.DeviceType = DeviceType.File;
            bkpDeviceItem.Name = dbbackupfile;
            backup.Devices.Add(bkpDeviceItem);
            backup.Initialize = true;
            backup.SqlBackup(svr);

            this.BackupFile = dbbackupfile;
        }
        catch (Exception ex)
        {


        }


    }
}