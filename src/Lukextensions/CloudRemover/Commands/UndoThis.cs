
namespace Lukextensions.CloudRemover
{
    [Command(PackageGuids.CloudRemoverString, PackageIds.UndoThis)]
    internal sealed class UndoThis : BaseCommand<UndoThis>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
                var doc = await VS.Documents.GetActiveDocumentViewAsync();

                if (!doc.IsCSharpDocument())
                    throw new ArgumentException("File is not a CSharp document");

                var solution = await VS.Solutions.GetCurrentSolutionAsync();
                if (solution is null)
                    throw new ArgumentNullException("Solution path is null");

                var documentContent = doc.TextBuffer.CurrentSnapshot.GetText();
                ThisRemover remover = new ThisRemover(documentContent);

                var result = remover.UndoThis();

                using (var edit = doc.TextBuffer.CreateEdit())
                {
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
