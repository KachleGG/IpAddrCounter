using System;
using System.Net;
using System.Text;

/// <summary>
/// Handles subnet calculations with equal-sized subnets
/// </summary>
public static class SubnetCalculator
{
    public static void DivideIntoSubnets(StringBuilder outputBuilder)
    {
        outputBuilder.AppendLine("\n--- Subnet Calculator ---");
        try
        {
            // Get IP address from user
            Console.Write("Enter network IP address (e.g. 192.168.1.0): ");
            string ipAddressInput = Console.ReadLine() ?? "";
            
            if (!IPAddress.TryParse(ipAddressInput, out IPAddress ipAddress))
            {
                throw new ArgumentException("Invalid IP address format.");
            }

            // Get current subnet mask
            Console.Write("Enter current subnet mask (e.g. 255.255.255.0 or /24): ");
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

            // Verify this is a network address
            IPAddress networkAddress = IpAddressUtility.CalculateNetworkAddress(ipAddress, subnetMask);
            if (!networkAddress.Equals(ipAddress))
            {
                string warning = $"Warning: {ipAddress} is not a network address. Using {networkAddress} instead.";
                outputBuilder.AppendLine(warning);
                Console.WriteLine(warning);
                ipAddress = networkAddress;
            }

            // Get user requirements
            Console.Write("How many subnets do you need? ");
            if (!int.TryParse(Console.ReadLine(), out int numSubnets) || numSubnets <= 0)
            {
                throw new ArgumentException("Number of subnets must be a positive integer.");
            }

            Console.Write("How many hosts per subnet do you need? ");
            if (!int.TryParse(Console.ReadLine(), out int hostsPerSubnet) || hostsPerSubnet <= 0)
            {
                throw new ArgumentException("Number of hosts must be a positive integer.");
            }

            // Calculate required subnet bits
            int requiredSubnetBits = (int)Math.Ceiling(Math.Log2(numSubnets));
            int requiredHostBits = (int)Math.Ceiling(Math.Log2(hostsPerSubnet + 2)); // +2 for network and broadcast addresses

            // Calculate new CIDR prefix
            int newCidrPrefix = 32 - requiredHostBits;
            
            // Check if the requirements can be met
            int availableHostBits = 32 - cidrPrefix;
            if (requiredSubnetBits + requiredHostBits > availableHostBits)
            {
                throw new ArgumentException(
                    $"Cannot accommodate {numSubnets} subnets with {hostsPerSubnet} hosts each. " +
                    $"You need {requiredSubnetBits + requiredHostBits} bits, but only have {availableHostBits} bits available.");
            }

            // Calculate actual values
            int actualSubnets = (int)Math.Pow(2, requiredSubnetBits);
            int actualHostsPerSubnet = (int)Math.Pow(2, requiredHostBits) - 2;

            // Generate and display subnet information
            IPAddress newSubnetMask = IpAddressUtility.CidrToSubnetMask(newCidrPrefix);
            outputBuilder.AppendLine($"\nNew Subnet Mask: {newSubnetMask} (/{newCidrPrefix})");
            outputBuilder.AppendLine($"Number of Subnets: {actualSubnets} (you requested {numSubnets})");
            outputBuilder.AppendLine($"Hosts per Subnet: {actualHostsPerSubnet} (you requested {hostsPerSubnet})");
            
            // Calculate and display individual subnet information
            byte[] baseBytes = ipAddress.GetAddressBytes();
            int subnetSizeInAddresses = (int)Math.Pow(2, 32 - newCidrPrefix);
            
            outputBuilder.AppendLine("\nSubnet Details:");
            outputBuilder.AppendLine("---------------");
            
            for (int i = 0; i < Math.Min(actualSubnets, 100); i++) // Limit to 100 subnets for display
            {
                // Calculate the subnet base address
                byte[] subnetBytes = (byte[])baseBytes.Clone();
                int subnetOffset = i * subnetSizeInAddresses;
                
                // Apply the subnet offset to the base address
                for (int byteIndex = 3; byteIndex >= 0; byteIndex--)
                {
                    int byteValue = subnetBytes[byteIndex] + (subnetOffset & 0xFF);
                    subnetBytes[byteIndex] = (byte)(byteValue & 0xFF);
                    subnetOffset >>= 8;
                }
                
                IPAddress subnetAddress = new IPAddress(subnetBytes);
                IPAddress subnetBroadcast = IpAddressUtility.CalculateBroadcastAddress(subnetAddress, newSubnetMask);
                Tuple<IPAddress, IPAddress> subnetRange = IpAddressUtility.CalculateIPRange(subnetAddress, subnetBroadcast);
                string subnetClass = IpAddressUtility.DetermineNetworkClass(subnetAddress);
                
                outputBuilder.AppendLine($"\nSubnet #{i+1}:");
                outputBuilder.AppendLine($"  Network Address: {subnetAddress}");
                outputBuilder.AppendLine($"  Subnet Mask:     {newSubnetMask} (/{newCidrPrefix})");
                outputBuilder.AppendLine($"  Broadcast:       {subnetBroadcast}");
                outputBuilder.AppendLine($"  IP Range:        {subnetRange.Item1} - {subnetRange.Item2}");
                outputBuilder.AppendLine($"  Hosts:           {actualHostsPerSubnet}");
                outputBuilder.AppendLine($"  Network Class:   {subnetClass}");
            }
            
            if (actualSubnets > 100)
            {
                outputBuilder.AppendLine("\n... (showing only the first 100 subnets)");
            }
            
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