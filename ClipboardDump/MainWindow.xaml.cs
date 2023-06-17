using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ClipboardDump.Properties;
using Common;
using CommonWpf;
using CommonWpf.Classes.UI;

namespace ClipboardDump
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();

			this.StatusLabel.Content = null;

			// check folder specified
			this.FolderBox_OnTextChanged(null, null);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			Settings.Default.Save();
		}

		private void DumpClipboard_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				string folder = FS.GetProjectPath("outputs", DateTime.Now.ToString("yyMMdd HHmmss"));

				this.TryDump(folder);

				this.lastFolder = folder;
			}
			catch (Exception ex)
			{
				ExceptionHandler.Catch(ex);
			}
		}

		private void TryDump(string folder, string prefix = null)
		{
			IDataObject dataObject = Clipboard.GetDataObject();
			if (dataObject == null) return;

			string[] formats = dataObject.GetFormats().ToArray();
			formats.ForEach(f => DumpData(dataObject: dataObject, format: f, folder: folder, prefix : prefix));

			this.StatusLabel.Content = $"Processed formats: {formats.Length} at {DateTime.Now:HH:mm:ss}";
		}

		private static readonly char[] invalidChars = Path.GetInvalidFileNameChars();
		private string lastFolder;
		private static void DumpData(IDataObject dataObject, string format, string folder, string prefix)
		{
			try
			{
				object o;
				if (format == "FileContents")
				{
					return;
					//MemoryStream fms = (MemoryStream)dataObject.GetData("FileGroupDescriptorW");
					//byte[] bytes = fms.ToArray();
					//string str = Encoding.UTF8.GetString(bytes);
					//o = str;
				}
				else
				{
					o = dataObject.GetData(format);
				}

				string ext = ".log";
				byte[] content = null;
				if (o is MemoryStream ms)
				{
					byte[] bytes = ms.ToArray();
					o = $"MemoryStream\r\n{BitConverter.ToString(bytes).Replace("-", " ")}";
					ms.Position = 0;
				}
				else if (o is Bitmap bitmap)
				{
					using (MemoryStream stream = new MemoryStream())
					{
						bitmap.Save(stream, ImageFormat.Png);
						content = stream.ToArray();
						ext = ".png";
					}
				}
				else if (o is Image image)
				{
					using (MemoryStream stream = new MemoryStream())
					{
						image.Save(stream, ImageFormat.Png);
						content = stream.ToArray();
						ext = ".png";
					}
				}
				else if (o is string[] strings)
				{
					o = $"string[]\r\n{string.Join("\r\n", strings)}";
				}

				string filename = (prefix ?? "") + format + ext;
				invalidChars.ForEach(c => filename = filename.Replace(c.ToString(), ((int)c).ToString()));
				string file = FS.Combine(folder, filename);
				FS.EnsureFileFolder(file);

				if (content == null)
				{
					File.WriteAllText(file, o?.ToString() ?? "");
				}
				else
				{
					File.WriteAllBytes(file, content);
				}
			}
			catch (Exception ex)
			{
				Exception exception = new Exception($"Error while process format '{format}'.", ex);

				string errorFileName = $"{prefix ?? ""}errors.log";

				string errorFile = Path.Combine(folder, errorFileName);
				File.AppendAllText(errorFile, $"{exception}\r\n\r\n");

				ExceptionHandler.Catch(exception);
			}
		}

		private void OpenDumpFolder_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				if (this.lastFolder == null)
				{
					FS.OpenInDefaultApp(FS.GetProjectPath("outputs"));
				}
				else
				{
					FS.OpenInDefaultApp(this.lastFolder);
				}
			}
			catch (Exception ex)
			{
				ExceptionHandler.Catch(ex);
			}
		}

		private void FolderBox_OnTextChanged(object sender, RoutedEventArgs e)
		{
			try
			{
				string folder = this.FolderBox.Text;
				this.StackPanelForSpecificFolder.IsEnabled = !string.IsNullOrEmpty(folder) && Directory.Exists(folder);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Catch(ex);
			}
		}

		private void DumpSpecificButton_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				string folder = this.FolderBox.Text;
				if (string.IsNullOrEmpty(folder)) return;
				if (!Directory.Exists(folder)) return;

				int? lastNumber = FindLastNumber(folder);
				int number = lastNumber == null ? 1 : lastNumber.Value + 1;

				string prefix = $"{number:000000}_";

				this.TryDump(folder, prefix);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Catch(ex);
			}
		}

		/// <summary>
		/// Scans all files in folder with such format: NNNNNNN_filename.ext, where NNNNNN - number (zero left aligned),
		/// filename - any file name, ext - any extension.
		/// Parse all NNNNNN and returl last. If no such files exists, returns null.
		/// </summary>
		private static int? FindLastNumber(string folder)
		{
			string[] files = Directory.GetFiles(folder);
			int? lastNumber = null;

			foreach (string file in files)
			{
				string fileName = Path.GetFileName(file);
				string[] parts = fileName.Split('_');

				if (parts.Length == 2)
				{
					string numberString = parts[0];
					if (int.TryParse(numberString, out int number))
					{
						if (lastNumber == null || number > lastNumber)
						{
							lastNumber = number;
						}
					}
				}
			}

			return lastNumber;
		}

		private void OpenSpecificDumpFolder_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				string folder = this.FolderBox.Text;
				if (string.IsNullOrEmpty(folder)) return;
				if (!Directory.Exists(folder)) return;

				FS.OpenInDefaultApp(folder);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Catch(ex);
			}
		}
	}
}
