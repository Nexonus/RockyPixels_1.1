namespace RockyPixels.Models
{
    public class PostTagsEditViewModel
    {
        public int PostId { get; set; }
        public string Topic { get; set; }

        public List<int> SelectedTagIds { get; set; }

        public List<Tag> AllTags { get; set; }
    }
}
