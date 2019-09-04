

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkUwpApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3_CollectionsList : BindablePage
    {
        public Scenario3_CollectionsList()
        {
            this.InitializeComponent();
        }
        //protected override void OnNavigatedTo(NavigationEventArgs e) //it works
        //{
        //    base.OnNavigatedTo(e);
        //    if (e.Parameter is ImagesCollection)
        //    {
        //        CollectionsList.Items.Add((ImagesCollection)e.Parameter);
        //    }
        //}
    }
}
