using System;
using System.Collections.Generic;

namespace Sudoku
{
    class JsonStorageGroup
    {
        public string StorageName { get; set; }
        public string Source { get; set; }
        public List<JsonCategory> Categories { get; set; }
    }

    class JsonCategory
    {
        public string Name { get; set; }
        public List<JsonItem> Items { get; set; }
    }

    class JsonItem
    {
        public string Name { get; set; }
        public bool Completed { get; set; }
        public bool Bookmarked { get; set; }
        public int Rating { get; set; }
        public string Grid { get; set; }
    }

}
