using System;


namespace Ryde
{
    public class Rating
    {
        public int Id { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public int Stars { get; set; } // 1-5 stars
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }


        public Rating()
        {
            CreatedAt = DateTime.Now;
        }

    }
}
