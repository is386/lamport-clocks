using System;
using System.IO;
using System.Net;
using Akka.Actor;
using Akka.Cluster;
using Akka.Configuration;
using System.Threading.Tasks;


namespace hw1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string def = File.ReadAllText("akka.hocon");
            // Akka.NET says it replaces the public hostname with the appropriate DNS... but that doesn't really happen. So I've rigged the docker run command to set HOSTNAME to the DNS name on the private network...
            string hostset = def.Replace("BADHOST",
             Environment.GetEnvironmentVariable("HOSTNAME") +
             (Environment.GetEnvironmentVariable("AKKA_SEED_PATH") != null
             ? "." + Environment.GetEnvironmentVariable("AKKA_SEED_PATH")
             : ""));
            var config = ConfigurationFactory.ParseString(hostset);
            var ipAddress = config.GetString("akka.remote.dot-netty.tcp.public-hostname", Dns.GetHostName());
            var port = config.GetInt("akka.remote.dot-netty.tcp.port");
            var selfAddress = $"akka.tcp://demo@{ipAddress}:{port}";
            var sys = ActorSystem.Create("hw4", config);
            Console.WriteLine($"joined at {selfAddress}");

            var seeds = config.GetStringList("akka.cluster.seed-nodes");

            IActorRef procActor = sys.ActorOf(Props.Create<ProcessActor>(seeds, port), "proc");
            Cluster.Get(sys).Subscribe(procActor, ClusterEvent.SubscriptionInitialStateMode.InitialStateAsSnapshot, typeof(ClusterEvent.MemberJoined));

            await sys.WhenTerminated;
        }
    }
}
