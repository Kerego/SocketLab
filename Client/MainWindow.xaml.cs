using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Socket _socket;
		byte[] _buffer = new byte[25000];

		public MainWindow(Socket socket)
		{
			InitializeComponent();
			_socket = socket;
		}

		private void SendClicked(object sender, RoutedEventArgs e)
		{
			if (InputBox.Text.Length == 0)
				return;
			try
			{
				if(InputBox.Text == "%Close")
				{
					_socket.Close();
					textBlock.Inlines.Add(new Run($"Closed") { Foreground = new SolidColorBrush(Colors.Red) });
					textBlock.Inlines.Add(new LineBreak());
					InputBox.Text = string.Empty;
					return;
				}

				//send message
				_socket.Send(Encoding.Default.GetBytes(InputBox.Text));

				//receive response
				_socket.Receive(_buffer);
				var response = Encoding.Default.GetString(_buffer.Where(x => x != 0).ToArray());


				if (InputBox.Text != "%Picsoritdidnthappen")
				{
					//output log
					textBlock.Inlines.Add(new Run($"Me : {InputBox.Text}") { Foreground = new SolidColorBrush(Colors.Green) });
					textBlock.Inlines.Add(new LineBreak());
					textBlock.Inlines.Add(new Run($"Server : {response}") { Foreground = new SolidColorBrush(Colors.Blue) });
					textBlock.Inlines.Add(new LineBreak());
				}
				else
				{
					//output image
					var image = LoadImage(_buffer);
					var ImageControl = new Image();
					ImageControl.Width = 300;
					ImageControl.Height = 300;
					ImageControl.Source = image;

					textBlock.Inlines.Add(new Run($"Me : {InputBox.Text}") { Foreground = new SolidColorBrush(Colors.Green) });
					textBlock.Inlines.Add(new LineBreak());
					var container = new InlineUIContainer(ImageControl);
					textBlock.Inlines.Add(container);
					textBlock.Inlines.Add(new LineBreak());

				}

			}
			catch
			{
				if (!_socket.Connected)
				{
					textBlock.Inlines.Add(new Run("Connection Lost"));
					textBlock.Inlines.Add(new LineBreak());
				}
			}

			//clear buffer and inputbox
			InputBox.Text = string.Empty;
			Array.Clear(_buffer, 0, _buffer.Length);
			//scroll log
			ScrollBar.ScrollToEnd();
		}

		private static BitmapImage LoadImage(byte[] imageData)
		{
			if (imageData == null || imageData.Length == 0) return null;
			var image = new BitmapImage();
			using (var mem = new MemoryStream(imageData))
			{
				mem.Position = 0;
				image.BeginInit();
				image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.UriSource = null;
				image.StreamSource = mem;
				image.EndInit();
			}
			image.Freeze();
			return image;
		}

	}
}
