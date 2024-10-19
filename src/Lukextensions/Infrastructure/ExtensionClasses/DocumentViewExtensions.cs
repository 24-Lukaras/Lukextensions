using System.IO;

namespace Lukextensions;

internal static class DocumentViewExtensions
{
    public static bool IsCSharpDocument(this DocumentView document) => Path.GetExtension(document.FilePath).ToLowerInvariant() == ".cs";
}
