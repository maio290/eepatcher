/*
 * Created by SharpDevelop.
 * User: Mario
 * Date: 12.01.2021
 * Time: 04:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;


namespace EEPatcher
{
	class Program
	{
		
		static string DirServer = "localhost:1337";
		static UInt32 LanBroadcastPort = 8585;
		static UInt32 EEFileTransferPort = 33335;
		static UInt32 LobbyPort = 33336;
		static bool UseLocalIP = false;
		static bool NoPingInProgress = true;
		static bool CDKeyCheck = false;
		static bool CRCCheck = false;
		static bool CheckForInternet = false;
		static bool Public = true;
		static bool AllowDuplicateNames = false;
		static bool AppendNumberToDefaultLanName = true;
		static bool AllowJoinGameWithoutPing = true;
		
		
		public static void printConfig()
		{
			Console.WriteLine("DirServer: "+DirServer);
			Console.WriteLine("LanBroadcastPort: "+LanBroadcastPort);
			Console.WriteLine("EEFileTransferPort: "+EEFileTransferPort);
			Console.WriteLine("LobbyPort: "+LobbyPort);		
			Console.WriteLine("UseLocalIP: "+UseLocalIP);
			Console.WriteLine("CDKeyCheck: "+CDKeyCheck);
			Console.WriteLine("CRCCheck: "+CRCCheck);
			Console.WriteLine("CheckForInternet: "+CheckForInternet);
			Console.WriteLine("Public: "+Public);
			Console.WriteLine("AllowDuplicateNames: "+AllowDuplicateNames);
			Console.WriteLine("AppendNumberToDefaultLanName: "+AppendNumberToDefaultLanName);
			Console.WriteLine("AllowJoinGameWithoutPing: "+AllowJoinGameWithoutPing);
			
		}
		
		public static string boolToString(bool target)
		{
			return target ? "found" : "not found";
		}
		
		public static string replaceValueByKey(string key, object value, string target)
		{
			int idx = target.IndexOf(key);
			
			if(idx == -1)
			{
				Console.WriteLine("Critical: Didn't find the key "+key+" within the config file");
			}
			else
			{
				int valueStart = target.IndexOf(':', idx);
				int valueEnd = target.IndexOf("\n", valueStart);
				
				string val = target.Substring(valueStart,valueEnd-valueStart);
				target = target.Replace(key+val,key+": "+value.ToString().ToLower());
			}
			
			return target;
		}
		
		public static void patchFile(string path)
		{
			Console.WriteLine("Patching file: "+path);
			string lines = File.ReadAllText(path);
			lines = replaceValueByKey("DirServer",DirServer, lines);
			lines = replaceValueByKey("LanBroadcastPort",LanBroadcastPort, lines);
			lines = replaceValueByKey("EEFileTransferPort",EEFileTransferPort, lines);
			lines = replaceValueByKey("LobbyPort",LobbyPort, lines);		
			lines = replaceValueByKey("UseLocalIP",UseLocalIP, lines);
			lines = replaceValueByKey("CDKeyCheck",CDKeyCheck, lines);
			lines = replaceValueByKey("CRCCheck",CRCCheck, lines);
			lines = replaceValueByKey("CheckForInternet",CheckForInternet, lines);
			lines = replaceValueByKey("Public", Public, lines);
			lines = replaceValueByKey("AllowDuplicateNames",AllowDuplicateNames, lines);
			lines = replaceValueByKey("AppendNumberToDefaultLanName",AppendNumberToDefaultLanName, lines);
			lines = replaceValueByKey("AllowJoinGameWithoutPing",AllowJoinGameWithoutPing, lines);
			File.Delete(path);
			File.WriteAllText(path,lines);
			Console.WriteLine("File patched!");
			Console.WriteLine();
		}
		
		public static void Main(string[] args)
		{
			Console.Title = "Empire Earth - GOG Patcher";
			string path = "C:\\GOG Games\\Empire Earth Gold Edition";
			string filename = "WONLobby.cfg";
			

			
			Console.WriteLine("EEPatcher patches the WONLobby.cfg to not require any CD key, resets the ports used and sets other options");
			Console.WriteLine();
			Console.Write("Do you want to continue? (y/n): ");
			var val = Console.ReadLine();

			if(!val.ToLower().Equals("y")) return;
			
			Console.WriteLine("Checking default folder: "+path);
			
			bool isValid = Directory.Exists(path) && Directory.Exists(path+"\\"+"Empire Earth");
			
			while(!isValid)
			{
				Console.Write("Couldn't find the subfolder 'Empire Earth', please enter a new path:");
				path = Console.ReadLine();
				if(String.IsNullOrEmpty(path)) return;
				isValid = Directory.Exists(path) && Directory.Exists(path+"\\"+"Empire Earth");
			}
			
			Console.WriteLine();
			Console.Write("Subfolder Empire Earth found, checking for expansion: ");
			bool hasExpansion = Directory.Exists(path+"\\"+"Empire Earth - The Art of Conquest");
			Console.WriteLine(boolToString(hasExpansion));
			
			string baseGameConfigPath =  path+"\\Empire Earth\\"+filename;
			string expansionConfigPath = path+"\\Empire Earth - The Art of Conquest\\"+filename;
			
			bool baseGameConfigFileExists = File.Exists(baseGameConfigPath);
			bool expansionConfigFileExists = File.Exists(expansionConfigPath);
			
			Console.WriteLine("Checking for config file "+filename+" for base game: "+boolToString(baseGameConfigFileExists));

			if(hasExpansion)
			{
				Console.WriteLine("Checking for config file "+filename+" for expansion: "+boolToString(expansionConfigFileExists));
			}
			
			Console.WriteLine();
			
			Console.Write("Patch base game? (y/n): ");
			bool patchBaseGame = Console.ReadLine().ToLower().Equals("y");
			
			Console.Write("Patch expansion? (y/n): ");
			bool patchExpansion = Console.ReadLine().ToLower().Equals("y");
			
			Console.WriteLine();
			
			Console.WriteLine("+++ Patch Config +++");
			printConfig();
			Console.WriteLine();
			Console.Write("Write changes? (y/n): ");
			bool writeChanges = Console.ReadLine().ToLower().Equals("y");
			
			if(writeChanges)
			{
				Console.WriteLine();
				if(baseGameConfigFileExists)
				{
					if(!File.Exists(baseGameConfigPath+".original")) File.Copy(baseGameConfigPath, baseGameConfigPath+".original");
					patchFile(baseGameConfigPath);
				}
				
				if(expansionConfigFileExists)
				{
					if(!File.Exists(expansionConfigPath+".original")) File.Copy(expansionConfigPath, expansionConfigPath+".original");
					patchFile(expansionConfigPath);
				}
				
			}
			else
			{
				Console.WriteLine("No changes written.");
			}
			
				Console.Write("Press any key to exit...");
				Console.ReadKey(true);			
		}
	}
}