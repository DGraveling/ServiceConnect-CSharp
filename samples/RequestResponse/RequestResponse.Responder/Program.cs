﻿using System;
using System.Collections.Generic;
using ServiceConnect;
using RequestRepsonse.Messages;

namespace RequestResponse.Responder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*********** Responder ***********");

            Bus.Initialize(x =>
            {
                x.SetHost("localhost");
                x.SetQueueName("Responder");
            });
            
            Console.ReadLine();
        }
    }
}
