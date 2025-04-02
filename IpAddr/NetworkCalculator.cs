using System;
using System.Net;

/// <summary>
/// Handles basic network information calculations
/// </summary>
public static class NetworkCalculator
{
    public static void GetNetworkInfo()
    {
        Console.WriteLine("\n--- Network Information ---");
        try
        {
            // Get IP address from user
            Console.Write("Enter IP address (e.g. 192.168.1.1): ");
            string ipAddressInput = Console.ReadLine() ?? "";
            
            if (!IPAddress.TryParse(ipAddressInput, out IPAddress ipAddress))
            {
                throw new ArgumentException("Invalid IP address format.");
            }

            // Get subnet mask from user (can be in CIDR or decimal format)
            Console.Write("Enter subnet mask (e.g. 255.255.255.0 or /24): ");
            string subnetInput = Console.ReadLine() ?? "";
            
            int cidrPrefix = 0;
            IPAddress subnetMask;
            
            if (subnetInput.StartsWith("/"))
            {
                if (!int.TryParse(subnetInput.Substring(1), out cidrPrefix) || cidrPrefix < 0 || cidrPrefix > 32)
                {
                    throw new ArgumentException("Invalid CIDR notation. Must be between /0 and /32.");
                }
                subnetMask = IpAddressUtility.CidrToSubnetMask(cidrPrefix);
            }
            else
            {
                if (!IPAddress.TryParse(subnetInput, out subnetMask))
                {
                    throw new ArgumentException("Invalid subnet mask format.");
                }
                cidrPrefix = IpAddressUtility.GetCidrPrefixFromSubnetMask(subnetMask);
            }

            // Calculate network information
            IPAddress networkAddress = IpAddressUtility.CalculateNetworkAddress(ipAddress, subnetMask);
            IPAddress broadcastAddress = IpAddressUtility.CalculateBroadcastAddress(networkAddress, subnetMask);
            Tuple<IPAddress, IPAddress> ipRange = IpAddressUtility.CalculateIPRange(networkAddress, broadcastAddress);
            string networkClass = IpAddressUtility.DetermineNetworkClass(ipAddress);
            
            // Display results
            Console.WriteLine("\nResults:");
            Console.WriteLine($"IP Address:       {ipAddress}");
            Console.WriteLine($"Subnet Mask:      {subnetMask} (/{cidrPrefix})");
            Console.WriteLine($"Network Address:  {networkAddress}");
            Console.WriteLine($"Broadcast Address: {broadcastAddress}");
            Console.WriteLine($"IP Range:         {ipRange.Item1} - {ipRange.Item2}");
            Console.WriteLine($"Number of Hosts:  {Math.Pow(2, 32 - cidrPrefix) - 2}");
            Console.WriteLine($"Network Class:    {networkClass}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        
        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
} 