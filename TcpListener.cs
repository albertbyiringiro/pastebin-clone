using System.Net;
using System.Net.Sockets;
using System.Text;

// create a new TcpListener server that is going to be listening to tcp requests
IPAddress loopBackAddress = IPAddress.Loopback;
var listener = new TcpListener(loopBackAddress, 8080);
// start to listen to client requests
listener.Start();

// start a blocking call to accept requests
using TcpClient client = listener.AcceptTcpClient();

// get a streaming object for reading and writing
using NetworkStream stream = client.GetStream();

using var reader = new StreamReader(stream, Encoding.ASCII, leaveOpen: true);
string? requestLine = reader.ReadLine();

// M1 Design Decision: Repeated request headers are combined with ", " per RFC 9110
// rather than silently overwritten.
var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

while (true)
{
    string? headerLine = reader.ReadLine();
    if (string.IsNullOrEmpty(headerLine))
    {
        break;
    }

    int colonIndex = headerLine.IndexOf(':');
    if (colonIndex > 0)
    {
        string name = headerLine[..colonIndex].Trim();
        string value = headerLine[(colonIndex + 1)..].Trim();

        if (headers.TryGetValue(name, out string? existingValue))
        {
            headers[name] = $"{existingValue}, {value}";
        }
        else
        {
            headers[name] = value;
        }
    }
}

if (!string.IsNullOrEmpty(requestLine))
{
    string[] parts = requestLine.Split(' ');
    if (parts.Length >= 2)
    {
        string method = parts[0];
        string rawTarget = parts[1];

        string path = rawTarget.Split('?')[0];

        Console.WriteLine(method);
        Console.WriteLine(path);
    }
}
int size = headers.TryGetValue("Content-Length", out string? contentLengthValue) ? Convert.ToInt32(contentLengthValue) : 0;

string content = string.Empty;
if (size > 0)
{
    char[] buffer = new char[size];

    int charsReader = reader.Read(buffer, 0, buffer.Length);

    content = new(buffer, 0, charsReader);
}

Console.WriteLine(content);

// Print each stored header pair
foreach (var header in headers)
{
    Console.WriteLine($"{header.Key}: {header.Value}");
}

byte[] response = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nContent-Length: 0\r\n\r\n");

// SEND the 200 OK response
stream.Write(response, 0, response.Length);
