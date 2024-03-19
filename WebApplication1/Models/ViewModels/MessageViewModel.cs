namespace WebApplication1.Models.ViewModels
{
    public class MessageViewModel
    {
        public List<Message> Messages { get; set; }
        public List<ConnectionRequest> AcceptedConnections { get; set; }
    }
}
