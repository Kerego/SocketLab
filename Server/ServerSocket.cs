using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	class ServerSocket
	{
		static Socket _socket;
		static DateTime _openTime;
		static void Main(string[] args)
		{
			//Endpoint parsing
			if (args.Length < 2)
				return;
			byte[] ip = null;
			int port = 0;
			ParseEndPoint(args, ref ip, ref port);

			if (ip == null || port == 0)
				return;

			//create and bind socket 
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			var endPoint = new IPEndPoint(new IPAddress(ip), port);

			_socket.Bind(endPoint);
			_socket.Listen(10);
			_openTime = DateTime.Now;
			Console.WriteLine("Server Started!");


			Hastalavista += _socket.Close;
			Hastalavista += () => Console.WriteLine("Server closed");
			//accept 10 users
			for (int i = 1; i < 11; i++)
			{
				try
				{
					var client = _socket.Accept();
					//i may change before task is started
					var t = i;
					var task = new Task(() => ProcessClient(client, t));
					task.Start();
					Hastalavista += client.Close;
					Console.WriteLine($"Client {i} connected, Address: {client.RemoteEndPoint.ToString()}");
				}
				catch
				{
					//accept aborted
				}
			}

			Console.Read();
		}

		private static void ParseEndPoint(string[] args, ref byte[] ip, ref int port)
		{
			try
			{
				ip = args[0].Split('.').Select(x => byte.Parse(x)).ToArray();
				port = Int32.Parse(args[1]);
			}
			catch
			{
				Console.WriteLine("Incorrect Ip or Port");
				Console.Read();
			}
		}

		delegate void Terminate();
		static event Terminate Hastalavista;

		static void TriggerHastalavista()
		{
			if(Hastalavista != null)
				Hastalavista();
		}

		
		static void ProcessClient(Socket clientSocket, int clientId)
		{
			var buffer = new byte[1000];
			while(clientSocket.Connected)
			{
				try
				{
					clientSocket.Receive(buffer);
				}
				catch
				{
					Console.WriteLine($"Client {clientId} disconnected");
					break;
				}
				//remove unused space
				var message = Encoding.Default.GetString(buffer.Where(x => x != 0).ToArray());
				byte[] response = null;

				//act based on message
				if (message == "%Time")
					response = Encoding.Default.GetBytes($"Time on server {DateTime.Now.ToString("T")}");
				else if (message == "%Host")
					response = Encoding.Default.GetBytes("Bejenari Marian!");
				else if (message == "%Knock, knock")
					response = Encoding.Default.GetBytes($"Knock, knock!  \r\nWho’s there ? \r\nThermos. \r\nThermos who ? \r\nThermos be a better knock - knock joke than this. ");
				else if (message.EndsWith("?"))
					response = Encoding.Default.GetBytes($"42");
				else if (message == "%Picsoritdidnthappen")
					response = File.ReadAllBytes("terminator2.jpg");
				else if (message == "%Uptime")
					response = Encoding.Default.GetBytes(String.Format("Uptime {0:%h} hours {0:%m} minutes {0:%s} seconds", DateTime.Now - _openTime));
				else if (message == "%Hastalavista")
				{
					clientSocket.Send(Encoding.Default.GetBytes($"Server shut itself down"));
					Console.WriteLine($"Client {clientId} disconnected");
					TriggerHastalavista();
					break;
				}
				else
					response = Encoding.Default.GetBytes("Can you elaborate on that?");

				clientSocket.Send(response);

				//clear buffer
				Console.WriteLine($"Client {clientId} : " + message);
				Array.Clear(buffer, 0, buffer.Length);

			}
		}

	}
}
