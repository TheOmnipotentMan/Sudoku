namespace Sudoku
{
    class JsonItem
    {
        public string StorageName { get; set; }
        public string Source { get; set; }
        public string Category { get; set; }
        public string ItemName { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsBookmarked { get; set; }
        public int Rating { get; set; }

        public string Grid { get; set; }


        public JsonItem()
        {
            StorageName = "DefaultStorage";
            Source = "";
            Category = "DefaultCategory";
            ItemName = "";
            IsCompleted = false;
            IsBookmarked = false;
            Rating = 0;
            Grid =
                "012"+"345"+"678"+
                "123"+"456"+"787"+
                "234"+"567"+"876"+
                "345"+"678"+"765"+
                "456"+"787"+"654"+
                "567"+"876"+"543"+
                "678"+"765"+"432"+
                "787"+"654"+"321"+
                "876"+"543"+"210";
        }

        public JsonItem(string storageName, string category, string grid)
        {
            StorageName = storageName;
            Category = category;
            Grid = grid;

            Source = "";
            ItemName = "";
            IsCompleted = false;
            IsBookmarked = false;
            Rating = 0;
        }

        public JsonItem(string storageName, string source, string category, string itemName, bool isCompleted, bool isBookmarked, int rating, string grid)
        {
            StorageName = storageName;
            Source = source;
            Category = category;
            ItemName = itemName;
            IsCompleted = isCompleted;
            IsBookmarked = isBookmarked;
            Rating = rating;
            Grid = grid;
        }
    }


    class JsonItem2
    {

    }


}
