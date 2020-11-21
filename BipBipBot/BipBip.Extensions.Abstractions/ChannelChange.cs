namespace BipBip.Extensions.Abstractions
{
    public class ChannelChange
    {
        public Kind ChangeKind { get; set; }
        public string Message { get; set; }
        
        public enum Kind
        {
            MODE,
            TOPIC,
        }
    }
    
    
}