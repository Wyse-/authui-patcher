using System;
using System.Drawing;
using System.Windows.Forms;

namespace authui_patcher
{
    public partial class MainForm : Form
    {
        private bool successfulLoad = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckForSuccessfulLoad();
        }


        /// <summary>
        /// Checks for successful load of the authui.dll file.
        /// </summary>
        private void CheckForSuccessfulLoad()
        {
            patchButton.Enabled = false;
            restoreButton.Enabled = false;
            if (PatchManagement.ReadAuthuiBytes())
            {
                CheckIfPatched();
                if (PatchManagement.CleanAuthuiString())
                {
                    successfulLoad = true;
                }
            }
        }

        /// <summary>
        /// On click of the patch button attempt patching authui.dll and display a success or an error messagebox based on the result.
        /// </summary>
        private void PatchButton_Click(object sender, EventArgs e)
        {
            if (PatchManagement.PatchAuthui() && successfulLoad)
            {
                CheckIfPatched();
                MessageBox.Show("Authui successfully patched", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Authui patching failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Check if authui.dll is patched, then set label text and buttons accordingly.
        /// </summary>
        private void CheckIfPatched()
        {
            if (PatchManagement.IsAuthuiPatched())
            {
                infoLabel.ForeColor = Color.Green;
                infoLabel.Text = "Authui.dll is patched, 256 KB limit bypassed.";
                restoreButton.Enabled = true;
                patchButton.Enabled = false;
            }
            else
            {
                infoLabel.ForeColor = Color.Red;
                infoLabel.Text = "Authui.dll is not patched, 256 KB limit active.";
                patchButton.Enabled = true;
                restoreButton.Enabled = false;
            }
        }


        /// <summary>
        /// On click of the patch button attempt restoring authui.dll and display a success or an error messagebox based on the result. 
        /// If restoring failes the user is asked if they want to try restoring authui.dll after killing explorer.exe (it hooks onto the authui.dll file and prevents it from being deleted); explorer.exe is automatically restarted by Windows after being killed.
        /// </summary>
        private void RestoreButton_Click(object sender, EventArgs e)
        {
            if (PatchManagement.RestoreAuthui())
            {
                CheckIfPatched();
                MessageBox.Show("Authui.dll successfully restored", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if(MessageBox.Show("Authui.dll restoration failed, retry after killing explorer.exe?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes){
                    if (PatchManagement.RestoreAuthuiKillingExplorer())
                    {
                        CheckIfPatched();
                        MessageBox.Show("Authui.dll successfully restored", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Authui.dll restoration after killing explorer.exe failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
