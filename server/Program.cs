using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatServer
{
    static TcpListener server = null;
    static List<TcpClient> clients = new List<TcpClient>();

    static void Main()
    {
        Console.WriteLine("Starting server...");
        StartServer();

        while (true)
        {
            Console.WriteLine("Type 'exit' to stop the server.");
            if (Console.ReadLine()?.ToLower() == "exit")
            {
                StopServer();
                break;
            }
        }
    }

    static void StartServer()
    {
        server = new TcpListener(IPAddress.Any, 5000);
        server.Start();
        Console.WriteLine("Server started on port 5000.");

        Thread clientAcceptThread = new Thread(AcceptClients);
        clientAcceptThread.Start();
    }

    static void AcceptClients()
    {
        while (true)
        {
            var client = server.AcceptTcpClient();
            clients.Add(client);
            Console.WriteLine("New client connected!");

            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        try
        {
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");
                BroadcastMessage(message, client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client error: {ex.Message}");
        }
        finally
        {
            clients.Remove(client);
            client.Close();
            Console.WriteLine("Client disconnected.");
        }
    }

    static void BroadcastMessage(string message, TcpClient sender)
    {
        foreach (var client in clients)
        {
            if (client == sender) continue;

            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }

    static void StopServer()
    {
        foreach (var client in clients)
        {
            client.Close();
        }

        clients.Clear();
        server.Stop();
        Console.WriteLine("Server stopped.");
    }
}