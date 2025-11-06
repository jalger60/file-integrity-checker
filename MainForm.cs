using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FileIntegrityChecker.WinForms.Models;
using FileIntegrityChecker.WinForms.Services;

namespace FileIntegrityChecker.WinForms.Forms
{
    public partial class MainForm : Form
    {
        private IntegrityStore? _baseline;
        private List<ScanResultRow> _latestResults = new();
        private string BaselinesDir => Path.Combine(AppContext.BaseDirectory, "Baselines");
        private string ReportsDir => Path.Combine(AppContext.BaseDirectory, "Reports");

        public MainForm()
        {
            InitializeComponent();
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "RelativePath", HeaderText = "Relative Path", DataPropertyName = "RelativePath" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BaselineHash", HeaderText = "Baseline SHA-256", DataPropertyName = "BaselineHash" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CurrentHash", HeaderText = "Current SHA-256", DataPropertyName = "CurrentHash" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", DataPropertyName = "Status" });
            grid.RowPrePaint += Grid_RowPrePaint;
        }

        private void Grid_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = grid.Rows[e.RowIndex];
            if (row.DataBoundItem is ScanResultRow r)
            {
                switch (r.Status)
                {
                    case IntegrityStatus.Unchanged: row.DefaultCellStyle.BackColor = Color.White; break;
                    case IntegrityStatus.Modified: row.DefaultCellStyle.BackColor = Color.MistyRose; break;
                    case IntegrityStatus.New: row.DefaultCellStyle.BackColor = Color.Honeydew; break;
                    case IntegrityStatus.Missing: row.DefaultCellStyle.BackColor = Color.LemonChiffon; break;
                }
            }
        }

        private void btnBrowse_Click(object? sender, EventArgs e)
        {
            using var f = new FolderBrowserDialog();
            if (f.ShowDialog() == DialogResult.OK) txtRoot.Text = f.SelectedPath;
        }

        private void btnCreateBaseline_Click(object? sender, EventArgs e)
        {
            try
            {
                var root = ValidateRoot();
                Logger.Log($"Creating baseline for '{root}'");
                _baseline = IntegrityStore.CreateBaseline(root, DefaultIncludeFilter);
                Directory.CreateDirectory(BaselinesDir);
                var path = Path.Combine(BaselinesDir, $"baseline_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                _baseline.Save(path);
                baselinePathLabel.Text = $"Baseline: {path}";
                lblStatus.Text = $"Baseline created with {_baseline.Records.Count} files.";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); Logger.Log($"ERROR CreateBaseline: {ex}"); }
        }

        private void btnScan_Click(object? sender, EventArgs e)
        {
            try
            {
                var root = ValidateRoot();
                if (_baseline == null)
                {
                    var existing = TryChooseBaseline();
                    if (existing == null) return;
                    _baseline = IntegrityStore.Load(existing);
                    baselinePathLabel.Text = $"Baseline: {existing}";
                }

                if (HashService.NormalizeDirectory(root) != _baseline.RootDirectory)
                {
                    MessageBox.Show("Selected folder does not match the baseline root.", "Baseline Mismatch");
                    return;
                }

                Logger.Log($"Scanning '{root}' against baseline");
                _latestResults = Scanner.CompareToBaseline(_baseline);
                BindResults(_latestResults);
                var counts = _latestResults.GroupBy(r => r.Status).ToDictionary(g => g.Key, g => g.Count());
                lblStatus.Text =
                    $"Scan complete. Unchanged={counts.GetValueOrDefault(IntegrityStatus.Unchanged)}, " +
                    $"Modified={counts.GetValueOrDefault(IntegrityStatus.Modified)}, " +
                    $"New={counts.GetValueOrDefault(IntegrityStatus.New)}, " +
                    $"Missing={counts.GetValueOrDefault(IntegrityStatus.Missing)}";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); Logger.Log($"ERROR Scan: {ex}"); }
        }

        private void btnExport_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!_latestResults.Any()) { MessageBox.Show("No results to export. Run a scan first."); return; }
                var path = ReportExporter.ExportCsv(_latestResults, ReportsDir);
                Logger.Log($"Report exported: {path}");
                lblStatus.Text = $"Report exported to {path}";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = path, UseShellExecute = true });
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); Logger.Log($"ERROR Export: {ex}"); }
        }

        private void btnSign_Click(object? sender, EventArgs e)
        {
            try
            {
                var baselinePath = GetBaselinePathFromLabel();
                if (baselinePath == null) { MessageBox.Show("No baseline selected."); return; }
                var sigPath = SignatureService.SignFile(baselinePath);
                Logger.Log($"Baseline signed: {sigPath}");
                lblStatus.Text = $"Baseline signed: {sigPath}";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); Logger.Log($"ERROR Sign: {ex}"); }
        }

        private void btnVerify_Click(object? sender, EventArgs e)
        {
            try
            {
                var baselinePath = GetBaselinePathFromLabel();
                if (baselinePath == null) { MessageBox.Show("No baseline selected."); return; }
                var ok = SignatureService.VerifyFile(baselinePath);
                lblStatus.Text = ok ? "Signature VALID" : "Signature INVALID or missing";
                Logger.Log($"Verify signature: {ok}");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); Logger.Log($"ERROR Verify: {ex}"); }
        }

        private void BindResults(List<ScanResultRow> rows) { grid.DataSource = null; grid.DataSource = rows; }

        private string ValidateRoot()
        {
            var root = txtRoot.Text;
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
                throw new InvalidOperationException("Please select a valid root folder.");
            return HashService.NormalizeDirectory(root);
        }

        private bool DefaultIncludeFilter(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext is ".tmp" or ".log" or ".lnk") return false;
            var name = Path.GetFileName(path);
            if (name.StartsWith("~")) return false;
            return true;
        }

        private string? TryChooseBaseline()
        {
            using var ofd = new OpenFileDialog { Filter = "Baseline JSON (*.json)|*.json|All files (*.*)|*.*" };
            ofd.InitialDirectory = Path.Combine(AppContext.BaseDirectory, "Baselines");
            return ofd.ShowDialog() == DialogResult.OK ? ofd.FileName : null;
        }

        private string? GetBaselinePathFromLabel()
        {
            const string prefix = "Baseline: ";
            if (baselinePathLabel.Text.StartsWith(prefix))
            {
                var path = baselinePathLabel.Text.Substring(prefix.Length);
                return File.Exists(path) ? path : null;
            }
            return null;
        }
    }
}