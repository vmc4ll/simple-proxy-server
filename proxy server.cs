using System;
using System.Net;
using System.Net.Sockets;

class ProxyServer
{
    static void Main()
    {
        Console.Write("Введите IP-адрес первого сервера: ");
        string firstServerIP = Console.ReadLine();
        
        Console.Write("Введите IP-адрес второго сервера: ");
        string secondServerIP = Console.ReadLine();
        
        TcpListener listener = new TcpListener(IPAddress.Any, 8080);
        listener.Start();
        
        Console.WriteLine("Прокси-сервер запущен. Ожидание подключений...");
        
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            
            Console.WriteLine("Подключен клиент. Установка соединения...");
            
            NetworkStream clientStream = client.GetStream();
            
            TcpClient server = new TcpClient();
            
            try
            {
                server.Connect(firstServerIP, 80);
            }
            catch
            {
                Console.WriteLine("Ошибка подключения к первому серверу.");
                client.Close();
                continue;
            }
            
            NetworkStream serverStream = server.GetStream();
            
            byte[] buffer = new byte[4096];
            int bytesRead;
            
            while ((bytesRead = clientStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                serverStream.Write(buffer, 0, bytesRead);
                serverStream.Flush();
                
                byte[] serverResponseBuffer = new byte[4096];
                int serverResponseBytesRead = serverStream.Read(serverResponseBuffer, 0, serverResponseBuffer.Length);
                
                clientStream.Write(serverResponseBuffer, 0, serverResponseBytesRead);
                clientStream.Flush();
            }
            
            client.Close();
            server.Close();
            
            Console.WriteLine("Соединение закрыто.");
        }
    }
}
