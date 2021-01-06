namespace BipBip.Extensions.Abstractions
{
    public interface IPrivateMessage : IIrcEvent
    {
        public string SenderName { get; set; }
        public string SenderHost { get; set; }
        public string Destination { get; set; }
        public string ContentText { get; set; }
    }
}