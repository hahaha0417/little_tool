namespace CppDocGenerator.Services;

public static class TextPromptDialog
{
    public static string? Show(IWin32Window owner, string title, string prompt, string defaultValue = "")
    {
        using var form = new Form();
        using var label = new Label();
        using var textBox = new TextBox();
        using var okButton = new Button();
        using var cancelButton = new Button();

        form.Text = title;
        form.ClientSize = new Size(420, 130);
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.StartPosition = FormStartPosition.CenterParent;
        form.MaximizeBox = false;
        form.MinimizeBox = false;
        form.ShowInTaskbar = false;

        label.AutoSize = true;
        label.Location = new Point(15, 15);
        label.Text = prompt;

        textBox.Location = new Point(15, 40);
        textBox.Size = new Size(390, 23);
        textBox.Text = defaultValue;

        okButton.Text = "確定";
        okButton.Location = new Point(249, 82);
        okButton.DialogResult = DialogResult.OK;

        cancelButton.Text = "取消";
        cancelButton.Location = new Point(330, 82);
        cancelButton.DialogResult = DialogResult.Cancel;

        form.Controls.Add(label);
        form.Controls.Add(textBox);
        form.Controls.Add(okButton);
        form.Controls.Add(cancelButton);
        form.AcceptButton = okButton;
        form.CancelButton = cancelButton;

        return form.ShowDialog(owner) == DialogResult.OK
            ? textBox.Text.Trim()
            : null;
    }
}
