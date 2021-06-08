using System.Linq;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using Akka.Cluster;
using System;

namespace hw4
{
    public class ProcessActor : UntypedActor
    {
        private ILoggingAdapter log = Context.GetLogger();
        private Clock C = new Clock();
        private int id;
        private bool owned = false;

        // List of every processc
        private List<IActorRef> procs = new List<IActorRef>();

        // List of requests for the resource
        private List<Request> reqs = new List<Request>();

        // Set of procs that sent an acknowledgement. When this set is the same
        // length as procs, we know we have been acknowledged by everyone else
        private HashSet<IActorRef> ackProcs = new HashSet<IActorRef>();

        private List<string> seeds;
        private int Port { get; }

        public ProcessActor(List<string> peers, int port)
        {
            this.id = 0;
            this.seeds = peers;
            this.Port = port;
        }

        protected override void OnReceive(object message)
        {
            bool start = false;
            switch (message)
            {
                case ClusterEvent.CurrentClusterState st:
                    foreach (var m in st.Members)
                    {
                        log.Info($"Heard about prior member joining at {m.Address}");
                    }
                    break;

                // On member join, we select the one that joined and send them a string message
                case ClusterEvent.MemberJoined ev:
                    log.Info($"Heard about member joining at {ev.Member.Address}");
                    var sel = Context.System.ActorSelection(ActorPath.Parse($"{ev.Member.Address}/user/proc"));
                    sel.Tell("join");
                    break;

                // On recieve of a string, the sender is added to the procs list and the current
                // actor is id'd as 1. It tells the other actor to start
                case string identify:
                    procs.Add(Sender);
                    id = 1;
                    start = true;
                    Sender.Tell(new Begin());
                    break;

                // Adds the sender to the procs list and can now start the main proc
                case Begin begin:
                    procs.Add(Sender);
                    start = true;
                    break;
            }
            if (start)
            {
                Become(MainProc);
                Self.Tell(new Begin());
            }
        }

        protected void MainProc(object message)
        {
            switch (message)
            {
                // Process sends request and adds it to the list
                case Begin m:
                    C.Tick();
                    log.Info("ACQUIRING");
                    Request r = new Request(C.Time, id);
                    reqs.Add(r);
                    C.Tick();
                    procs.ForEach(p => p.Tell(r));
                    break;

                // Process adds sender's request and sends acknowledgement
                case Request m:
                    C.Time = Math.Max(C.Time, m.Time) + 1;
                    reqs.Add(m);
                    C.Tick();
                    Sender.Tell(new Ack(C.Time, id));
                    break;

                // Process removes all requests of the sender from the list
                case Release m:
                    C.Time = Math.Max(C.Time, m.Time) + 1;
                    reqs = reqs.Where(r => r.Id != m.Id).ToList();
                    break;

                // Adds sender to set of acknowledgements
                case Ack m:
                    C.Time = Math.Max(C.Time, m.Time) + 1;
                    C.Tick();
                    ackProcs.Add(Sender);
                    break;

                // Terminates the proc; releases the resource if it has it
                case End m:
                    if (owned)
                    {
                        log.Info("RELEASING");
                        reqs = reqs.Where(r => r.Id != id).ToList();
                        ackProcs.Clear();
                        owned = false;
                    }
                    log.Info("TERMINATING");
                    Context.Stop(Self);
                    break;
            }

            // Get resource when all others have acknowledged and own request is earliest
            if ((ackProcs.Count == procs.Count) && IsMinRequest())
            {
                log.Info("OWNED");
                owned = true;

                // Just something to simulate a "critical section"
                int x = 1000000000;
                while (x != 0)
                {
                    x--;
                }

                Release();
                C.Tick();
                Self.Forward(new Begin());
            }
        }

        // Returns true if own request is earliest in the requests list
        private bool IsMinRequest()
        {
            Request min = reqs[0];
            foreach (Request r in reqs)
            {
                if (r.Time < min.Time)
                {
                    min = r;
                }
                else if (r.Time == min.Time)
                {
                    // If equal, we choose the one with the smaller id
                    min = min.Id < r.Id ? min : r;
                }
            }
            return min.Id == id;
        }

        // Sends a release message to all procs and releases the resource
        private void Release()
        {
            log.Info("RELEASING");
            Release r = new Release(C.Time, id);
            C.Tick();
            procs.ForEach(p => p.Tell(r));
            reqs = reqs.Where(r => r.Id != id).ToList();
            ackProcs.Clear();
            owned = false;
        }
    }
}
