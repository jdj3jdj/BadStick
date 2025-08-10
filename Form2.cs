using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Xbox_360_BadUpdate_USB_Tool
{
    public partial class Form2 : Form
    {

        private readonly HttpClient _httpClient = new HttpClient();
        public bool DriveSet = true;
        public string DevicePath = "";
        private int _totalSteps;
        private int _currentStep;
        private Dictionary<string, CheckBox> _checkBoxDict; 

        public Form2()
        {
            InitializeComponent();
            InitializeCheckBoxDict();
            LoadUsbDrives();
        }

        private class UsbDriveItem
        {
            public string RootPath { get; }
            public string DisplayName { get; }

            public UsbDriveItem(string rootPath, string volumeLabel)
            {
                RootPath = rootPath;
                DisplayName = string.IsNullOrEmpty(volumeLabel)
                    ? rootPath
                    : $"{rootPath} ({volumeLabel})";
            }

            public override string ToString() => DisplayName;
        }

        private void UpdateStatus(string text)
        {
            StatusLabel.Text = text;
        }

        private void SetProgressBar(int percent)
        {
            ProgressBar.Value = percent;
        }

        private void LoadUsbDrives()
        {
            DeviceList.Items.Clear();

            var drives = DriveInfo.GetDrives()
                .Where(d => (d.DriveType == DriveType.Removable || d.DriveType == DriveType.Fixed) && d.IsReady &&
                            string.Equals(d.DriveFormat, "FAT32", StringComparison.OrdinalIgnoreCase))
                .Select(d => new UsbDriveItem(d.RootDirectory.FullName, d.VolumeLabel))
                .ToList();

            DeviceList.Items.AddRange(drives.ToArray());

            if (DeviceList.Items.Count > 0)
            {
                DeviceList.SelectedIndex = 0;
                var firstDrive = DeviceList.Items[0] as UsbDriveItem;
                if (firstDrive != null)
                {
                    DevicePath = firstDrive.RootPath;
                    DriveSet = true;
                }
            }
            else
            {
                DevicePath = null;
                DriveSet = false;
            }
        }

        private async Task CountdownExitStatusAsync()
        {
            for (int i = 3; i >= 1; i--)
            {
                UpdateStatus($"Status: Exiting in {i}...");
                await Task.Delay(1000);
            }
            Application.Exit();
        }

        public async Task DownloadFileAsync(string url, string destinationFilePath, IProgress<int> progress = null)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    var total = response.Content.Headers.ContentLength ?? -1L;
                    var canReportProgress = total != -1 && progress != null;

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var totalRead = 0L;
                        var buffer = new byte[8192];
                        int read;
                        while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, read);
                            totalRead += read;

                            if (canReportProgress)
                            {
                                int percent = (int)((totalRead * 100L) / total);
                                progress.Report(percent);
                            }
                        }
                    }
                }
            }
        }

        private Task ExtractPackageAsync(string pkgFilePath, string destinationPath, IProgress<int> progress = null)
        {
            return Task.Run(() =>
            {
                using (var archive = ZipFile.OpenRead(pkgFilePath))
                {
                    int totalEntries = archive.Entries.Count;
                    int processedEntries = 0;

                    foreach (var entry in archive.Entries)
                    {
                        var fullPath = Path.Combine(destinationPath, entry.FullName);

                        var directory = Path.GetDirectoryName(fullPath);

                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        if (!string.IsNullOrEmpty(entry.Name))
                        {
                            entry.ExtractToFile(fullPath, overwrite: true);
                        }

                        processedEntries++;

                        if (progress != null)
                        {
                            int percent = (int)((processedEntries * 100L) / totalEntries);
                            progress.Report(percent);
                        }
                    }
                }
            });
        }


        public class PackageInfo
        {
            public string FileName { get; set; }
            public string CheckBoxName { get; set; }
            public string DownloadUrl { get; set; }
            public bool AlwaysDownload => string.IsNullOrEmpty(CheckBoxName);
        }

        private readonly List<PackageInfo> _allPackages = new List<PackageInfo>
        {
            new PackageInfo { FileName = "Aurora.zip", CheckBoxName = "AuroraToggle", DownloadUrl = "https://www.dropbox.com/scl/fi/gdgqbbg4r94g9hy1mxhan/Aurora.zip?rlkey=fr1ev0sogohayslylkbf24bhy&st=b3q3htde&dl=1" },
            new PackageInfo { FileName = "Freestyle.zip", CheckBoxName = "FSDToggle", DownloadUrl = "https://www.dropbox.com/scl/fi/idikvnbomc710o28voyy6/Freestyle.zip?rlkey=qvez1sldllnpr9enthaqqp30h&st=hln3a2ot&dl=1" },
            new PackageInfo { FileName = "Emerald.zip", CheckBoxName = "EmeraldToggle", DownloadUrl = "https://www.dropbox.com/scl/fi/t0sd1tbfgiso8djpiqdrf/Emerald.zip?rlkey=ztm50819lbq5zvq2cln8xdp54&st=hf3gwio3&dl=1" },
            new PackageInfo { FileName = "FFPlay.zip", CheckBoxName = "FFPlayToggle", DownloadUrl = "https://www.dropbox.com/scl/fi/07q3t4hrv412eg48nlsgj/FFPlay.zip?rlkey=1e47jxzatpjn71doq23wzkdxw&st=t6nr9h1f&dl=1" },
            new PackageInfo { FileName = "GOD Unlocker.zip", CheckBoxName = "GODUnlockerToggle", DownloadUrl = "https://www.dropbox.com/scl/fi/7vz83x7isrysqdjjyg7ab/GOD-Unlocker.zip?rlkey=bmz2voe0sithx1mm6234wtnvx&st=887wlt7q&dl=1" },
            new PackageInfo { FileName = "HDDx Fixer.zip", CheckBoxName = "HDDxToggle", DownloadUrl = "https://www.dropbox.com/scl/fi/ex9c72u50vy1vmeswgeh6/HDDx-Fixer.zip?rlkey=08sgeoq8rxhlbq2ys690s2042&st=o6qqq8d3&dl=1" },
            new PackageInfo { FileName = "IngeniouX.zip", CheckBoxName = "IngeniousXToggle", DownloadUrl = "https://www.dropbox.com/scl/fi/h35kqk6y7pwn5sswcgjqq/IngeniouX.zip?rlkey=pavfpt958m1hl2oxn8gzb86zu&st=i1chpp1x&dl=1" },
            new PackageInfo { FileName = "NXE2GOD.zip", CheckBoxName = "NXE2GODToggle", DownloadUrl = "https://www.dropbox.com/scl/fi/657wj1okzfrnowvxx7cca/NXE2GOD.zip?rlkey=7wkfw99cq8jdfw92s9uz00qns&st=vqyvwt8n&dl=1" },
            new PackageInfo { FileName = "Payload.zip", CheckBoxName = null, DownloadUrl = "https://www.dropbox.com/scl/fi/8qx3lq762i8yit6a65c3o/Payload.zip?rlkey=5buz3aby3pmyo1qvhqzivt1ly&st=vuh0xua1&dl=1" },
            new PackageInfo { FileName = "RBB.zip", CheckBoxName = null, DownloadUrl = "https://www.dropbox.com/scl/fi/so7auipznd9q706yxzoqa/RBB.zip?rlkey=rzevzbt1am333l3y3dcia0opl&st=1i6xms96&dl=1" },
            new PackageInfo { FileName = "Viper360.zip", CheckBoxName = "Viper360Toggle", DownloadUrl = "https://www.dropbox.com/scl/fi/kfjjf3invk3572l6arpto/Viper360.zip?rlkey=xuagzume96xgkwnygajev8cuv&st=uvwzwspv&dl=1" },
            new PackageInfo { FileName = "Xenu.zip", CheckBoxName = "XenuToggle", DownloadUrl = "https://www.dropbox.com/scl/fi/e44qsq5048bpiihqy6dov/Xenu.zip?rlkey=uaypv1g5tw7698hwzcmd3lvzo&st=8poemfw5&dl=1" },
            new PackageInfo { FileName = "XeXLoader.zip", CheckBoxName = "XeXLoaderToggle", DownloadUrl = "https://www.dropbox.com/scl/fi/4i8bd9w09nyxl0gi7b13h/XeXLoader.zip?rlkey=skc174t2pmqfr6rdirombyz71&st=cpi2hq0q&dl=1" },
            new PackageInfo { FileName = "XeXMenu.zip", CheckBoxName = null, DownloadUrl = "https://www.dropbox.com/scl/fi/4bqg2zxgpz0mpwsrkbkks/XeXMenu.zip?rlkey=dd1575yiark2r3iu76m10lvd3&st=yg353j8l&dl=1" },
            new PackageInfo { FileName = "XM360.zip", CheckBoxName = "XM360Toggle", DownloadUrl = "https://www.dropbox.com/scl/fi/xxigfbl5v4igrvo1q255l/XM360.zip?rlkey=a6q9fj72uqx7c1g5ftste1cha&st=29c58ye3&dl=1" },
            new PackageInfo { FileName = "XNA Offline.zip", CheckBoxName = "XNAToggle", DownloadUrl = "https://www.dropbox.com/scl/fi/4xd8tgwrvcwmblfb6hxul/XNA-Offline.zip?rlkey=oik3iw4907npwtzdry70s83iz&st=98crwkxz&dl=1" },
            new PackageInfo { FileName = "XPG Chameleon.zip", CheckBoxName = "XPGToggle", DownloadUrl = "https://www.dropbox.com/scl/fi/z76cwz31ifk73ofaap7yg/XPG-Chameleon.zip?rlkey=cbwj53gejsrd0fr374ors5ydg&st=ggn4slza&dl=1" },
           new PackageInfo { FileName = "Plugins.zip", CheckBoxName = "PluginsToggle", DownloadUrl = "https://www.dropbox.com/scl/fi/2mluslhrf177g3cyo4k2c/Plugins.zip?rlkey=m16n76sdsw2e6x8w9usv6txzh&st=lqdechz0&dl=1" }

        };

        private void InitializeCheckBoxDict()
        {
            _checkBoxDict = new Dictionary<string, CheckBox>
            {
                { "AuroraToggle", AuroraToggle },
                { "FSDToggle", FSDToggle },
                { "EmeraldToggle", EmeraldToggle },
                { "FFPlayToggle", FFPlayToggle },
                { "GODUnlockerToggle", GODUnlockerToggle },
                { "HDDxToggle", HDDxToggle },
                { "IngeniousXToggle", IngeniouXToggle },
                { "NXE2GODToggle", NXE2GODToggle },
                { "Viper360Toggle", Viper360Toggle },
                { "XenuToggle", XenuToggle },
                { "XeXLoaderToggle", XeXLoaderToggle },
                { "XM360Toggle", XM360Toggle },
                { "XNAToggle", XNAToggle },
                { "XPGToggle", XPGToggle },
                { "PluginsToggle", PluginsToggle }
            };
        }
        private List<PackageInfo> GetSelectedPackages()
        {
            return _allPackages.Where(pkg =>
                pkg.AlwaysDownload ||
                (_checkBoxDict.TryGetValue(pkg.CheckBoxName, out var checkbox) && checkbox.Checked)
            ).ToList();
        }

        public async Task DownloadAndExtractPackagesAsync(
        List<PackageInfo> packages,
        Dictionary<string, CheckBox> checkBoxes,
        string usbRootPath,
        IProgress<int> progress = null)
        {
            if (string.IsNullOrWhiteSpace(usbRootPath) || !Directory.Exists(usbRootPath))
            {
                UpdateStatus("Status: Please Select A Valid USB Device");
                return;
            }

            string appTempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
            if (!Directory.Exists(appTempFolder))
            {
                Directory.CreateDirectory(appTempFolder);
            }

            int totalPackages = packages.Count;
            int currentPackageIndex = 0;

            foreach (var pkg in packages)
            {
                if (!pkg.AlwaysDownload)
                {
                    if (!checkBoxes.TryGetValue(pkg.CheckBoxName, out var cb) || !cb.Checked)
                    {
                        continue;
                    }
                }

                try
                {
                    currentPackageIndex++;
                    UpdateStatus($"Status: Downloading {pkg.FileName} ({currentPackageIndex}/{totalPackages})");
                    var tempFilePath = Path.Combine(appTempFolder, pkg.FileName);
                    var downloadProgress = new Progress<int>(percent =>
                    {
                        int overallPercent = (int)(((currentPackageIndex - 1 + (percent / 100.0)) / totalPackages) * 100 * 0.5);
                        SetProgressBar(overallPercent);
                    });
                    await DownloadFileAsync(pkg.DownloadUrl, tempFilePath, downloadProgress);
                    UpdateStatus($"Status: Extracting {pkg.FileName} ({currentPackageIndex}/{totalPackages})");
                    var extractProgress = new Progress<int>(percent =>
                    {
                        int overallPercent = (int)(((currentPackageIndex - 1 + (percent / 100.0)) / totalPackages) * 100 * 0.5 + 50);
                        SetProgressBar(overallPercent);
                    });
                    await ExtractPackageAsync(tempFilePath, usbRootPath, extractProgress);
                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Status: Error with {pkg.FileName}: {ex.Message}");
                }
            }
            UpdateStatus("Status: All Downloads Completed");
            SetProgressBar(100);
        }

        private void DeviceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DeviceList.SelectedItem is UsbDriveItem selectedDrive)
            {
                DevicePath = selectedDrive.RootPath;
                DriveSet = true;
                Debug.WriteLine($"Selected drive: {DevicePath}");
            }
            else
            {
                DevicePath = null;
                DriveSet = false;
            }
        }


        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private bool FormatDriveToFat32(string drivePath)
        {
            try
            {
                string driveLetter = Path.GetPathRoot(drivePath).TrimEnd('\\');

                string query = $"SELECT * FROM Win32_Volume WHERE DriveLetter = '{driveLetter}'";

                using (var searcher = new ManagementObjectSearcher(query))
                {
                    var volumes = searcher.Get();

                    foreach (ManagementObject volume in volumes)
                    {
                        var inParams = volume.GetMethodParameters("Format");
                        inParams["FileSystem"] = "FAT32";
                        inParams["QuickFormat"] = true;

                        ManagementBaseObject outParams = volume.InvokeMethod("Format", inParams, null);

                        uint returnValue = (uint)(outParams.Properties["ReturnValue"].Value);

                        if (returnValue == 0)
                        {
                            return true;
                        }
                        else
                        {
                            MessageBox.Show($"Failed to format drive. WMI Format returned error code: {returnValue}", "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Debug.WriteLine($"Format failed with error code: {returnValue}");
                            return false;
                        }
                    }
                }

                MessageBox.Show("Drive not found or inaccessible for formatting.", "BadStick Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error formatting drive: {ex.Message}", "BadStick Format Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async void StartBtn_Click(object sender, EventArgs e)
        {
            if (DeviceList.SelectedItem == null)
            {
                MessageBox.Show("Please select a USB device.", "Badstick Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string usbPath = DeviceList.SelectedItem.ToString();

            if (string.IsNullOrEmpty(usbPath) || !Directory.Exists(usbPath))
            {
                MessageBox.Show("Please select a valid USB device.", "BadStick Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Are you sure you want to format {usbPath} to FAT32? This will erase all data on the device.",
                "Confirm Format",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
            {
                UpdateStatus("Status: Format cancelled");
                return;
            }

            UpdateStatus("Status: Formatting device...");
            ProgressBar.Value = 0;
            bool formatSuccess = await Task.Run(() => FormatDriveToFat32(usbPath));
            if (!formatSuccess)
            {
                MessageBox.Show("Failed to format the device.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Status: Format failed");
                return;
            }
            UpdateStatus("Status: Format completed. Starting downloads...");
            var packagesToDownload = GetSelectedPackages();
            _totalSteps = packagesToDownload.Count;
            foreach (var pkg in packagesToDownload)
            {
                string tempFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", pkg.FileName);
                if (File.Exists(tempFilePath))
                {
                    using (var archive = ZipFile.OpenRead(tempFilePath))
                    {
                        _totalSteps += archive.Entries.Count;
                    }
                }
                else
                {
                    _totalSteps += 10;
                }
            }

            _currentStep = 0;

            var progress = new Progress<int>(percent =>
            {
                ProgressBar.Value = percent;
            });

            await DownloadAndExtractPackagesAsync(packagesToDownload, _checkBoxDict, usbPath, progress);

            var overallProgress = new Progress<int>(percent => SetProgressBar(percent));
            await DownloadAndExtractPackagesAsync(packagesToDownload, _checkBoxDict, usbPath, overallProgress);

            UpdateStatus("Status: Done! USB Ready.");
            ProgressBar.Value = 100;
            MessageBox.Show("Your USB is ready!\n\nYou are now free to SAFELY eject your USB and insert into your" +
                "Xbox 360 Console, and copy all of the downloaded extras onto your hard drive via XeXMenu. Do" +
                "not however, copy Rock Band Blitz or the BadUpdate payload to your consoles hard drive, those" +
                "need to remain on your USB stick for the BadUpdate exploit to function properly.", "BadStick", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Thread.Sleep(500);
            await CountdownExitStatusAsync();
        }

        private void SelectAllToggle_CheckedChanged(object sender, EventArgs e)
        {
            bool checkAll = SelectAllToggle.Checked;

            foreach (var kvp in _checkBoxDict)
            {
                kvp.Value.Checked = checkAll;
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void RefDrivesBtn_Click(object sender, EventArgs e)
        {
            LoadUsbDrives();
        }
    }
}
