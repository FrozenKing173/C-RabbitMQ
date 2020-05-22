using System;
using ContactDetailsServiceB.BusinessModels;
using System.Threading;
using ContactDetailsServiceB.BusinessInterface;

/*
 * Name: Jeandre van Dyk
 * Date: 7 March 2018
 * Project: WongaContactDetailsServiceA
 * Description: This project will initiate RPC Client service for sending and receiving message.
 *              Note that it does not use all the code, however with integrated testing it makes use of an internal RPC Server.
 *              
 *              I hope you enjoy reading my code and thanks for the challenge on RabbitMQ.
 */

namespace ContactDetailsServiceB
{
    class Program
    {

        private static IServiceBus _serviceBus = null;
        private static readonly object _consoleLocker = new object();

        static void Main(string[] args)
        {            
            InitConsole();

            bool errors = true;
            string rpcClient = "";

            // The following will cater for creating a Client service.
            bool askForClient = false;
            if (askForClient)
            {
                do
                {
                    lock (_consoleLocker)
                    {
                        PrintHeader("Initialize");
                        Console.Write("RPC: Initialize Client? Yes/No: ");
                        rpcClient = Console.ReadLine();
                    }
                    if (!String.IsNullOrEmpty(rpcClient) && !String.IsNullOrWhiteSpace(rpcClient))
                    {
                        if (rpcClient.Length >= 1 && rpcClient.Length <= 3)
                        {
                            if (rpcClient.ToUpper().StartsWith('Y') || rpcClient.ToUpper().StartsWith('N'))
                            {
                                // rpcClient is either yes or no
                                errors = false;
                            }
                        }
                    }

                } while (errors); // Done
            }

            // The following will cater for creating a Server service.
            bool askForServer = true;
            string rpcServer = "";
            if (askForServer)
            {
                
                errors = true;
                do
                {
                    lock (_consoleLocker)
                    {
                        PrintHeader("Initialize");
                        Console.Write("RPC: Initialize internal/local Server? Yes/No: ");
                        rpcServer = Console.ReadLine();
                    }

                    if (!String.IsNullOrEmpty(rpcServer) && !String.IsNullOrWhiteSpace(rpcServer))
                    {
                        if (rpcServer.Length >= 1 && rpcServer.Length <= 3)
                        {
                            if (rpcServer.ToUpper().StartsWith('Y') || rpcServer.ToUpper().StartsWith('N'))
                            {
                                //rpcServer is either yes or no
                                errors = false;
                            }
                        }
                    }
                } while (errors);//Done
            }


            if (rpcServer.ToUpper().StartsWith('Y')) Server(); //Starts Server

            if (rpcClient.ToUpper().StartsWith('Y')) Client(); //Starts Client
            
            Close();

        }
        private static void InitConsole()
        {
            Console.Title = "Wango RabbitMQ Development Form: Service A";          
            Console.ForegroundColor = ConsoleColor.White;
        }
        private static void Server()
        {
            _serviceBus = new BusinessServiceBus(2);
            _serviceBus.OnBusinessChange += new EventHandler(OnBusinessChange);
            Console.ForegroundColor = ConsoleColor.Green;
            PrintHeader(_serviceBus.ToString());
            Thread.Sleep(2000);
            Console.ForegroundColor = ConsoleColor.White;

            bool exit = false;
            do
            {
                PrintHeader("RPC Server running");
                Console.WriteLine("[Press 'Q' to terminate]");
                if (Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    PrintHeader("RPC Server running");
                    Console.WriteLine("[Press 'Y' = yes or 'N' = no]");
                    Console.WriteLine("Are you sure?");
                    if (Console.ReadKey(true).Key == ConsoleKey.Y)
                    {
                        exit = true;
                    }
                }
            } while (!exit);
            GC.Collect();
        }
        private static void Client()
        {
            _serviceBus = new BusinessServiceBus(1);
            _serviceBus.OnBusinessChange += new EventHandler(OnBusinessChange);
            Console.ForegroundColor = ConsoleColor.Green;
            PrintHeader(_serviceBus.ToString());          
            Thread.Sleep(2000);
            Console.ForegroundColor = ConsoleColor.White;
            bool exit = false;

            do
            {
                PrintHeader("RPC Client running");
                lock (_consoleLocker)
                {                   
                    Console.WriteLine("[Press 'Q' to terminate or 'E' for a name]");                  
                }
                ConsoleKeyInfo kInfo = Console.ReadKey();
                if (kInfo.Key == ConsoleKey.Q)
                {
                    PrintHeader("RPC Server running");
                    Console.WriteLine("[Press 'Y' = yes or 'N' = no]");
                    Console.WriteLine("Are you sure?");
                    if (Console.ReadKey(true).Key == ConsoleKey.Y)
                    {
                        exit = true;
                    }
                }
                else if (kInfo.Key == ConsoleKey.E)
                {
                    lock (_consoleLocker)
                    {
                        PrintHeader("RPC Server running");
                        Console.Write("Name: ");
                        _serviceBus.Send(Console.ReadLine());
                    }
                }
            } while (!exit);
        }
            
        private static void Close()
        {
            if (_serviceBus != null) _serviceBus.Close();
            PrintFooter();
        }
        private static string getDateTime()
        {
            return DateTime.Now.ToString("hh:mm");
        }
        private static void PrintHeader(string consoleMsg)
        {
            lock (_consoleLocker)
            {
                Console.Clear();
                Console.WriteLine("############################################ ContactDetails-ServiceBus ########################################## {0}", getDateTime());
                Console.WriteLine("Console: {0}!", consoleMsg);
                Console.WriteLine();
            }


        }
        private static void PrintFooter()
        {
            lock (_consoleLocker)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Clear();
                Console.WriteLine("############################################ ContactDetails-ServiceBus ########################################## {0}", getDateTime());
                Console.WriteLine("Console Closing!");
                Thread.Sleep(1000);
            }
        }
        private static void OnBusinessChange(object sender, EventArgs e)
        {
            lock (_consoleLocker)
            {
                string answer = _serviceBus.Receive();
                Console.WriteLine("-----");
                Console.WriteLine(answer);
            }
        }

    }
}
