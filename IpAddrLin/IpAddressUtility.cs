using System;
using System.Net;

/// <summary>
/// Utility class for IP address operations
/// </summary>
public static class IpAddressUtility
{
    public static IPAddress CidrToSubnetMask(int cidrPrefix)
    {
        uint mask = 0xFFFFFFFF << (32 - cidrPrefix);
        byte[] bytes = new byte[4];
        bytes[0] = (byte)(mask >> 24);
        bytes[1] = (byte)(mask >> 16);
        bytes[2] = (byte)(mask >> 8);
        bytes[3] = (byte)(mask);
        
        return new IPAddress(bytes);
    }

    public static int GetCidrPrefixFromSubnetMask(IPAddress subnetMask)
    {
        byte[] bytes = subnetMask.GetAddressBytes();
        int count = 0;
        
        foreach (byte b in bytes)
        {
            count += CountBits(b);
        }
        
        return count;
    }

    public static int CountBits(byte b)
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            if ((b & (1 << (7 - i))) != 0)
            {
                count++;
            }
        }
        return count;
    }

    public static IPAddress CalculateNetworkAddress(IPAddress ipAddress, IPAddress subnetMask)
    {
        byte[] ipBytes = ipAddress.GetAddressBytes();
        byte[] maskBytes = subnetMask.GetAddressBytes();
        byte[] networkBytes = new byte[4];
        
        for (int i = 0; i < 4; i++)
        {
            networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
        }
        
        return new IPAddress(networkBytes);
    }

    public static IPAddress CalculateBroadcastAddress(IPAddress networkAddress, IPAddress subnetMask)
    {
        byte[] networkBytes = networkAddress.GetAddressBytes();
        byte[] maskBytes = subnetMask.GetAddressBytes();
        byte[] broadcastBytes = new byte[4];
        
        for (int i = 0; i < 4; i++)
        {
            broadcastBytes[i] = (byte)(networkBytes[i] | ~maskBytes[i]);
        }
        
        return new IPAddress(broadcastBytes);
    }

    public static Tuple<IPAddress, IPAddress> CalculateIPRange(IPAddress networkAddress, IPAddress broadcastAddress)
    {
        byte[] networkBytes = networkAddress.GetAddressBytes();
        byte[] broadcastBytes = broadcastAddress.GetAddressBytes();
        
        // First usable IP is network address + 1
        if (networkBytes[3] < 255)
        {
            networkBytes[3]++;
        }
        else if (networkBytes[2] < 255)
        {
            networkBytes[2]++;
            networkBytes[3] = 0;
        }
        else if (networkBytes[1] < 255)
        {
            networkBytes[1]++;
            networkBytes[2] = 0;
            networkBytes[3] = 0;
        }
        else if (networkBytes[0] < 255)
        {
            networkBytes[0]++;
            networkBytes[1] = 0;
            networkBytes[2] = 0;
            networkBytes[3] = 0;
        }
        
        // Last usable IP is broadcast address - 1
        if (broadcastBytes[3] > 0)
        {
            broadcastBytes[3]--;
        }
        else if (broadcastBytes[2] > 0)
        {
            broadcastBytes[2]--;
            broadcastBytes[3] = 255;
        }
        else if (broadcastBytes[1] > 0)
        {
            broadcastBytes[1]--;
            broadcastBytes[2] = 255;
            broadcastBytes[3] = 255;
        }
        else if (broadcastBytes[0] > 0)
        {
            broadcastBytes[0]--;
            broadcastBytes[1] = 255;
            broadcastBytes[2] = 255;
            broadcastBytes[3] = 255;
        }
        
        return new Tuple<IPAddress, IPAddress>(
            new IPAddress(networkBytes),
            new IPAddress(broadcastBytes)
        );
    }

    public static string DetermineNetworkClass(IPAddress ipAddress)
    {
        byte firstOctet = ipAddress.GetAddressBytes()[0];
        
        if (firstOctet >= 1 && firstOctet <= 126)
            return "Class A (1.0.0.0 to 126.255.255.255)";
        else if (firstOctet >= 128 && firstOctet <= 191)
            return "Class B (128.0.0.0 to 191.255.255.255)";
        else if (firstOctet >= 192 && firstOctet <= 223)
            return "Class C (192.0.0.0 to 223.255.255.255)";
        else if (firstOctet >= 224 && firstOctet <= 239)
            return "Class D (224.0.0.0 to 239.255.255.255) - Multicast";
        else if (firstOctet >= 240 && firstOctet <= 255)
            return "Class E (240.0.0.0 to 255.255.255.255) - Reserved";
        else
            return "Special address or reserved";
    }
} 