using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace simpleTCPIP
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// simple
			Server server = new Server (35000, "127.0.0.1");
			Client client = new Client (35000, "127.0.0.1");
			server.initListener();
			client.connect();
			client.sendData("200");
			Console.WriteLine(server.getData());
			client.disConnect();
			server.closeListener();
		}
	}

	class Server{
		IPAddress ipAddress = null;
		int port = 0;
		Socket listener = null;
		Socket socket = null;

		public Server(int _port, string _ipString){
			port = _port;
			ipAddress = IPAddress.Parse(_ipString);
		}

		public async void initListener() {
			listener = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listener.Bind(new IPEndPoint(ipAddress, port));
			listener.Listen (1);
			Console.WriteLine("server:--Start Listen--");

			await Task.Run( () => {
				socket = listener.Accept();
				Console.WriteLine("server:--Connected Client--");
			} );
		}

		public string getData(){
			if (socket != null) {					
				byte[] buf = new byte[500];
				int bufLen = socket.Receive (buf, buf.Length, SocketFlags.None);
				if (bufLen > 0) {
					return System.Text.Encoding.UTF8.GetString (buf);
				} else {
					return null;
				}
			} else {
				Console.WriteLine("server:--Socket is not found--");
				return null;
			}
		}

		public void closeListener(){
			if (listener != null) {
				listener.Close();
				listener = null;
				Console.WriteLine("server:--Close Listen--");
			}
		}
	}

	class Client{
		IPAddress ipAddress = null;
		int port = 0;
		Socket socket = null;

		public Client(int _port,string _ipString){
			port = _port;
			ipAddress = IPAddress.Parse(_ipString);
		}

		public void connect(){
			socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.NoDelay = true;
			socket.Connect(ipAddress, port);
		}

		public void sendData(string d){
			if (socket != null) {					
				byte[] buf = System.Text.Encoding.UTF8.GetBytes (d);
				socket.Send (buf, buf.Length, SocketFlags.None);
			}
		}

		public void disConnect(){
			if (socket != null) {
				socket.Shutdown (SocketShutdown.Both);
				socket.Close ();
				Console.WriteLine("client:--Close Socket--");
			}
		}
	}

}
