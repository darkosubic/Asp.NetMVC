using System.Configuration;

namespace DarkoMVCApp.Helpers
{
    public interface IWebConfigReader
    {
        string DefaultCollectionReader { get; }
        int MaxContactsPerPage { get; }
    }

    public class WebConfigReader : IWebConfigReader
    {
        public string DefaultCollectionReader => ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public int MaxContactsPerPage => int.Parse(ConfigurationManager.AppSettings["maxContactsPerPage"]);
    }
}