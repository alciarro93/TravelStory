using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelStory.Model;
using TravelStory.Resources;
using Xamarin.Forms;

namespace TravelStory.View
{
    public partial class TakeNote : ContentPage
    {

        public TakeNote()
        {
            InitializeComponent();
            BindingContext = new ViewModel.TakeNoteVM();

            SaveNoteBtn.Text = Dictionary.ResourceManager.GetString("Save", Dictionary.Culture) + " " +
                               Dictionary.ResourceManager.GetString("Note", Dictionary.Culture);
           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            EditorNote.Focus();
        }


    }
}
