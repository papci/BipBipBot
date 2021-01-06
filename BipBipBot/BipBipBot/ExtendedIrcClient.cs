using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using BipBip.Extensions.Abstractions;
using BipBipBot.DataEngine;
using IrcDotNet;

namespace BipBipBot
{
    public sealed class ExtendedIrcClient : StandardIrcClient

    {
    public ServerConfiguration ServerConfiguration { get; private set; }
    public Subject<IrcEvent> OnIrcEvent { get; private set; }
    public Subject<PrivateMessageEvent> OnPrivateMessage { get; private set; }

    public List<IBipExtension> Extensions { get; private set; }


    public ExtendedIrcClient(ServerConfiguration serverConfiguration)
    {
        ServerConfiguration = serverConfiguration;
        this.OnIrcEvent = new Subject<IrcEvent>();
        this.OnPrivateMessage = new Subject<PrivateMessageEvent>();
        this.Extensions = new List<IBipExtension>();
        LoadExtensions();
    }

    private void LoadExtensions()
    {
        if ((ServerConfiguration.Extensions?.Count).GetValueOrDefault(0) < 1)
            return;
        foreach (string extensionPath in ServerConfiguration.Extensions)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), extensionPath);
            Assembly assembly = Assembly.LoadFile(fullPath);
        }
    }

    public Task ConnectAsync()
    {
        IrcUserRegistrationInfo registrationInfo = new IrcUserRegistrationInfo();
        registrationInfo.RealName = this.ServerConfiguration.BotName;
        registrationInfo.UserName = ServerConfiguration.BotName;
        registrationInfo.NickName = ServerConfiguration.BotName;
        RawMessageReceived += OnRawMessageReceived;
        base.Connect(ServerConfiguration.GetServerEndpoint(), false, registrationInfo);
        return Task.CompletedTask;
    }

    public Task DisconnectAsync()
    {
        return Task.Run(() => { base.Disconnect(); });
    }

    private void OnRawMessageReceived(object? sender, IrcRawMessageEventArgs e)
    {
        Task.Run(async () => await ResponseHandler.HandleAsync(e.RawContent, this));
    }


    public void JoinChannels()
    {
        this.SendMessageJoin(ServerConfiguration.ChannelConfigurations.Select(x => x.ChannelName));
    }

    public Task SendMessageAsync(string destination, string message)
    {
        return Task.Run(() => { base.SendMessagePrivateMessage(new[] {destination}, message); });
    }
    }
}