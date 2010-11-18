using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kwwika.Common.Logging;
using Kwwika.QueueComponents;
using Kwwika.Common.Logging.NLog;
using Kwwika.Library.QueueComponents;

namespace KwwikaUpdateServiceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            KwwikaServiceConsole console = new KwwikaServiceConsole();

            Console.WriteLine("\"START\" to start the service\n" +
                "\"STOP\" to stop the service.\n" +
                "\"SEND\" to send a message. You should then enter a message in the format \"<TOPIC> fieldName1=fieldValue1,fieldName2=FieldValue2,...,fieldNameX=fieldValueX\".\n" +
                //"\"READ\" read the next message from the queue.\n" +
                "\"QUIT\" to stop the service and quit the application.");

            string action = "";
            while (action != "QUIT")
            {                
                action = Console.ReadLine().ToUpper();
                switch (action)
                {
                    case "START":
                        console.Start();
                        break;
                    case "SEND":
                        Console.WriteLine("Enter your message to send");
                        string message = Console.ReadLine();
                        try
                        {
                            console.Send(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error parsing message!" + ex.ToString());
                        }
                        break;
                    case "STOP":
                    case "QUIT":
                        console.Stop();
                        break;
                    default:
                        Console.WriteLine(action + " is an unknown command!");
                        break;
                }
            }

            Environment.Exit(0);
        }
    }

    class KwwikaServiceConsole
    {
        ILoggingService _logging;
        MessageQueueReader _reader;
        MessageQueueWriter _writer;

        bool _started = false;

        public KwwikaServiceConsole()
        {
            _logging = new LoggingService("KwwikaService");
            _writer = new MessageQueueWriter(Config.PublishQueue, typeof(PublishMessage), _logging);
        }

        public void Start()
        {
            if (!_started)
            {
                _started = true;
                Console.WriteLine("starting...");
                _reader =
                    new MessageQueueReader(Config.PublishQueue,
                        new MessageConsumer(Config.KwwikaApiKey, Config.KwwikaDomain, Config.PublishQueue, _logging),
                        typeof(PublishMessage),
                        _logging);
                _reader.Start();
            }
            else
            {
                Console.WriteLine("not in a stopped state!");
            }
        }

        public void Stop()
        {
            if (_started)
            {
                _started = false;
                Console.WriteLine("stopping...");
                _reader.Stop();
            }
            else
            {
                Console.WriteLine("not in a started state!");
            }
        }

        internal void Send(string message)
        {
            string[] parts = message.Split(' ');
            string topicName = parts[0];
            string[] fields = parts[1].Split(',');

            PublishMessage publish = new PublishMessage(topicName);
            foreach (string nameValue in fields)
            {
                string[] nameAndValue = nameValue.Split('=');
                publish.Values.Add(nameAndValue[0], nameAndValue[1]);
            }
            _writer.Write(publish);
        }
    }
}
