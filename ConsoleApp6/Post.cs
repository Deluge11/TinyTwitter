

namespace ConsoleApp6
{
   public class Post
    {
        public int Id { get; private set; }
        public string PostMassage { get; set; }
        public string PosterName { get; private set; }
        public DateTime Date { get; private set; }

        public Post(int id, string msg, string posterName,DateTime date)
        {
            Id = id;
            PostMassage = msg;
            PosterName = posterName;
            Date = date;
        }
    }
}
