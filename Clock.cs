namespace hw4
{
    // Lamport Clock implementation 
    public class Clock
    {
        public int Time { get; set; }

        public void Tick()
        {
            Time += 1;
        }
    }
}
