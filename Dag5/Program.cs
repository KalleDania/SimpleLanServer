using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Xml.Serialization;
using System.IO;

namespace Dag5
{
    class Program
    {

        static void Main(string[] args)
        {
            RunClientServer();
        }

        private static void RunClientServer()
        {
            Console.WriteLine("Do you wanna host or connect to a server? Press 1 to host, 2 to connect.");
            string choise = Console.ReadLine();


            if (choise == "1") // Host
            {
                Server();
            }

            if (choise == "2") // Connect
            {
                Connect();
            }
        }

        private static void Server()
        {
            List<string> highscores = new List<string>();

            TcpListener server = null;
            int port = 13000;
            IPAddress localAddr = IPAddress.Any;

            server = new TcpListener(localAddr, port);
            server.Start();

            //Array til bytes
            Byte[] bytes = new Byte[256];

            //String, som indeholder vores data
            String data = null;


            while (true)
            {
                //Vi skriver en besked så brugeren ved hvad der sker
                Console.Write("Waiting for a connection... ");

                //Vi opretter en TCP client, som accepterer indkommende forbindelser
                //Den venter på en forbindelse her
                TcpClient client = server.AcceptTcpClient();

                //Denne linje eksekveres når vi modtager en forbindelse
                Console.WriteLine("Connected!");

                //Vi resetter vores data
                data = null;

                //Vi får fat på en stream fra vores client
                NetworkStream stream = client.GetStream();

                //En indexer, som bruges til at holde styr på
                //hvor mange bytes vi skal læse fra vores stream
                int i;

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(HighscorePackage));
                HighscorePackage p;

                //Løber vores stream a bytes igennem
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    //Bruges ascii encoding(decoding) til at voresætte bytes til en string
                    data = Encoding.ASCII.GetString(bytes, 0, i);

                    //Skrives resultatet ud til konsollen
                    Console.WriteLine("Received: {0}", data);

                    //Laver det vi modtager om til en string
                    using (TextReader reader = new StringReader(data))
                    {
                        //Deserialiserer det tilbage til et objekt
                        p = (HighscorePackage)xmlSerializer.Deserialize(reader);
                    }
                    Console.WriteLine("p {0}", p.Message);                    highscores.Add(p.Message);

                    Console.WriteLine("Full list:");                    foreach (string value in highscores)
                    {
                        Console.WriteLine(value);
                    }
                }
                //client.Close();
            }
        }

        private static void Connect()
        {
            bool sendingElement;
            bool running = true;

            HighscorePackage p = new HighscorePackage()//Objekt, som vi skal sende
            {
                Message = "Hello",
                Number = 13,
            };

            //Vi skal bruge en port
            //Det er VIGTIGT at server og client ar samme port
            int port = 13000;


            while (running)
            {
                // Den ip der connectes til
                Console.WriteLine("Input target ip:");
                Console.WriteLine("10.131.64.207");
                string targetIP = Console.ReadLine();

                Console.WriteLine("Do you wanna send another package?, y/n");
                string answer = Console.ReadLine();
                if (answer == "y")
                {
                    sendingElement = true;
                }
                else
                {
                    sendingElement = false;
                }

                while (sendingElement)
                {
                    Console.WriteLine("Sending Highscore Package.");

                    //XML serializer til at serialisere pakken
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(HighscorePackage));

                    //Instantierer vores client
                    TcpClient client = new TcpClient(targetIP, port);

                    //Opretter en networkstream
                    NetworkStream stream = client.GetStream();

                    if (stream.CanWrite)//Sender vores serialisered data over en stream
                    {
                        xmlSerializer.Serialize(stream, p);
                    }

                    //Viser brugeren at vores data er sendt
                    Console.WriteLine("Sent: {0}", p);
                   
                    stream.Close();
                    //client.Close();

                    sendingElement = false;
                }
                answer = "";
            }
        }
    }
}


