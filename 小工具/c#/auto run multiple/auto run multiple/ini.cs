using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

// https://www.delftstack.com/zh-tw/howto/csharp/csharp-ini-file/
namespace hahahalib
{

	public class ReadWriteINIfile
	{
		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string name, string key, string val, string filePath);
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

		public string path;

		public ReadWriteINIfile(string inipath)
		{
			path = inipath;
		}
		public void WriteINI(string name, string key, string value)
		{
			WritePrivateProfileString(name, key, value, this.path);
		}
		public string ReadINI(string name, string key)
		{
			/*
			StringBuilder sb = new StringBuilder(255);
			int ini = GetPrivateProfileString(name, key, "", sb, 255, this.path);
			*/
			StringBuilder sb = new StringBuilder(1024);
			int ini = GetPrivateProfileString(name, key, "", sb, 1024, this.path);
			return sb.ToString();
		}
	}
}