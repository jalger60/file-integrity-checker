using System.Windows.Forms;

namespace FileIntegrityChecker.WinForms.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null!;
        private TextBox txtRoot;
        private Button btnBrowse;
        private Button btnCreateBaseline;
        private Button btnScan;
        private Button btnExport;
        private Button btnSign;
        private Button btnVerify;
        private DataGridView grid;
        private Label lblStatus;
        private Label baselinePathLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtRoot = new TextBox();
            btnBrowse = new Button();
            btnCreateBaseline = new Button();
            btnScan = new Button();
            btnExport = new Button();
            btnSign = new Button();
            btnVerify = new Button();
            grid = new DataGridView();
            lblStatus = new Label();
            baselinePathLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            SuspendLayout();

            // txtRoot
            txtRoot.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRoot.Location = new System.Drawing.Point(12, 12);
            txtRoot.Size = new System.Drawing.Size(640, 23);

            // btnBrowse
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new System.Drawing.Point(658, 12);
            btnBrowse.Size = new System.Drawing.Size(90, 23);
            btnBrowse.Text = "Select Folder";
            btnBrowse.Click += btnBrowse_Click;

            // btnCreateBaseline
            btnCreateBaseline.Location = new System.Drawing.Point(12, 50);
            btnCreateBaseline.Size = new System.Drawing.Size(130, 30);
            btnCreateBaseline.Text = "Create Baseline";
            btnCreateBaseline.Click += btnCreateBaseline_Click;

            // btnScan
            btnScan.Location = new System.Drawing.Point(148, 50);
            btnScan.Size = new System.Drawing.Size(110, 30);
            btnScan.Text = "Scan && Compare";
            btnScan.Click += btnScan_Click;

            // btnExport
            btnExport.Location = new System.Drawing.Point(264, 50);
            btnExport.Size = new System.Drawing.Size(110, 30);
            btnExport.Text = "Export Report";
            btnExport.Click += btnExport_Click;

            // btnSign
            btnSign.Location = new System.Drawing.Point(380, 50);
            btnSign.Size = new System.Drawing.Size(120, 30);
            btnSign.Text = "Sign Baseline";
            btnSign.Click += btnSign_Click;

            // btnVerify
            btnVerify.Location = new System.Drawing.Point(506, 50);
            btnVerify.Size = new System.Drawing.Size(120, 30);
            btnVerify.Text = "Verify Signature";
            btnVerify.Click += btnVerify_Click;

            // grid
            grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grid.Location = new System.Drawing.Point(12, 95);
            grid.Size = new System.Drawing.Size(736, 380);
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // baselinePathLabel
            baselinePathLabel.Location = new System.Drawing.Point(12, 78);
            baselinePathLabel.Size = new System.Drawing.Size(736, 15);
            baselinePathLabel.Text = "Baseline: (none)";

            // lblStatus
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.Location = new System.Drawing.Point(12, 482);
            lblStatus.Size = new System.Drawing.Size(736, 23);
            lblStatus.Text = "Ready";

            // MainForm
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(760, 514);
            Controls.Add(txtRoot);
            Controls.Add(btnBrowse);
            Controls.Add(btnCreateBaseline);
            Controls.Add(btnScan);
            Controls.Add(btnExport);
            Controls.Add(btnSign);
            Controls.Add(btnVerify);
            Controls.Add(grid);
            Controls.Add(lblStatus);
            Controls.Add(baselinePathLabel);
            MinimumSize = new System.Drawing.Size(720, 400);
            Text = "File Integrity Checker – PNW (Alec Rodgers, James Alger, Bryan)";
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
