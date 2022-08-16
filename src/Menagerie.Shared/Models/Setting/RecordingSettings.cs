namespace Menagerie.Shared.Models.Setting;

public class RecordingSettings
{
    public bool Enabled { get; set; }
    public int Crf { get; set; }
    public int FrameRate { get; set; }
    public int ClipDuration { get; set; }
    public int ClipSaveDelay { get; set; }
    public int ClipSavePadding { get; set; }
    public int NbClipsToKeep { get; set; }
    public int CleanupTimeout { get; set; }
    public string OutputPath { get; set; }
}