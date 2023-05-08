namespace texnotest.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserGroupId { get; set; }
        public int UserStateId { get; set; }

        public UserGroup UserGroup { get; set; }
        public UserState UserState { get; set; }
    }





}
