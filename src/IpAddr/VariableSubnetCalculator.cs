using System;
using System.Net;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Handles subnet calculations with variable-sized subnets
/// </summary>
public static class VariableSubnetCalculator
{
    public static void DivideIntoSubnets(StringBuilder outputBuilder)
    {
        outputBuilder.AppendLine("\n--- Subnet Calculator (Variable Hosts) ---");
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

            // Get number of subnets
            Console.Write("How many subnets do you need? ");
            if (!int.TryParse(Console.ReadLine(), out int numSubnets) || numSubnets <= 0)
            {
                throw new ArgumentException("Number of subnets must be a positive integer.");
            }

            // Get host requirements for each subnet
            List<int> hostsPerSubnet = new List<int>();
            for (int i = 0; i < numSubnets; i++)
            {
                Console.Write($"Enter number of hosts needed for subnet #{i+1}: ");
                if (!int.TryParse(Console.ReadLine(), out int hosts) || hosts <= 0)
                {
                    throw new ArgumentException($"Number of hosts for subnet #{i+1} must be a positive integer.");
                }
                hostsPerSubnet.Add(hosts);
            }

            // Calculate required bits for each subnet
            List<int> requiredHostBits = new List<int>();
            List<int> subnetCidrPrefixes = new List<int>();
            
            foreach (int hosts in hostsPerSubnet)
            {
                int hostBits = (int)Math.Ceiling(Math.Log2(hosts + 2)); // +2 for network and broadcast
                requiredHostBits.Add(hostBits);
                subnetCidrPrefixes.Add(32 - hostBits);
            }

            // Check if we have enough address space
            int totalAddressesNeeded = 0;
            foreach (int hostBits in requiredHostBits)
            {
                totalAddressesNeeded += (int)Math.Pow(2, hostBits);
            }

            int availableAddresses = (int)Math.Pow(2, 32 - cidrPrefix);
            if (totalAddressesNeeded > availableAddresses)
            {
                throw new ArgumentException(
                    $"Not enough address space. You need {totalAddressesNeeded} addresses " +
                    $"but only have {availableAddresses} available.");
            }

            // Sort subnets by size (largest first for optimal allocation)
            List<Tuple<int, int>> subnetIndexAndSize = new List<Tuple<int, int>>();
            for (int i = 0; i < numSubnets; i++)
            {
                subnetIndexAndSize.Add(new Tuple<int, int>(i, requiredHostBits[i]));
            }
            
            // Sort by host bits (largest subnet first)
            subnetIndexAndSize.Sort((a, b) => b.Item2.CompareTo(a.Item2));

            // Allocated subnet information
            List<IPAddress> subnetAddresses = new List<IPAddress>();
            List<IPAddress> subnetMasks = new List<IPAddress>();
            List<int> subnetPrefixes = new List<int>();
            List<IPAddress> broadcastAddresses = new List<IPAddress>();
            List<Tuple<IPAddress, IPAddress>> ipRanges = new List<Tuple<IPAddress, IPAddress>>();
            List<string> networkClasses = new List<string>();
            List<int> actualHostsPerSubnet = new List<int>();

            // Start with the base network address
            byte[] currentBytes = ipAddress.GetAddressBytes();
            
            // Allocate subnets in order from largest to smallest
            for (int i = 0; i < numSubnets; i++)
            {
                int originalIndex = subnetIndexAndSize[i].Item1;
                int hostBits = requiredHostBits[originalIndex];
                int subnetCidr = 32 - hostBits;
                
                // Calculate subnet mask for this subnet
                IPAddress currentSubnetMask = IpAddressUtility.CidrToSubnetMask(subnetCidr);
                
                // Calculate subnet address (should already be aligned)
                IPAddress currentSubnetAddress = new IPAddress((byte[])currentBytes.Clone());
                
                // Calculate broadcast address
                IPAddress currentBroadcast = IpAddressUtility.CalculateBroadcastAddress(currentSubnetAddress, currentSubnetMask);
                
                // Calculate IP range
                Tuple<IPAddress, IPAddress> currentRange = IpAddressUtility.CalculateIPRange(currentSubnetAddress, currentBroadcast);
                
                // Store information based on original order
                while (subnetAddresses.Count <= originalIndex)
                {
                    subnetAddresses.Add(null);
                    subnetMasks.Add(null);
                    subnetPrefixes.Add(0);
                    broadcastAddresses.Add(null);
                    ipRanges.Add(null);
                    networkClasses.Add(null);
                    actualHostsPerSubnet.Add(0);
                }
                
                subnetAddresses[originalIndex] = currentSubnetAddress;
                subnetMasks[originalIndex] = currentSubnetMask;
                subnetPrefixes[originalIndex] = subnetCidr;
                broadcastAddresses[originalIndex] = currentBroadcast;
                ipRanges[originalIndex] = currentRange;
                networkClasses[originalIndex] = IpAddressUtility.DetermineNetworkClass(currentSubnetAddress);
                actualHostsPerSubnet[originalIndex] = (int)Math.Pow(2, hostBits) - 2;
                
                // Move to the next subnet starting address (which is the current broadcast + 1)
                byte[] nextSubnetBytes = currentBroadcast.GetAddressBytes();
                bool carry = true;
                for (int byteIndex = 3; byteIndex >= 0 && carry; byteIndex--)
                {
                    nextSubnetBytes[byteIndex]++;
                    carry = nextSubnetBytes[byteIndex] == 0;
                }
                
                currentBytes = nextSubnetBytes;
            }

            // Display subnet information
            outputBuilder.AppendLine("\nSubnet Details:");
            outputBuilder.AppendLine("---------------");
            
            for (int i = 0; i < numSubnets; i++)
            {
                outputBuilder.AppendLine($"\nSubnet #{i+1}:");
                outputBuilder.AppendLine($"  Requested Hosts: {hostsPerSubnet[i]}");
                outputBuilder.AppendLine($"  Actual Hosts:    {actualHostsPerSubnet[i]}");
                outputBuilder.AppendLine($"  Network Address: {subnetAddresses[i]}");
                outputBuilder.AppendLine($"  Subnet Mask:     {subnetMasks[i]} (/{subnetPrefixes[i]})");
                outputBuilder.AppendLine($"  Broadcast:       {broadcastAddresses[i]}");
                outputBuilder.AppendLine($"  IP Range:        {ipRanges[i].Item1} - {ipRanges[i].Item2}");
                outputBuilder.AppendLine($"  Network Class:   {networkClasses[i]}");
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