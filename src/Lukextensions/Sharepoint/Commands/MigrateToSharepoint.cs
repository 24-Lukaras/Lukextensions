using Lukextensions.Common;
using Lukextensions.SharePoint;
using System.IO;
using System.Linq;

namespace Lukextensions.Sharepoint
{
    [Command(PackageGuids.SharepointString, PackageIds.MigrateToSharepoint)]
    internal sealed class MigrateToSharepoint : BaseCommand<MigrateToSharepoint>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var project = await VS.Solutions.GetActiveProjectAsync();
            var path = Path.Combine(PathProvider.GetPathForModule(PathProvider.SHAREPOINT_FOLDER), $"{project.Name}.json");

            if (!File.Exists(path))
            {
                if (await VS.MessageBox.ShowConfirmAsync("Settings configuration for this project does not exist!", "Create settings file?"))
                {
                    await VS.Commands.ExecuteAsync(PackageGuids.Sharepoint, PackageIds.OpenSettings);
                }
                return;
            }

            string settingsContent;
            using (var reader = new StreamReader(path)) 
            {
                settingsContent = await reader.ReadToEndAsync();
            }
            var settings = SharepointProjectSettings.FromJson(settingsContent);
            var client = new GraphClient(settings);
            var migrator = new CodeFirstMigrator(client);

            var documents = await VS.Solutions.GetActiveItemsAsync();

            foreach (var activeDocument in documents)
            {
                if (!await VS.Documents.IsOpenAsync(activeDocument.FullPath))
                {
                    await VS.Documents.OpenAsync(activeDocument.FullPath);
                }

                var doc = await VS.Documents.GetDocumentViewAsync(activeDocument.FullPath);

                if (!doc.IsCSharpDocument())
                    throw new ArgumentException("File is not a CSharp document");

                var documentContent = doc.TextBuffer.CurrentSnapshot.GetText();

                var result = await migrator.MigrateAsync(documentContent, settings.SiteId);

                using (var edit = doc.TextBuffer.CreateEdit())
                {
                    edit.Replace(0, doc.TextBuffer.CurrentSnapshot.Length, result);
                    edit.Apply();
                }
            }
        }
    }
}
