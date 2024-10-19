
using System.IO;

namespace Lukextensions
{
    internal static class PathProvider
    {

        public const string ROOT_FOLDER = "Lukextensions";

        public const string SHAREPOINT_FOLDER = "Sharepoint";

        public static string GetPathForModule(string moduleFolder) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ROOT_FOLDER, SHAREPOINT_FOLDER);


        /// <summary>
        /// <para>Method that ensures a file is already created or will be created upon call.</para>
        /// <para>If the file did not exist, returns <see cref="StreamWriter"/> for the new file.</para>
        /// </summary>
        /// <param name="filePath">Path of a file.</param>
        /// <returns><see cref="StreamWriter"/> instance for newly created file.</returns>
        public static StreamWriter EnsureNewFile(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (!File.Exists(filePath))
            {
                return File.CreateText(filePath);
            }
            return null;
        }
    }
}
