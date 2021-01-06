namespace BipBip.Extensions.Abstractions
{
    public interface IIrcEvent
    {
         IBipClient BipClient { get; set; }
    }
}