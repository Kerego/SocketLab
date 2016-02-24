using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
	/// <summary>
	/// Interaction logic for ConnectWindow.xaml
	/// </summary>
	public partial class ConnectWindow : Window
	{
		public ConnectWindow()
		{
			InitializeComponent();
		}


		private void button_Click(object sender, RoutedEventArgs e)
		{
			byte[] ip = null;
			int port = 0;
			try
			{
				ip = IpAddressBox.Text.Split('.').Select(x => byte.Parse(x)).ToArray();
				port = Int32.Parse(PortBox.Text);
				var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				var endPoint = new IPEndPoint(new IPAddress(ip), port);
				socket.Connect(endPoint);
				MainWindow window = new MainWindow(socket);
				window.Show();
				this.Close();
			}
			catch (Exception args)
			{
				MessageBox.Show(args.Message);
				return;
			}

		}
	}
}
