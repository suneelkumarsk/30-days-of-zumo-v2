using Microsoft.Azure.Mobile.Server;

namespace Backend.DataObjects
{
    public class Image : EntityData
    {
        public string Filename { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}