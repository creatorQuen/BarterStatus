using LeadStatusUpdater.Enums;
using System.Collections.Generic;

namespace LeadStatusUpdater.Models
{
    public class LeadOutputModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string BirthDate { get; set; }
        public Role Role { get; set; }
        public List<AccountOutputModel> Accounts { get; set; }
    }
}
