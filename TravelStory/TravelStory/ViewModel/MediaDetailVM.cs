using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TravelStory.Messages;
using TravelStory.Model;
using TravelStory.Resources;
using TravelStory.View;
using Xamarin.Forms;
using static TravelStory.Model.Interfaces;

namespace TravelStory.ViewModel
{
    public class MediaDetailVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string PageTitle { get; set; }
        public string Data { get; set; }
        public string LabelDescription { get; set; }
        public string Description { get; set; }
        public string DeleteMedia { get; set; }
        public string DeleteVideo { get; set; }
        public string SaveMedia { get; set; }
        public string PlayMedia { get; set; }
        public string AddMedia { get; set; }
        public string InfoImage { get; set; }

        public bool imgVis { get; set; }
        public bool videoVis { get; set; }
        public bool DelVideoVis { get; set; }
        public bool addMediaVis { get; set; }
        public bool LabelInfoImage { get; set; }

        public ICommand PlayMediaCmd { get; set; }
        public ICommand DeleteMediaCmd { get; set; }
        public ICommand DeleteVideoCmd { get; set; }
        public ICommand SaveMediaCmd { get; set; }
        public ICommand AddMediaCmd { get; set; }
        public ICommand OpenPromptPhotoCmd { get; set; }

        public ImageSource FileImage { get; set; }

        public bool ShowDel { get; set; }
        public bool ShowSave { get; set; }       

        protected MaintenanceDB<MediaM> connMedia;

        MediaM itemPassed;

        public MediaDetailVM()
        {
            PageTitle = Dictionary.ResourceManager.GetString("Detail", Dictionary.Culture);
            LabelDescription = Dictionary.ResourceManager.GetString("LabelDesc", Dictionary.Culture);
            DeleteMedia = Dictionary.ResourceManager.GetString("Delete", Dictionary.Culture);
            SaveMedia = Dictionary.ResourceManager.GetString("Save", Dictionary.Culture);
            DeleteVideo = Dictionary.ResourceManager.GetString("DeleteVideo", Dictionary.Culture);
            InfoImage = Dictionary.ResourceManager.GetString("InfoImage", Dictionary.Culture);

            itemPassed = (MediaM)App.SelectedObj;

            ShowDel = true;
            ShowSave = true;

            Data = itemPassed.TimeStampString;
            Description = itemPassed.Text;

            OpenPromptPhotoCmd = new Command(OpenPromptPhotoFunc);

            PlayMedia = Dictionary.ResourceManager.GetString("Play", Dictionary.Culture);
            PlayMediaCmd = new Command(PlayMediaFunc);

            AddMedia = Dictionary.ResourceManager.GetString("AddMedia", Dictionary.Culture);
            AddMediaCmd = new Command(AddMediaFunc);

            if (itemPassed.Type == MediaType.PHOTO.ToString())
            {
                imgVis = true;
                videoVis = false;
                DelVideoVis = false;
                addMediaVis = false;
                LabelInfoImage = true;

                FileImage = ImageSource.FromFile(itemPassed.Path);
            }

            if (itemPassed.Type == MediaType.VIDEO.ToString())
            {
                imgVis = false;
                videoVis = true;
                DelVideoVis = true;
                addMediaVis = false;
                LabelInfoImage = false;
            }

            if (itemPassed.Type == MediaType.TEXT.ToString() || itemPassed.Type == MediaType.LOCATION.ToString())
            {
                imgVis = false;
                videoVis = false;
                DelVideoVis = false;
                addMediaVis = true;
                LabelInfoImage = false;
            }

            SaveMediaCmd = new Command(SaveMediaFunc);
            DeleteMediaCmd = new Command(DeleteMediaFunc);
            DeleteVideoCmd = new Command(DeleteVideoFunc);

            connMedia = new MaintenanceDB<MediaM>();
        }


        private async void AddMediaFunc()
        {
            var title = Dictionary.ResourceManager.GetString("Add", Dictionary.Culture)+":";
            var back = Dictionary.ResourceManager.GetString("Back", Dictionary.Culture);
            var addPhoto = Dictionary.ResourceManager.GetString("Photo", Dictionary.Culture);
            var addVideo = Dictionary.ResourceManager.GetString("Video", Dictionary.Culture);
            var action = await App.CurrentPage.DisplayActionSheet(title, back, null, addPhoto, addVideo);

            if (action == addPhoto)
            {
                #region PICK PHOTO
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    return;
                }

                var fileName = string.Format("photo_{0}.png", itemPassed.IdMedia);

                var file = await CrossMedia.Current.PickPhotoAsync();

                if (file == null)
                    return;

                itemPassed.Type = MediaType.PHOTO.ToString();
                itemPassed.Name = fileName;
                itemPassed.Path = file.Path;
                itemPassed.Synced = false;
                itemPassed.ModType = ModType.UPDATED;
                connMedia.SaveEdit(itemPassed);

                var travel = ManageDB.mainConnection.GetTravel(itemPassed.IdTravel);
                travel.ModType = ModType.UPDATED;
                travel.Synced = false;
                var connTravel = new MaintenanceDB<TravelM>();
                connTravel.SaveEdit(travel);

                FileImage = ImageSource.FromFile(itemPassed.Path);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FileImage"));

                imgVis = true;              
                addMediaVis = false;
                LabelInfoImage = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LabelInfoImage"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("imgVis"));                
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("addMediaVis"));

                //await GeneralFunc.UploadToAzure();
                #endregion
            }
            else
            {
                #region PICK VIDEO
                if (action == addVideo)
                {
                    await CrossMedia.Current.Initialize();

                    if (!CrossMedia.Current.IsPickVideoSupported)
                    {
                        return;
                    }

                    var fileName = string.Format("video_{0}.mp4", itemPassed.IdMedia);

                    var file = await CrossMedia.Current.PickVideoAsync();

                    if (file == null)
                        return;

                    itemPassed.Type = MediaType.VIDEO.ToString();
                    itemPassed.Name = fileName;
                    itemPassed.Path = file.Path;
                    itemPassed.Synced = false;
                    itemPassed.ModType = ModType.UPDATED;
                    connMedia.SaveEdit(itemPassed);

                    var travel = ManageDB.mainConnection.GetTravel(itemPassed.IdTravel);
                    travel.ModType = ModType.UPDATED;
                    travel.Synced = false;
                    var connTravel = new MaintenanceDB<TravelM>();
                    connTravel.SaveEdit(travel);

                    videoVis = true;
                    DelVideoVis = true;
                    addMediaVis = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("videoVis"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DelVideoVis"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("addMediaVis"));

                    //await GeneralFunc.UploadToAzure();
                }
                #endregion
            }
        }

        private async void OpenPromptPhotoFunc()
        {
            var back = Dictionary.ResourceManager.GetString("Back", Dictionary.Culture);
            var delPhoto = Dictionary.ResourceManager.GetString("delPhoto", Dictionary.Culture);
            var openPhoto = Dictionary.ResourceManager.GetString("openPhoto", Dictionary.Culture);
            var action = await App.CurrentPage.DisplayActionSheet("", back, null, openPhoto, delPhoto);

            if (action == delPhoto)
            {
                #region DELETE PHOTO
                var okDel = await DeleteMediaFile();
                if (okDel)
                {
                    imgVis = false;
                    addMediaVis = true;
                    LabelInfoImage = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LabelInfoImage"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("imgVis"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("addMediaVis"));

                    //await GeneralFunc.UploadToAzure();
                }                   
                #endregion
            }
            else
            {
                #region OPEN PHOTO
                if (action == openPhoto)
                {
                    Device.OpenUri(new Uri(itemPassed.Path));
                }             
                #endregion
            }
        }

        private async void DeleteVideoFunc()
        {
            var okDel = await DeleteMediaFile();
            if (okDel)
            {
                addMediaVis = true;
                videoVis = false;
                DelVideoVis = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("addMediaVis"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("videoVis"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DelVideoVis"));

                //await GeneralFunc.UploadToAzure();
            }
            
        }

        private async Task<bool> DeleteMediaFile()
        {
            var title = Dictionary.ResourceManager.GetString("TitleConfMex", Dictionary.Culture);
            var mex = Dictionary.ResourceManager.GetString("MexDelFile", Dictionary.Culture);
            var yes = Dictionary.ResourceManager.GetString("Yes", Dictionary.Culture);
            var no = Dictionary.ResourceManager.GetString("No", Dictionary.Culture);

            var answer = await App.CurrentPage.DisplayAlert(title, mex, yes, no);
            if (answer)
            {
                if (itemPassed.Text == "")
                {
                    itemPassed.Type = MediaType.LOCATION.ToString();
                }
                else
                {
                    itemPassed.Type = MediaType.TEXT.ToString();
                }
                itemPassed.Name = "";
                itemPassed.Path = "";
                itemPassed.Synced = false;
                itemPassed.ModType = ModType.UPDATED;
                connMedia.SaveEdit(itemPassed);

                var travel = ManageDB.mainConnection.GetTravel(itemPassed.IdTravel);
                travel.ModType = ModType.UPDATED;
                travel.Synced = false;
                var connTravel = new MaintenanceDB<TravelM>();
                connTravel.SaveEdit(travel);
                return true;
            }
            else
            {
                return false;
            }
                
        }

        private async void DeleteMediaFunc()
        {
            var title = Dictionary.ResourceManager.GetString("TitleConfMex", Dictionary.Culture);
            var mex = Dictionary.ResourceManager.GetString("MexDelMedia", Dictionary.Culture);
            var yes = Dictionary.ResourceManager.GetString("Yes", Dictionary.Culture);
            var no = Dictionary.ResourceManager.GetString("No", Dictionary.Culture);

            var answer = await App.CurrentPage.DisplayAlert(title, mex, yes, no);
            if (answer)
            {
                itemPassed.Synced = false;
                itemPassed.ModType = ModType.DELETED;
                connMedia.SaveEdit(itemPassed);

                var travel = ManageDB.mainConnection.GetTravel(itemPassed.IdTravel);
                travel.ModType = ModType.UPDATED;
                travel.Synced = false;
                var connTravel = new MaintenanceDB<TravelM>();
                connTravel.SaveEdit(travel);

                MessagingCenter.Send(new PageSelected(typeof(MediaDetail), false, true), "");
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageSelected"));
                //await GeneralFunc.UploadToAzure();
            }
        }

        private void SaveMediaFunc()
        {
            //await GeneralFunc.UploadToAzure();
            MessagingCenter.Send(new PageSelected(typeof(MediaDetail), false, true), "");
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageSelected"));
        }

        private void PlayMediaFunc()
        {
            Device.OpenUri(new Uri(itemPassed.Path));
        }

    }
}
