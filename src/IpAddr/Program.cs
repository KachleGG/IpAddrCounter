using System;
using System.Net;
using System.Text;
using UpdateMGR;

class Program
{
    static void Main(string[] args)
    {
        Updater updater = new Updater("KachleGG", "IpAddrCounter", "1.0.6", "IpAddr");
        updater.Update();

        Console.WriteLine("IP Address Calculator");
        Console.WriteLine("=====================");
        Console.WriteLine($"current version: {updater.currentVersion}");
        
        bool exitProgram = false;
        bool saveOutput = false;
        StringBuilder outputBuilder = new StringBuilder();
        
        while (!exitProgram)
        {
            Console.WriteLine("\nSelect an option:");
            Console.WriteLine("1. Get Network Info");
            Console.WriteLine("2. Divide Network into Subnets");
            Console.WriteLine("3. Divide Network into Subnets with Variable Hosts");
            Console.WriteLine("4. Exit");
            Console.WriteLine("S. Toggle Save Output to File");
            Console.WriteLine();
            Console.WriteLine($"Save Output: {(saveOutput ? "ON" : "OFF")}");
            Console.WriteLine();
            Console.Write("Your choice (1-4): ");

            string choice = Console.ReadLine() ?? "";

            switch (choice.ToUpper())
            {
                case "1":
                    outputBuilder.Clear();
                    NetworkCalculator.GetNetworkInfo(outputBuilder);
                    if (saveOutput)
                    {
                        SaveOutputToFile(outputBuilder.ToString());
                    }
                    break;
                case "2":
                    outputBuilder.Clear();
                    SubnetCalculator.DivideIntoSubnets(outputBuilder);
                    if (saveOutput)
                    {
                        SaveOutputToFile(outputBuilder.ToString());
                    }
                    break;
                case "3":
                    outputBuilder.Clear();
                    VariableSubnetCalculator.DivideIntoSubnets(outputBuilder);
                    if (saveOutput)
                    {
                        SaveOutputToFile(outputBuilder.ToString());
                    }
                    break;
                case "4":
                    exitProgram = true;
                    break;
                case "S":
                    saveOutput = !saveOutput;
                    Console.WriteLine($"Save Output {(saveOutput ? "enabled" : "disabled")}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
            Console.Clear();
        }
    }

    public static void SaveOutputToFile(string output)
    {
        Console.Clear();
        Console.WriteLine("Name of the output file(without suffix): ");
        string filePath = Console.ReadLine() ?? "";
        filePath += ".txt";
        File.WriteAllText(filePath, output);
        Console.WriteLine($"Output saved to {filePath}");
    }
}
