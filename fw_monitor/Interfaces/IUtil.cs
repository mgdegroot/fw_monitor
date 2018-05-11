namespace fw_monitor
{
    // TODO: interface overkill???...
    public interface IUtil
    {
        bool WriteToFile(string path, string content, bool append);
        string ReadFromFile(string path, bool trhowWhenNotFound);
    }
}