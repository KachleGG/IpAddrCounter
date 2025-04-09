using System;
using System.Net;
using System.Text;

/// <summary>
/// Handles basic network information calculations
/// </summary>
public static class NetworkCalculator
{
    public static void GetNetworkInfo(StringBuilder outputBuilder)
    {
        outputBuilder.AppendLine("\n--- Network Information ---");
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
            outputBuilder.AppendLine("\nResults:");
            outputBuilder.AppendLine($"IP Address:       {ipAddress}");
            outputBuilder.AppendLine($"Subnet Mask:      {subnetMask} (/{cidrPrefix})");
            outputBuilder.AppendLine($"Network Address:  {networkAddress}");
            outputBuilder.AppendLine($"Broadcast Address: {broadcastAddress}");
            outputBuilder.AppendLine($"IP Range:         {ipRange.Item1} - {ipRange.Item2}");
            outputBuilder.AppendLine($"Number of Hosts:  {Math.Pow(2, 32 - cidrPrefix) - 2}");
            outputBuilder.AppendLine($"Network Class:    {networkClass}");
            
            // Also write to console
            Console.WriteLine(outputBuilder.ToString());
        }
        catch (Exception ex)
        {
            string errorMessage = $"Error: {ex.Message}";
            outputBuilder.AppendLine(errorMessage);
            Console.WriteLine(errorMessage);
        }
        
        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
} 