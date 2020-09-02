﻿using System;
using System.Collections.Generic;
using ServiceConnect;

namespace Filters.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*********** Consumer ***********");
            var bus = Bus.Initialize(config =>
            {
                config.SetHost("localhost");
                config.SetQueueName("Filters.Consumer");
                config.SetNumberOfClients(10);
                config.BeforeConsumingFilters = new List<Type>
                {
                    typeof(BeforeFilter1),
                    typeof(BeforeFilter2)
                };
                config.AfterConsumingFilters = new List<Type>
                {
                    typeof(AfterFilter1),
                    typeof(AfterFilter2)
                };
            });

            bus.StartConsuming();

            Console.ReadLine();
        }
    }
}
