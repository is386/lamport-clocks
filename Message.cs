namespace hw4
{
    // Tells the actors to begin requesting
    public class Begin { }

    // Tells the actors to stop running
    public class End { }

    // Message class that keeps track of the time it was sent and who sent it
    public class Message
    {
        public int Time { get; set; }

        public int Id { get; set; }

        public Message(int time, int id)
        {
            Time = time;
            Id = id;

        }
    }

    // Message type for Requests
    public class Request : Message
    {
        public Request(int time, int id) : base(time, id) { }

    }

    // Message type for Acknowledgements
    public class Ack : Message
    {
        public Ack(int time, int id) : base(time, id) { }

    }

    // Message type for Releases
    public class Release : Message
    {
        public Release(int time, int id) : base(time, id) { }

    }

}
