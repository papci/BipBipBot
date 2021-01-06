using System;
using System.Threading.Tasks;

namespace BipBip.Extensions.Abstractions
{
  public interface IBipExtension
  {


      /// <summary>
      /// Triggered when a user join a channel
      /// </summary>
      /// <param name="channel"></param>
      /// <param name="userName"></param>
      /// <returns></returns>
      Task OnChannelJoinedAsync(string channel, string userName);

      /// <summary>
      /// Triggered when a user part a channel
      /// </summary>
      /// <param name="channel"></param>
      /// <param name="userName"></param>
      /// <returns></returns>
      Task OnChannelPartedAsync(string channel, string userName);

      /// <summary>
      /// Triggered when a message is received
      /// </summary>
      /// <param name="channel"></param>
      /// <param name="user"></param>
      /// <param name="message"></param>
      /// <returns></returns>
      Task OnMessageReceivedAsync(string channel, string user, string message);

      
      /// <summary>
      /// Triggered when something is changed is the channel configuration
      /// </summary>
      /// <param name="channel"></param>
      /// <returns></returns>
      Task OnChannelModdedAsync(string channel, ChannelChange change);

      /// <summary>
      /// Triggered when bot is connected and ready 
      /// </summary>
      /// <returns></returns>
      Task OnConnectedAsync();

      /// <summary>
      /// Triggered when bot is disconnected from the server
      /// </summary>
      /// <returns></returns>
      Task OnDisconnectedAsync();
  }
}