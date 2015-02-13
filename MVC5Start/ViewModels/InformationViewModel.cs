namespace MVC5Start.ViewModels
{
    public class InformationViewModel
    {
        public InformationViewModel()
        {
            this.Title = "Information";
        }

        public string Title { get; set; }

        public string Message { get; set; }

        public string CallbackUrl { get; set; }

        public string LoginUrl { get; set; }
    }
}