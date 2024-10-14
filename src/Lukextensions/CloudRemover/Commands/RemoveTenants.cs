namespace Lukextensions.CloudRemover
{
    [Command(PackageGuids.CloudRemoverString, PackageIds.RemoveTenants)]
    internal sealed class RemoveTenants : BaseCommand<RemoveTenants>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var doc = await VS.Documents.GetActiveDocumentViewAsync();

            using (var edit = doc.TextBuffer.CreateEdit())
            {
                foreach (var selected in doc.TextView.Selection.SelectedSpans)
                {
                    var text = selected.GetText();

                    var tenantRemover = new TenantRemover(text);
                    var result = tenantRemover.RemoveTenantParameters();

                    edit.Replace(selected, result);
                }
                edit.Apply();
            }
        }
    }
}
