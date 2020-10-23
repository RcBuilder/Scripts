using System;
using System.Diagnostics;
using System.IO;

/*
    #if DEBUG
    TraceSingleTon.Instance.SetEnabled(true);
    #endif
    TraceSingleTon.Instance.SetFilePath("D:\\trace.txt");
    TraceSingleTon.Instance.Append("AfterInstallation.Main"); 
*/

public class TraceSingleTon
{
    private static volatile TraceSingleTon _instance;
    private static object syncRoot = new Object();

    private TraceSingleTon() { }

    public static TraceSingleTon Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (syncRoot)
                {
                    if (_instance == null)
                        _instance = new TraceSingleTon();
                }
            }

            return _instance;
        }
    }

    // ---

    protected string FilePath { get; set; }
    protected bool Enabled { get; set; } = false;

    public void SetFilePath(string FilePath)
    {
        this.FilePath = FilePath;
    }

    public void SetEnabled(bool Enabled)
    {
        this.Enabled = Enabled;
    }

    public void Append(string Message)
    {
        if (!this.Enabled) return;

        try
        {
            if (string.IsNullOrEmpty(this.FilePath))
                throw new ArgumentException("FilePath is Empty, use SetFilePath to set a value.");

            File.AppendAllText(this.FilePath, $"{Message} {Environment.NewLine}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}
