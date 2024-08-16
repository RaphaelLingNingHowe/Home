namespace ProgramGuard.Services
{
    public class FileListChangeNotifier
    {
        public event EventHandler? FileListChanged;

        public void NotifyFileListChanged()
        {
            FileListChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
