using DVLD_Buisness;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Cn = System.Console;


namespace DVLD.Classes
{
    internal static  class clsGlobal
    {
        public static clsUser CurrentUser;

        ///<summary>
        ///This Method For Application Logs.
        ///<summary>
        public static string ChangePasswordToHash(string Password) 
        {
            using (SHA256 Sha256 = SHA256.Create()) 
            {
                byte[] bytes = Sha256.ComputeHash(Encoding.UTF8.GetBytes(Password));

                return BitConverter.ToString(bytes).Replace("-",""); ;   
            
            }
        
        }
        public static void WriteEventLogs(string logMessage,int Num)
        {
            string sourceName = "UserApp";
            string logName = "Application"; // صح بدل Appllication

            // إنشاء الـ Source لو مش موجود
            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, logName);

            }
            // كتابة رسائل في الـ Event Log
            if (Num == 1)
            {
                EventLog.WriteEntry(sourceName, logMessage, EventLogEntryType.Information);
             
            }
            else if (Num == 2)
            {
                EventLog.WriteEntry(sourceName, logMessage, EventLogEntryType.Warning);
           
                
            }
            else
            {
                EventLog.WriteEntry(sourceName, logMessage, EventLogEntryType.Error);
            }
      
        }
        public static void DeleteUserAndPasswordRegistry()
        {
            string KeyPath = @"Software\DVDLCreditionals";
            //string KeyPath1 = @"Software\Password";
            try
            {
                using (RegistryKey BaseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser,RegistryView.Registry64)) 
                {
                    using (RegistryKey SubKey = BaseKey.OpenSubKey(KeyPath)) 
                    {
                        if (SubKey != null)
                        {
                            SubKey.DeleteValue("UserName");
                            SubKey.DeleteValue("Password");
                        }
                        else
                        {
                            Cn.WriteLine("Key Registy Not Found.");
                        }
                    }
                }       
            }
            catch (UnauthorizedAccessException)
            {
                
                Cn.WriteLine("Unauthorized Access Exception.");

            }
            catch (Exception Ex)
            {
                Cn.WriteLine("Error {0}", Ex);

            }      
        }
        public static void RemeberMyUsernameAndPassworByRegistry(string Username, string Password)
        {
            string KeyPath = @"HKEY_CURRENT_USER\Software\DVDLCreditionals";
            if ( ! string.IsNullOrWhiteSpace(Username) && ! string.IsNullOrWhiteSpace(Password) )
            {
            

                try
                {
                    Registry.SetValue(KeyPath,"UserName", Username, RegistryValueKind.String);
                    Registry.SetValue(KeyPath,"Password", Password,RegistryValueKind.String);

                    Cn.WriteLine("User And Password Name Saved As To Remember Me.");
                }
                catch (Exception Ex)
                {
                    Cn.WriteLine("Error {0}", Ex);
                }
            }
            else 
            {
                DeleteUserAndPasswordRegistry();
            }
        }
        public static bool RetriveUsernameAndPassworByRegistry(ref string Username,ref string Password)
        {
            bool ReturnValue = false;
            string KeyPath = @"HKEY_CURRENT_USER\Software\DVDLCreditionals";
            try
            {
                Username = Registry.GetValue(KeyPath, "UserName", null)as string;
                Password = Registry.GetValue(KeyPath, "Password", null)as string;
                if (Username != null && Password!=null) 
                {
                    ReturnValue = true;
                }

            }
            catch (Exception Ex)
            {
                Cn.WriteLine("Error {0}", Ex);

            }
            return ReturnValue;
        
         
        }
        public static bool RememberUsernameAndPassword(string Username, string Password)
        {

            try
            {
                //this will get the current project directory folder.
                string currentDirectory = System.IO.Directory.GetCurrentDirectory();


                // Define the path to the text file where you want to save the data
                string filePath = currentDirectory + "\\data.txt";

                //incase the username is empty, delete the file
                if (Username=="" && File.Exists(filePath)) 
                { 
                     File.Delete(filePath);
                    return true;

                }

                // concatonate username and passwrod withe seperator.
                string dataToSave = Username + "#//#"+Password ;

                // Create a StreamWriter to write to the file
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write the data to the file
                    writer.WriteLine(dataToSave);
                   
                     return true;
                }
            }
            catch (Exception ex)
            {
               MessageBox.Show ($"An error occurred: {ex.Message}");
                return false;
            }

        }

        public static bool GetStoredCredential(ref string Username, ref string Password)
        {
            //this will get the stored username and password and will return true if found and false if not found.
            try
            {
                //gets the current project's directory
                string currentDirectory = System.IO.Directory.GetCurrentDirectory();

                // Path for the file that contains the credential.
                string filePath  = currentDirectory + "\\data.txt";

                // Check if the file exists before attempting to read it
                if (File.Exists(filePath))
                {
                    // Create a StreamReader to read from the file
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        // Read data line by line until the end of the file
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            Console.WriteLine(line); // Output each line of data to the console
                            string[] result = line.Split(new string[] { "#//#" }, StringSplitOptions.None);

                            Username = result[0];
                            Password = result[1];
                        }
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show ($"An error occurred: {ex.Message}");
                return false;   
            }

        }
    }
}
