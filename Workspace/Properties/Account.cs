using Galaxy_Swapper_v2.Workspace.Hashes;
using Galaxy_Swapper_v2.Workspace.Swapping.Other;
using Galaxy_Swapper_v2.Workspace.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Galaxy_Swapper_v2.Workspace.Properties
{
    public static class Account
    {
        public static readonly string Path = $"{App.Config}\\Account.dat";
        public static bool Valid()
        {
            try
            {
                if (!File.Exists(Path))
                {
                    return false;
                }

                var reader = new Reader(File.ReadAllBytes(Path));

                ulong usernameHash = reader.Read<ulong>();
                int usernameLength = reader.Read<int>();
                byte[] usernameBuffer = reader.ReadBytes(usernameLength);
                string username = Encoding.ASCII.GetString(usernameBuffer);

                if (username != Environment.UserName)
                {
                    Log.Warning("Account username was not as expected and will return invalid");
                    return false;
                }

                ulong expectedUsernameHash = CityHash.Hash(Encoding.ASCII.GetBytes(Environment.UserName));
                if (usernameHash != expectedUsernameHash)
                {
                    Log.Warning("Account username hash was not as expected and will return invalid");
                    return false;
                }

                int daysAmount = reader.Read<int>();

                DateTime dateTime = DateTime.Now;
                for (int i = 0; i < daysAmount; i++)
                {
                    ulong dateHash = reader.Read<ulong>();
                    int dateLength = reader.Read<int>();
                    byte[] dateBuffer = reader.ReadBytes(dateLength);

                    if (dateHash != CityHash.Hash(dateBuffer))
                    {
                        Log.Information("Account date hash was not as expected and will be skipped");
                        continue;
                    }

                    if (Encoding.ASCII.GetString(dateBuffer) == dateTime.ToString("dd/MM/yyyy"))
                    {
                        Log.Information("Found a valid date in account data");
                        return true;
                    }
                }

                Log.Information("Could not find a valid date in account data");
                return false;
            }
            catch (Exception e)
            {
                Log.Fatal("Application caught a unexpected error while checking account data");
                Log.Fatal(e.ToString());
                return false;
            }
        }

        public static bool Create(int days)
        {
            try
            {
                if (File.Exists(Path))
                    File.Delete(Path);

                var writer = new Writer(new byte[60000]);
                byte[] username = Encoding.ASCII.GetBytes(Environment.UserName);

                writer.Write(CityHash.Hash(username));
                writer.Write(username.Length);
                writer.WriteBytes(username);

                writer.Write(days);

                DateTime dateTime = DateTime.Now;
                for (int i = 0; i < days; i++)
                {
                    byte[] date = Encoding.ASCII.GetBytes(dateTime.AddDays(i).ToString("dd/MM/yyyy"));
                    writer.Write(CityHash.Hash(date));
                    writer.Write(date.Length);
                    writer.WriteBytes(date);
                }

                File.WriteAllBytes(Path, writer.ToByteArray(writer.Position));
                Log.Information($"Wrote to: {Path} with {days} days");
                return true;
            }
            catch (Exception e)
            {
                Log.Fatal("Application caught a unexpected error while creating account data");
                Log.Fatal(e.ToString());
                return false;
            }
        }
        
        public static bool Activate(string Activation)
        {
            int days = 999;
            if (!Create(days))
                return false;

            // Message.Display(Languages.Read(Languages.Type.Header, "Info"), string.Format(Languages.Read(Languages.Type.Message, "LoginSuccess"), days));
            Message.Display(Languages.Read(Languages.Type.Header, "Info"), "Cracked by Alejandro");
            return true;
        }
    }
}