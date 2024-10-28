using Lukextensions.SharePoint;
using System.IO;

namespace Lukextensions.Sharepoint
{
    [Command(PackageGuids.SharepointString, PackageIds.MigrateFromSharepoint)]
    internal sealed class MigrateFromSharepoint : BaseCommand<MigrateFromSharepoint>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
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
                var migrator = new SharepointFirstMigration(client);

                var doc = await VS.Documents.GetActiveDocumentViewAsync();

                if (!doc.IsCSharpDocument())
                    throw new ArgumentException("File is not a CSharp document");

                if (doc.TextView.Selection.SelectedSpans.Count == 0)
                    throw new ArgumentNullException("No search phrase was selected");


                using (var edit = doc.TextBuffer.CreateEdit())
                {
                    var searchTerm = doc.TextView.Selection.SelectedSpans[0].GetText();

                    edit.Replace(doc.TextView.Selection.SelectedSpans[0], "");
                    
                    var documentContent = edit.Snapshot.GetText();
                    var result = await migrator.MigrateAsync(documentContent, searchTerm, settings.SiteId);

                    edit.Replace(0, doc.TextBuffer.CurrentSnapshot.Length, result);
                    edit.Apply();
                }
            }
            catch (Exception ex)
            {
                await VS.MessageBox.ShowErrorAsync(ex.Message);
            }            
        }
    }
}
