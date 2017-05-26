using System;
using System.Drawing;
using System.Windows.Forms;

namespace Com.GitHub.ZachDeibert.CommandWrapper {
	public class AskForm : Form {
		private TextBox Input;
		private CheckBox RememberBox;

		public string Value {
			get {
				return Input.Text;
			}
		}

		public bool RememberValue {
			get {
				return RememberBox.Checked;
			}
		}

		public AskForm(string prompt) {
			TableLayoutPanel layout = new TableLayoutPanel();
			layout.Dock = DockStyle.Fill;
			layout.RowCount = 4;
			layout.ColumnCount = 1;
			layout.Parent = this;

			Label label = new Label();
			label.Text = prompt;
			label.Dock = DockStyle.Fill;
			label.Parent = layout;
			layout.SetCellPosition(label, new TableLayoutPanelCellPosition(0, 0));

			Input = new TextBox();
			Input.Dock = DockStyle.Fill;
			Input.Parent = layout;
			layout.SetCellPosition(Input, new TableLayoutPanelCellPosition(0, 1));

			RememberBox = new CheckBox();
			RememberBox.Text = "Remember this information";
			RememberBox.Dock = DockStyle.Fill;
			RememberBox.Parent = layout;
			layout.SetCellPosition(RememberBox, new TableLayoutPanelCellPosition(0, 2));

			TableLayoutPanel btnPanel = new TableLayoutPanel();
			btnPanel.RowCount = 1;
			btnPanel.ColumnCount = 3;
			btnPanel.Parent = layout;
			layout.SetCellPosition(btnPanel, new TableLayoutPanelCellPosition(0, 3));

			Button submit = new Button();
			submit.Text = "Submit";
			submit.Parent = btnPanel;
			btnPanel.SetCellPosition(submit, new TableLayoutPanelCellPosition(2, 0));
			AcceptButton = submit;
			submit.Click += (sender, e) => DialogResult = DialogResult.OK;

			Button cancel = new Button();
			cancel.Text = "Cancel";
			cancel.Parent = btnPanel;
			btnPanel.SetCellPosition(cancel, new TableLayoutPanelCellPosition(1, 0));
			CancelButton = cancel;

			Margin = new Padding(10);
			Text = "User Input";
			Size = new Size(400, 150);
			CenterToScreen();
		}
	}
}

