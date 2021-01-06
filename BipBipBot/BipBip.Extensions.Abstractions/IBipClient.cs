using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace BipBip.Extensions.Abstractions
{
    public interface IBipClient
    {
        Task ConnectAsync();
        Task DisconnectAsync();

        Task SendMessageAsync(string destination, string message);
        Task ModUserAsyncOnChannelAsync(string mode, string userName, string channel);

        Task KickAsync(string userName, string channel);
        
        Subject<IPrivateMessage> OnPrivateMessage { get; set; }
        Subject<IIrcEvent> OnIrcEvent { get; set; }
        
    }
}