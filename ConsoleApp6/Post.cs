

namespace ConsoleApp6
{
    public class Post
    {
        public int Id { get; private set; }
        public string PostMassage { get; set; }
        public string PosterName { get; private set; }
        public HashSet<string> Likes { get; set; }
        public DateTime Date { get; private set; }

        public Post(int id, string msg, string posterName, DateTime date)
        {
            Id = id;
            PostMassage = msg;
            PosterName = posterName;
            Date = date;
            Likes = new();
        }

        public void Like(string username)
        {
            if (!Likes.Contains(username))
            {
                Likes.Add(username);
            }
            else
            {
                Likes.Remove(username);
            }
        }
     

    }
}
