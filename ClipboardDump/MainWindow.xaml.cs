using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
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
		}

		private void DumpClipboard_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				IDataObject dataObject = Clipboard.GetDataObject();
				if (dataObject == null) return;

				string[] formats = dataObject.GetFormats().ToArray();

				string folder = FS.GetProjectPath("outputs", DateTime.Now.ToString("yyMMdd HHmmss"));
				this.lastFolder = folder;
				formats.ForEach(f => DumpData(dataObject: dataObject, format: f, folder: folder));

				UIHelper.ShowMessage($"Processed formats: {formats.Length}");
			}
			catch (Exception ex)
			{
				ExceptionHandler.Catch(ex);
			}
		}

		private static readonly char[] invalidChars = Path.GetInvalidFileNameChars();
		private string lastFolder;
		private static void DumpData(IDataObject dataObject, string format, string folder)
		{
			try
			{
				object o = dataObject.GetData(format);
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

				string filename = format + ext;
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

				string errorFile = Path.Combine(folder, "errors.log");
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
	}
}
