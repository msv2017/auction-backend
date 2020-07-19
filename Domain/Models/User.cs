using System;

namespace Domain
{
    public class User
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public User(string name, string username, string password)
        {
            this.Id = Guid.NewGuid();
            this.DisplayName = name;
            this.Username = username;
            this.Password = password;
        }
    }
}
