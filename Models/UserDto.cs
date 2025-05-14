namespace infoX.api.Models
{
    public class UserDto
    {
        public int ID { get; set; }
        public string? Company_Id { get; set; }
        public string? Title { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? MaidenName { get; set; }
        public string? KnownName { get; set; }
        public string? Email { get; set; }
        public string? IdNumber { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Updated_At { get; set; }
        public int? User_Role { get; set; }
        public string? Phone_Number { get; set; }
    }
}
