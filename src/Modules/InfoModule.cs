using AccountibiliBot.Enums;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace AccountibiliBot;
using Discord.Commands;

 // Interation modules must be public and inherit from an IInterationModuleBase
public class InfoModule : InteractionModuleBase<SocketInteractionContext>
{
    // Dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
    public InteractionService Commands { get; set; }

    private InteractionHandler _handler;

    // Constructor injection is also a valid way to access the dependencies
    public InfoModule(InteractionHandler handler)
    {
        _handler = handler;
    }

    // You can use a number of parameter types in you Slash Command handlers (string, int, double, bool, IUser, IChannel, IMentionable, IRole, Enums) by default. Optionally,
    // you can implement your own TypeConverters to support a wider range of parameter types. For more information, refer to the library documentation.
    // Optional method parameters(parameters with a default value) also will be displayed as optional on Discord.

    // [Summary] lets you customize the name and the description of a parameter
    [SlashCommand("echo", "Repeat the input")]
    public async Task Echo(string echo, [Summary("mention the user")]bool mention = false)
        => await RespondAsync(echo + (mention ? Context.User.Mention : string.Empty));

    [SlashCommand("ping", "Pings the bot and returns its latency.")]
    public async Task GreetUserAsync()
        => await RespondAsync(text: $":ping_pong: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);
    
    // This command will greet target user in the channel this was executed in.
    [UserCommand("greet")]
    public async Task GreetUserAsync(IUser user)
        => await RespondAsync(text: $":wave: {Context.User} said hi to you, <@{user.Id}>!");
    
    // [Group] will create a command group. [SlashCommand]s and [ComponentInteraction]s will be registered with the group prefix
    [Group("test_group")]
    public class GroupExample : InteractionModuleBase<SocketInteractionContext>
    {
        // You can create command choices either by using the [Choice] attribute or by creating an enum. Every enum with 25 or less values will be registered as a multiple
        // choice option
        [SlashCommand("choice_example", "Enums create choices")]
        public async Task ChoiceExample(ExampleEnum input)
            => await RespondAsync(input.ToString());
    }

    // Pins a message in the channel it is in.
    [MessageCommand("pin")]
    public async Task PinMessageAsync(IMessage message)
    {
        // make a safety cast to check if the message is ISystem- or IUserMessage
        if (message is not IUserMessage userMessage)
            await RespondAsync(text: ":x: You cant pin system messages!");

        // if the pins in this channel are equal to or above 50, no more messages can be pinned.
        else if ((await Context.Channel.GetPinnedMessagesAsync()).Count >= 50)
            await RespondAsync(text: ":x: You cant pin any more messages, the max has already been reached in this channel!");

        else
        {
            await userMessage.PinAsync();
            await RespondAsync(":white_check_mark: Successfully pinned message!");
        }
    }
}