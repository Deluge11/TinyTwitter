

using Newtonsoft.Json;

namespace ConsoleApp6
{
  public  class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Password { get; private set; }
        public LinkedList<Post> Posts { get; private set; }
        public HashSet<string> Friends { get; private set; }
        public HashSet<string> FriendRequists { get; private set; }
        public HashSet<int> ChatID { get; private set; }

        public User(int id, string name, string password)
        {
            Id = id;
            Name = name;
            Password = password;
            Posts = new();
            Friends = new();
            FriendRequists = new();
            ChatID = new();
        }

        public void AddPost(Post post)
        {
            Posts.AddFirst(post);
        }
        public void AddFriendRequist(string username)
        {
            FriendRequists.Add(username);
        }
        public void RemoveFriendRequist(string username)
        {
            FriendRequists.Remove(username);
        }
        public void AddFriend(string username)
        {
            Friends.Add(username);
        }
        public void AddChat(int chatID)
        {
            ChatID.Add(chatID);
        }

    }
}
