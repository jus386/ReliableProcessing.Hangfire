using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using Hangfire;
using Microsoft.Owin.Hosting;
using Processing.Events;

namespace ConsoleAppServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = GetAvailablePort(12345, 54321, IPAddress.Loopback);
            var url = string.Format("http://localhost:{0}", port);

            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("OWIN Server started at {0}", url);

                BackgroundJobServerOptions ops = new BackgroundJobServerOptions
                {
                    ServerName = url
                };
                using (var server = new BackgroundJobServer(ops))
                {
                    EventAggregator.JobStatusChanged += EventAggregator_JobStatusChanged;
                    EventAggregator.JobCompleted += EventAggregator_JobCompleted;
                    EventAggregator.JobCountFound += EventAggregator_JobCountFound;

                    Console.WriteLine("Hangfire Server started, dashboard at {0}/hangfire. Press ENTER to exit.", url);
                    Console.ReadLine();
                    server.SendStop();
                }
            }
        }

        private static void EventAggregator_JobCountFound(object sender, JobStatus e)
        {
            Console.WriteLine("Job found {0}", e.Name);
        }

        private static void EventAggregator_JobCompleted(object sender, JobStatus e)
        {
            Console.WriteLine("Job completed {0}", e.Name);
        }

        private static void EventAggregator_JobStatusChanged(object sender, JobStatus e)
        {
            Console.WriteLine("Job status changed {0:0.00}%, {1}", e.Progress, e.Name);
        }

        private static int GetAvailablePort(int rangeStart, int rangeEnd, IPAddress ip, bool includeIdlePorts = false)
        {
            IPGlobalProperties ipProps = IPGlobalProperties.GetIPGlobalProperties();

            // if the ip we want a port on is an 'any' or loopback port we need to exclude all ports that are active on any IP
            Func<IPAddress, bool> isIpAnyOrLoopBack = i => IPAddress.Any.Equals(i) ||
                                                           IPAddress.IPv6Any.Equals(i) ||
                                                           IPAddress.Loopback.Equals(i) ||
                                                           IPAddress.IPv6Loopback.
                                                               Equals(i);
            // get all active ports on specified IP.
            List<ushort> excludedPorts = new List<ushort>();

            // if a port is open on an 'any' or 'loopback' interface then include it in the excludedPorts
            excludedPorts.AddRange(from n in ipProps.GetActiveTcpConnections()
                                   where
                                       n.LocalEndPoint.Port >= rangeStart &&
                                       n.LocalEndPoint.Port <= rangeEnd && (
                                       isIpAnyOrLoopBack(ip) || n.LocalEndPoint.Address.Equals(ip) ||
                                        isIpAnyOrLoopBack(n.LocalEndPoint.Address)) &&
                                        (!includeIdlePorts || n.State != TcpState.TimeWait)
                                   select (ushort)n.LocalEndPoint.Port);

            excludedPorts.AddRange(from n in ipProps.GetActiveTcpListeners()
                                   where n.Port >= rangeStart && n.Port <= rangeEnd && (
                                   isIpAnyOrLoopBack(ip) || n.Address.Equals(ip) || isIpAnyOrLoopBack(n.Address))
                                   select (ushort)n.Port);

            excludedPorts.AddRange(from n in ipProps.GetActiveUdpListeners()
                                   where n.Port >= rangeStart && n.Port <= rangeEnd && (
                                   isIpAnyOrLoopBack(ip) || n.Address.Equals(ip) || isIpAnyOrLoopBack(n.Address))
                                   select (ushort)n.Port);

            excludedPorts.Sort();
            long portNumber = rangeStart;

            for (int port = rangeStart; port <= rangeEnd; port++)
            {
                if (!excludedPorts.Contains((ushort)port) && Interlocked.Read(ref portNumber) < port)
                {
                    Interlocked.Increment(ref portNumber);

                    return port;
                }
            }

            return 0;
        }
    }
}
