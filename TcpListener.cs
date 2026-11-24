using System.Net;
using System.Net.Sockets;
using System.Text;

// create a new TcpListener server that is going to be listening to tcp requests

IPAddress loopBackAddress = IPAddress.IPv6Loopback;
var listener = new TcpListener(loopBackAddress, 8080);
// start to listen to client requests
listener.Start();

// start a blocking call to accept requests
using TcpClient client = listener.AcceptTcpClient();

// get a streaming object for reading and writing
NetworkStream stream = client.GetStream();

// create a buffer for reading data
byte[] buffer = new byte[4096];
// receive all the data sent by the client
int n = stream.Read(buffer, 0, buffer.Length);

// translate data bytes to ASCII
Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, n));

byte[] response = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\n\r\n");

// SEND BACK A response
stream.Write(response, 0, response.Length);
