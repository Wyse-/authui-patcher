using System;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace authui_patcher
{
    class PatchManagement
    {
#if WIN64
        private const string OriginalHexString = "E8-03-00-41-3B-C1-73-3E";   //
        private const string PatchedHexString = "E8-03-00-41-3B-C1-90-90";    // Conditional compilation symbols management: if a x64 executable is being compiled declare the x64 hex values as constants.
#endif                                                                        // 

#if WIN32                                                                     //
        private const string OriginalHexString = "3D-00-E8-03-00-73-51";      // Conditional compilation symbols management: if a x86 executable is being compiled declare the x86 hex values as constants.
        private const string PatchedHexString = "3D-00-E8-03-00-90-90";       //
#endif

        private const string AuthuiPath = @"C:\Windows\System32\authui.dll"; // Authui.dll file path.
        private const string AuthuiBackupPath = @"C:\Windows\System32\authui.dll_bak"; // Authui.dll backup file path, created when patching authui.dll.

        private static ProcessStartInfo takeown = new ProcessStartInfo("takeown", $"/f {AuthuiPath}"); // Process used to take ownership of the authui.dll file, which is necessary to change its permissions.
        private static ProcessStartInfo icacls = new ProcessStartInfo("icacls", $"{AuthuiPath} /grant administrators:F"); // Process used to edit permissions of the authui.dll file, giving full access to admins.
        private static byte[] fileBytes; // Authui.dll file stored as a byte array.
        private static string fileHex; // Authui.dll file stored as an hex string.
        private static string patchedFileHex; // Patched authui.dll file stored as an hex string.


        /// <summary>
        /// Read the authui.dll file as bytes and store its contents in the fileBytes byte array, then convert said byte array to hexadecimal and store it in fileHex.
        /// </summary>
        public static bool ReadAuthuiBytes()
        {
            try
            {
                fileBytes = File.ReadAllBytes(AuthuiPath);
                fileHex = BitConverter.ToString(fileBytes);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check if authui.dll is patched by verifying if its contents stored as hexadecimal contain the patched hex string. 
        /// </summary>
        /// <returns>
        /// True if authui.dll is patched.
        /// False if authui.dll is not patched.
        /// </returns>
        public static bool IsAuthuiPatched()
        {
            ReadAuthuiBytes();
            return fileHex.Contains(PatchedHexString);
        }

        /// <summary>
        /// Replace the original hex string in fileHex with the patched one and store the result in patchedFileHex, then clean it from the "-" characters.
        /// </summary>
        /// <returns>
        /// True if replacement and cleaning are successful.
        /// False if replacement and cleaning are not successful.
        /// </returns>
        public static bool CleanAuthuiString()
        {
           try
           {
             patchedFileHex = fileHex.Replace(OriginalHexString, PatchedHexString);
             patchedFileHex = patchedFileHex.Replace("-", "");
             return true;
           }
           catch (Exception)
           {
               return false;
           }
        }

        /// <summary>
        /// Patch authui.dll:
        /// - Call TakeOwnership function.
        /// - Rename authui.dll to authui.dll_bak to store it as a backup copy.
        /// - Create a new authui.dll file from the contents of patchedFileHex converted to a byte array.
        /// </summary>
        /// <returns>
        /// True if patching is successful.
        /// False if patching is not successful.
        /// </returns>
        public static bool PatchAuthui()
        {
            try
            {
                TakeOwnership();
                File.Move(AuthuiPath, AuthuiBackupPath);
                File.WriteAllBytes(AuthuiPath, StringToByteArray(patchedFileHex));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        /// <summary>
        /// Restore backup copy of authui.dll:
        /// - Call TakeOwnership function.
        /// - Delete authui.dll.
        /// - Rename authui.dll_bak to authui.dll.
        /// </summary>
        /// <returns>
        /// True if restoration is successful.
        /// False if restoration is not successful.
        /// </returns>
        public static bool RestoreAuthui()
        {
            try
            {
                TakeOwnership();
                File.Delete(AuthuiPath);
                File.Move(AuthuiBackupPath, AuthuiPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Restore backup copy of authui.dll after killing explorer.exe:
        /// - Kill explorer.exe.
        /// - Call TakeOwnership function.
        /// - Delete authui.dll.
        /// - Rename authui.dll_bak to authui.dll.
        /// - By default, explorer.exe will automatically be restarted by Windows.
        /// </summary>
        /// <returns>
        /// True if restoration is successful.
        /// False if restoration is not successful.
        /// </returns>
        /// <remarks>
        /// This is done because explorer.exe hooks onto authui.dll, preventing it deletion.
        /// </remarks>
        public static bool RestoreAuthuiKillingExplorer()
        {
            
            try
            {
                Process.GetProcessesByName("explorer")[0].Kill();
                TakeOwnership();
                File.Delete(AuthuiPath);
                File.Move(AuthuiBackupPath, AuthuiPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Take ownership of authui.dll:
        /// - Take ownership of the authui.dll by silently running takeown.
        /// - Give full permissions for admins to authui.dll by silently running icacls.
        /// </summary>
        private static void TakeOwnership()
        {
            takeown.WindowStyle = ProcessWindowStyle.Hidden;
            icacls.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(takeown);
            Process.Start(icacls);
        }

        /// <summary>
        /// Convert hex string to byte array.
        /// </summary>
        /// <param name="hex">Hex string to be converted.</param>
        /// <returns>
        /// Resulting byte array.
        /// </returns>
        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

    }
}
