



using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;


ServerObject listener = new ServerObject();
 listener.Listen();



class ServerObject
{
    static IPAddress localAddr = IPAddress.Parse("25.28.51.91");
    TcpListener tcpListener = new TcpListener(localAddr, 12345);
    List<ClientObject> clients = new List<ClientObject>();


    protected internal void Listen()
    {
        try
        {
            tcpListener.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
               
                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    clients.Add(clientObject);

                    clientObject.Process();
                if (clients.Count < 2)
                {
                    var msgAwait = "Ожидаем подключение 2-го игрока";
                    Console.WriteLine(msgAwait);
                    continue;
                }

                var player1 = clients[0].Id;
                var player2 = clients[1].Id;




                var msg1 = $"{clients[0].Name} - игрок 1, ходит - ";
                var msgChoise1 = "X";
                var msg2 = $"{clients[0].Name} - игрок 2, ходит - ";
                var msgChoise2 = "O";

                Message(msg1, player1);
                Message(msgChoise1, player1);

                Message(msg2, player2);
                Message(msgChoise2, player2);



                var msgHod = "Введите ход:";

                var msgPlayerHod1 = "Игрок2 ходит -";
                var msgPlayerHod2 = "Игрок1 ходит - ";
               
                while (true)
                {
                    

                    var msgPlayerHod_Char = clients[0].Reader.ReadLine();
                    Message(msgPlayerHod2, player2);
                    Message(msgPlayerHod_Char, player2);


                   

                    msgPlayerHod_Char = clients[1].Reader.ReadLine();
                    Message(msgPlayerHod1, player1);
                    Message(msgPlayerHod_Char, player1);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Disconnect();
        }
    }


    protected internal void Message(string message, string id)
    {
        foreach (var client in clients)
        {
            if (client.Id == id)
            {
                 client.Writer.WriteLine(message);
                
            }
        }
    }
    protected internal void Disconnect()
    {
        foreach (var client in clients)
        {
            client.Close();
        }
        tcpListener.Stop();
    }
}


class ClientObject
{
    protected internal string Id { get; } = Guid.NewGuid().ToString();
    protected internal StreamWriter Writer { get; }
    protected internal StreamReader Reader { get; }

    public string Name = "";

    TcpClient client;
    ServerObject server;

    public ClientObject(TcpClient tcpClient, ServerObject serverObject)
    {
        client = tcpClient;
        server = serverObject;

        var stream = client.GetStream();

        Reader = new StreamReader(stream);

        Writer = new StreamWriter(stream) { AutoFlush = true };
    }

    public  void Process()
    {
        try
        {

            string? userName = Reader.ReadLine();
            Name = userName;
            string? message = $"{userName} вошел в чат";
          
            Console.WriteLine(message);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    protected internal void Close()
    {
        Writer.Close();
        Reader.Close();
        client.Close();
    }
}









































