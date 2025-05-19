namespace HealthAndFinance.BL.Dtos
{
    public class UserCreateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserPassword { get; set; }
        public string ProfilePhoto { get; set; }
    }
}