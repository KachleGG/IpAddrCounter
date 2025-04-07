using System;
using System.Net;
using UpdateMGR;

class Program
{
    static void Main(string[] args)
    {
        Updater updater = new Updater("KachleGG", "IpAddrCounter", "1.0.0", "IpAddr");
        updater.Update();


        Console.WriteLine("IP Address Calculator");
        Console.WriteLine("=====================");

        bool exitProgram = false;
        while (!exitProgram)
        {
            Console.WriteLine("\nSelect an option:");
            Console.WriteLine("1. Get Network Info");
            Console.WriteLine("2. Divide Network into Subnets");
            Console.WriteLine("3. Divide Network into Subnets with Variable Hosts");
            Console.WriteLine("4. Exit");
            Console.Write("Your choice (1-4): ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    NetworkCalculator.GetNetworkInfo();
                    break;
                case "2":
                    SubnetCalculator.DivideIntoSubnets();
                    break;
                case "3":
                    VariableSubnetCalculator.DivideIntoSubnets();
                    break;
                case "4":
                    exitProgram = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}
