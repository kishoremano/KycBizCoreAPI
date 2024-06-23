using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Serilog;
using DataAccessLayer.Models;
using KycBizWebApi.Repository.Users;
using Microsoft.AspNetCore.Hosting;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DataAccessLayer.Repository.Common;
using Microsoft.Extensions.Configuration;
using Serilog.Extensions.Hosting;

namespace DataAccessLayer.Helper
{
    public static class Utilities
    {

        /// <summary>
        /// Get the Remote (when running on Server) or Local (when running locally) IP Address of the User
        /// </summary>
        /// <param name="ipAddress">Remote IP Address from the HTTP Context or Request</param>
        /// <returns>Remote or Local IP Address as IPv4 String</returns>
        public static string GetUserIP(IPAddress? ipAddress)
        {
            // Get the Remote IP Address as IPv4 String
            var remoteIP = ipAddress?.MapToIPv4().ToString() ?? string.Empty;
            // Check whether the Remote IP has returned a valid IP address and return it 
            if (!string.IsNullOrWhiteSpace(remoteIP) && !remoteIP.Equals("0.0.0.1")) return remoteIP;
            // Get Local IP address if Remote IP has returned invalid IP address 
            var host = Dns.GetHostEntry(Dns.GetHostName());
            // Loop through the Host Machine IP Addresses
            foreach (var ip in host.AddressList)
            {
                // Return the IP Address if it is part of the internal network
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.MapToIPv4().ToString();
                }
            }

            // Return the default Local IP address if there are nothing else available
            return "127.0.0.1";
        }

        /// <summary>
        /// Get the Machine Name of the User
        /// </summary>
        /// <param name="userIP">IP Address of the User</param>
        /// <returns>Machine Name if it is available, otherwise the string "No_Machine_Name"</returns>
        public static string GetUserMachine(string userIP)
        {
            try
            {
                return Dns.GetHostEntry(userIP).HostName;
            }
            catch
            {
                return "No_Machine_Name";
            }
        }

        public static void SetUserID<T>(HttpRequest request, IDiagnosticContext context, ILogger<T> logger)
        {
            // Get the User Identifier details from Authorization Header (Anonymous if authorization is not provided)
            var userId = request.HttpContext.User.Claims.FirstOrDefault() ?? new Claim("UserID", "Anonymous");
            context.Set("UserId", userId);
            logger.LogInformation("Authorized with User {UserID}", userId);
        }

       

    }
}
