using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatClient
{
    static TcpClient client = null;
    static NetworkStream stream = null;

    static void Main()
    {
        Console.WriteLine("Connecting to server...");
        ConnectToServer();

        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();

        Console.WriteLine("You can now start chatting! Type 'exit' to disconnect.");
        while (true)
        {
            string message = Console.ReadLine();
            if (message.ToLower() == "exit")
            {
                Disconnect();
                break;
            }

            SendMessage(message);
        }
    }

    static void ConnectToServer()
    {
        client = new TcpClient("127.0.0.1", 5000);
        stream = client.GetStream();
        Console.WriteLine("Connected to server.");
    }

    static void SendMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    static void ReceiveMessages()
    {
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"\n{message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection error: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Disconnected from server.");
            Disconnect();
        }
    }

    static void Disconnect()
    {
        stream?.Close();
        client?.Close();
        Console.WriteLine("Client disconnected.");
    }
}