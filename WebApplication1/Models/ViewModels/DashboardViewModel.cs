namespace WebApplication1.Models.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalFeesAmount { get; set; }
        public int NewMembersCount { get; set; }
        public int ActiveAdmissionMembersCount { get; set; }



        // Additional Variable
        public List<EntryLog> EntryLogs { get; set; }
    }
}
