using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using SlingshotSolution;
using System.Data.SQLite;

namespace Server {

    class Server {
        
        static void Main_(string[] args) {

            Server s = new Server();
            s.TestDatabase();

            //s.setUpConnection();
        }

        public void SetUpConnection() {
            Console.WriteLine("Test");
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 1302);
            listener.Start();

            while (true) {
                try {
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                StreamReader reader = new StreamReader(client.GetStream());
                StreamWriter writer = new StreamWriter(client.GetStream());

                byte[] buffer = new byte[1024];
                stream.Read(buffer, 0, buffer.Length);
                int len = 0;
                foreach (byte b in buffer) {
                    if (b != 0) {
                        len++;
                    }
                }
                string request = Encoding.UTF8.GetString(buffer, 0, len);
                Console.WriteLine("Request: " + request);

                string[] inputCoords = request.Split(", ");

                foreach (string word in inputCoords) {
                    Console.WriteLine(word);
                }

                var x = new SlingshotSolution.Program();
                //string data = x.ServerTest(0, 0, 60, 70);

                //writer.WriteLine(data);
                writer.Flush();

                } catch(Exception e) {
                    Console.WriteLine("Error connecting to server: " + e.StackTrace);
                }
            }
        }

        public void TestDatabase() {
            string cs = "Data Source=:memory:";
            string stm = "SELECT SQLITE_VERSION()";

            using var con = new SQLiteConnection(cs);
            con.Open();

            using var cmd = new SQLiteCommand(stm, con);
            string version = cmd.ExecuteScalar().ToString();

            Console.WriteLine($"SQLite version: {version}");
        }
    }
}