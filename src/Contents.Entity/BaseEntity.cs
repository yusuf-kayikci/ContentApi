using System;

namespace Contents.Entity
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime DeleteTime { get; set; }
        public DateTime InsertTime { get; set; }
    }
}
