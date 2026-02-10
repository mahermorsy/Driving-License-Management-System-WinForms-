using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Cn = System.Console;

namespace DVLD.Classes   // <-- غيّره هنا ليتطابق مع Default namespace للمشروع
{
    public static class clsGlobal2
    {
        // Hash password using SHA256 -> return lowercase hex (64 chars)
        public static string ChangePasswordToHash(string Password)
        {
            if (Password == null) Password = string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));
                // BitConverter.ToString يعطي "AA-BB-..." لذا نحذِف '-' ونجعل نص أحرف صغيرة
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// This Method For Application Logs.
        /// </summary>
        public static void WriteEventLogs(string logMessage, int Num)
        {
            string sourceName = "UserApp";
            string logName = "Application";

            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, logName);
            }

            if (Num == 1)
                EventLog.WriteEntry(sourceName, logMessage, EventLogEntryType.Information);
            else if (Num == 2)
                EventLog.WriteEntry(sourceName, logMessage, EventLogEntryType.Warning);
            else
                EventLog.WriteEntry(sourceName, logMessage, EventLogEntryType.Error);
        }

        public static void DeleteUserAndPasswordRegistry()
        {
            string KeyPath = @"Software\DVDLCreditionals";
            try
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                using (RegistryKey subKey = baseKey.OpenSubKey(KeyPath, true)) // true => writable
                {
                    if (subKey != null)
                    {
                        if (subKey.GetValue("UserName") != null) subKey.DeleteValue("UserName");
                        if (subKey.GetValue("Password") != null) subKey.DeleteValue("Password");
                    }
                    else
                    {
                        Cn.WriteLine("Key Registry Not Found.");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Cn.WriteLine("Unauthorized Access Exception.");
            }
            catch (Exception ex)
            {
                Cn.WriteLine("Error {0}", ex);
            }
        }

        public static void RemeberMyUsernameAndPassworByRegistry(string Username, string Password)
        {
            string KeyPath = @"HKEY_CURRENT_USER\Software\DVDLCreditionals";
            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
            {
                try
                {
                    // WARNING: Storing plain password in registry is insecure.
                    // Better: use ProtectedData.Protect(...) before storing.
                    Registry.SetValue(KeyPath, "UserName", Username, RegistryValueKind.String);
                    Registry.SetValue(KeyPath, "Password", Password, RegistryValueKind.String);
                    Cn.WriteLine("User And Password Saved To Registry.");
                }
                catch (Exception ex)
                {
                    Cn.WriteLine("Error {0}", ex);
                }
            }
            else
            {
                DeleteUserAndPasswordRegistry();
            }
        }

        public static bool RetriveUsernameAndPassworByRegistry(ref string Username, ref string Password)
        {
            bool ReturnValue = false;
            string KeyPath = @"HKEY_CURRENT_USER\Software\DVDLCreditionals";
            try
            {
                Username = Registry.GetValue(KeyPath, "UserName", null) as string;
                Password = Registry.GetValue(KeyPath, "Password", null) as string;
                if (Username != null && Password != null) ReturnValue = true;
            }
            catch (Exception ex)
            {
                Cn.WriteLine("Error {0}", ex);
            }
            return ReturnValue;
        }

        // Save credentials to a local file (not secure). Consider using ProtectedData.
        public static bool RememberUsernameAndPassword(string Username, string Password)
        {
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string filePath = Path.Combine(currentDirectory, "data.txt");

                if (string.IsNullOrEmpty(Username))
                {
                    if (File.Exists(filePath)) File.Delete(filePath);
                    return true;
                }

                // Warning: this stores password in plain text. Use DPAPI (ProtectedData) or avoid storing password.
                string dataToSave = Username + "#//#" + Password;
                File.WriteAllText(filePath, dataToSave, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public static bool GetStoredCredential(ref string Username, ref string Password)
        {
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string filePath = Path.Combine(currentDirectory, "data.txt");

                if (!File.Exists(filePath)) return false;

                string content = File.ReadAllText(filePath, Encoding.UTF8);
                if (string.IsNullOrEmpty(content)) return false;

                string[] result = content.Split(new string[] { "#//#" }, StringSplitOptions.None);
                if (result.Length >= 2)
                {
                    Username = result[0];
                    Password = result[1];
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return false;
            }
        }
    }
}
