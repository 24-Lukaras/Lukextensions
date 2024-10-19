using Lukextensions.SharePoint;
using System.IO;

namespace Lukextensions.Sharepoint
{
    [Command(PackageGuids.SharepointString, PackageIds.OpenSettings)]
    internal sealed class OpenSettings : BaseCommand<OpenSettings>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var project = await VS.Solutions.GetActiveProjectAsync();
            var path = Path.Combine(PathProvider.GetPathForModule(PathProvider.SHAREPOINT_FOLDER), $"{project.Name}.json");
            using (var newFile = PathProvider.EnsureNewFile(path))
            {
                if (newFile != null)
                {
                    await newFile.WriteAsync(new SharepointProjectSettings().ToJson());
                }
            }
            await VS.Documents.OpenAsync(path);
        }
    }
}
