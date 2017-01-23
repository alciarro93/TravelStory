using Nito.AsyncEx;
using Plugin.Connectivity;
using Plugin.Geolocator.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Toasts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    public class HomeTravelVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public INotifyTaskCompletion InitializationNotifier { get; private set; }

        public string PageTitle { get; private set; }
        public string TakeNote { get; set; }
        public string NewTitle { get; set; }
        public string NewTitleTravel { get; set; }
        public string Save { get; set; }
        public string InfoEmptyTravel { get; set; }

        public ICommand GetPositionCmd { get; set; }
        public ICommand TakePhotoCmd { get; set; }
        public ICommand TakeVideoCmd { get; set; }
        public ICommand EndCmd { get; set; }
        public ICommand ShowPinsCmd { get; set; }
        public ICommand DelTravelCmd { get; set; }
        public ICommand TakeNoteCmd { get; set; }
        public ICommand DecodeAddCmd { get; set; }
        public ICommand EditTitleCmd { get; set; }
        public ICommand ChangeTitleCmd { get; set; }

        public IList<MediaM> MediaCurrTravel { get; private set; }
        public TravelM TravelSelected { get; set; }

        public ImageSource PhotoSource { get; set; }
        public ImageSource VideoSource { get; set; }
        public ImageSource LocationSource { get; set; }
        public ImageSource CancelSource { get; set; }

        public bool ShowAlways { get; set; }
        public bool ShowCurrTravel { get; set; }

        public string CurrentTravelID;

        public bool VisAddMedia { get; set; }
        public bool EnableAddMedia { get; set; }
        public bool VisEditTitle { get; set; }
        public bool StackInfoEmtyTravel { get; set; }

        protected MaintenanceDB<MediaM> connMedia;
        protected MaintenanceDB<TravelM> connTravel;
        protected MaintenanceDB<SettingsM> connSett;
        protected MaintenanceDB<ProfileM> connProfile;

        public HomeTravelVM()
        {
            connMedia = new MaintenanceDB<MediaM>();
            connTravel = new MaintenanceDB<TravelM>();
            connSett = new MaintenanceDB<SettingsM>();
            connProfile = new MaintenanceDB<ProfileM>();

            ShowAlways = true;

            ShowPinsCmd = new Command(ShowPinsFunc);
            DecodeAddCmd = new Command(DecodeAddFunc);
            EditTitleCmd = new Command(EditTitleFunc);
            DelTravelCmd = new Command(DelTravelFunc);
            ChangeTitleCmd = new Command(ChangeTitleFunc);

            VisEditTitle = false;
            NewTitle = " " + Dictionary.ResourceManager.GetString("NewTitle", Dictionary.Culture);
            CancelSource = ImageSource.FromResource("TravelStory.Resources.Icons.CancelBtn.png");
            Save = Dictionary.ResourceManager.GetString("Save", Dictionary.Culture);
            InfoEmptyTravel = Dictionary.ResourceManager.GetString("InfoEmptyTravel", Dictionary.Culture);

            if (App.SelectedObj != null)
            {
                #region VIAGGIO CONCLUSO
                if (App.SelectedObj.GetType() == typeof(TravelM))
                {
                    TravelSelected = (TravelM)App.SelectedObj;
                    App.SelectedObj = null;
                    PageTitle = TravelSelected.Title;
                    VisAddMedia = false;
                }
                #endregion
            }
            else 
            {
                #region VIAGGIO IN CORSO
                CurrentTravelID = App.Current_User.ActiveTravel;
                //CurrentTravelID = int.Parse(ManageDB.mainConnection.GetSetting(StringsM.CURRENT_TRAVEL).Value);
                PageTitle = ManageDB.mainConnection.GetTravel(CurrentTravelID).Title;
                TakeNote = " " + Dictionary.ResourceManager.GetString("TakeNote", Dictionary.Culture);

                PhotoSource = ImageSource.FromResource("TravelStory.Resources.Icons.PhotoBtn.png");
                VideoSource = ImageSource.FromResource("TravelStory.Resources.Icons.VideoBtn.png");
                LocationSource = ImageSource.FromResource("TravelStory.Resources.Icons.LocationBtn.png");
                

                ShowCurrTravel = true; //mostra l'icona nella toolbar per concludre il viaggio             
                VisAddMedia = true;
                EnableAddMedia = true;

                GetPositionCmd = new Command(GetPositionFunc);
                TakePhotoCmd = new Command(TakePhotoFunc);
                TakeVideoCmd = new Command(TakeVideoFunc);
                EndCmd = new Command(EndFunc);
                TakeNoteCmd = new Command(TakeNoteFunc);

                #endregion
            }
        }

        #region Pull-to-Refresh

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy == value)
                    return;

                isBusy = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsBusy"));
            }
        }

        private Command reloadUI;

        public Command ReloadUI
        {
            get
            {
                return reloadUI ?? (reloadUI = new Command(ExecuteReloadUI, () =>
                {
                    return !IsBusy;
                }));
            }
        }

        private void ExecuteReloadUI()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            ReloadUI.ChangeCanExecute();

            ReloadMediaList();

            IsBusy = false;
            ReloadUI.ChangeCanExecute();
        }
        #endregion

        //Ottine la posizione 
        private async Task Pos()
        {
            var notificator = DependencyService.Get<IToastNotificator>();
            notificator.HideAll();

            var title = Dictionary.ResourceManager.GetString("Info", Dictionary.Culture);
            var mex = Dictionary.ResourceManager.GetString("SearchingGPS", Dictionary.Culture);
            //await notificator.NotifySticky(ToastNotificationType.Info, title, mex);
            notificator.Notify(ToastNotificationType.Info,title, mex, TimeSpan.FromSeconds(2),null,true);

            await GeneralFunc.GetCurretPosition();
        }

        //ricarica la pagina dei media
        private void ReloadMediaList()
        {
            if (TravelSelected != null) 
            {
                MediaCurrTravel = ManageDB.mainConnection.GetMediasTravel(TravelSelected.IdTravel).OrderByDescending(a => a.TimeStamp).ToList();
            }
            else   
            {
                MediaCurrTravel = ManageDB.mainConnection.GetMediasTravel(CurrentTravelID).OrderByDescending(a => a.TimeStamp).ToList();
            }

            if (MediaCurrTravel.Count==0 && TravelSelected==null)
            {
                StackInfoEmtyTravel = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StackInfoEmtyTravel"));
            }
            else
            {
                StackInfoEmtyTravel = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StackInfoEmtyTravel"));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MediaCurrTravel"));
        }

        //mostra i pin sulla mappa ed il percorso tra ogni punto
        private async void ShowPinsFunc()
        {
            if (MediaCurrTravel.Count() > 0)
            {
                App.CollectionMedia = MediaCurrTravel;
                MessagingCenter.Send<PageSelected>(new PageSelected(typeof(Map), true, false), "");
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageSelected"));
            }
            else
            {
                var title = Dictionary.ResourceManager.GetString("Warning", Dictionary.Culture);
                var mex = Dictionary.ResourceManager.GetString("NoMedia", Dictionary.Culture);
                var notificator = DependencyService.Get<IToastNotificator>();
                bool tapped = await notificator.Notify(ToastNotificationType.Warning,
                    title, mex, TimeSpan.FromSeconds(2));
            }
        }

        //decodifica gli indirizzi che non sono stati calcolati
        private async void DecodeAddFunc()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                if (MediaCurrTravel.Count>0)
                {
                    foreach (var item in MediaCurrTravel)
                    {
                        if (item.CompleteAddress=="")
                        {
                            item.CompleteAddress = await GeneralFunc.GetCurrentAddress(item.Latitude, item.Longitude);
                            item.Synced = false;
                            item.ModType = ModType.UPDATED;
                            connMedia.SaveEdit(item);
                        }                       
                    }
                    ReloadMediaList();

                    var travel = ManageDB.mainConnection.GetTravel(CurrentTravelID);
                    travel.ModType = ModType.UPDATED;
                    travel.Synced = false;
                    connTravel.SaveEdit(travel);

                    //await GeneralFunc.UploadToAzure();
                }
                else
                {
                    var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
                    var mex = Dictionary.ResourceManager.GetString("NoStops", Dictionary.Culture);
                    var notificator = DependencyService.Get<IToastNotificator>();
                    await notificator.Notify(ToastNotificationType.Warning, title, mex, TimeSpan.FromSeconds(2), null, true);

                    //var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
                    //var mex = Dictionary.ResourceManager.GetString("NoStops", Dictionary.Culture);
                    //await App.CurrentPage.DisplayAlert(title, mex, "OK");
                }           
            }
            else
            {
                var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
                var mex = Dictionary.ResourceManager.GetString("NoInternet", Dictionary.Culture);
                var notificator = DependencyService.Get<IToastNotificator>();
                await notificator.Notify(ToastNotificationType.Error, title, mex, TimeSpan.FromSeconds(2), null, true);

                //var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
                //var mex = Dictionary.ResourceManager.GetString("NoInternet", Dictionary.Culture);
                //await App.CurrentPage.DisplayAlert(title, mex, "OK");
            }
        }

        private void TakeNoteFunc()
        {
            InitializationNotifier = NotifyTaskCompletion.Create(Pos());

            MessagingCenter.Send(new PageSelected(typeof(TakeNote),true,false), "");
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageSelected"));
        }

        private async void GetPositionFunc()
        {
            EnableAddMedia = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableAddMedia"));

            await Pos();

            if (GeneralFunc.Position!=null)
            {
                var itemM = new MediaM();
                itemM.IdTravel = CurrentTravelID;
                itemM.Type = MediaType.LOCATION.ToString();
                itemM.Latitude = GeneralFunc.Position.Latitude;
                itemM.Longitude = GeneralFunc.Position.Longitude;
                itemM.CompleteAddress = await GeneralFunc.GetCurrentAddress(itemM.Latitude, itemM.Longitude);
                connMedia.SaveEdit(itemM);

                ReloadMediaList();

                var travel = ManageDB.mainConnection.GetTravel(CurrentTravelID);
                travel.ModType = ModType.UPDATED;
                travel.Synced = false;
                connTravel.SaveEdit(travel);

                //await GeneralFunc.UploadToAzure();
            }
            EnableAddMedia = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableAddMedia"));
        }

        private async void TakePhotoFunc()
        {
            var back = Dictionary.ResourceManager.GetString("Back", Dictionary.Culture);
            var newPhoto = Dictionary.ResourceManager.GetString("newPhoto", Dictionary.Culture);
            var pickPhoto = Dictionary.ResourceManager.GetString("pickPhoto", Dictionary.Culture);
            var action = await App.CurrentPage.DisplayActionSheet("", "", null, newPhoto, pickPhoto);

            if (action == newPhoto)
            {
                #region take new photo
                EnableAddMedia = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableAddMedia"));

                var itemM = new MediaM();
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    return;
                }

                var fileName = string.Format("photo_{0}.png", itemM.IdMedia);

                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Directory = DependencyService.Get<ILocalSettings>().GetMediaSavePath(),
                    Name = fileName,
                });

                if (file == null)
                    return;

                await Pos();
                if (GeneralFunc.Position != null)
                {
                    itemM.Type = MediaType.PHOTO.ToString();
                    itemM.Name = fileName;
                    itemM.Path = file.AlbumPath;
                    itemM.Latitude = GeneralFunc.Position.Latitude;
                    itemM.Longitude = GeneralFunc.Position.Longitude;
                    itemM.CompleteAddress = await GeneralFunc.GetCurrentAddress(itemM.Latitude, itemM.Longitude);
                    itemM.IdTravel = CurrentTravelID;
                    connMedia.SaveEdit(itemM);

                    ReloadMediaList();

                    var travel = ManageDB.mainConnection.GetTravel(CurrentTravelID);
                    travel.ModType = ModType.UPDATED;
                    travel.Synced = false;
                    connTravel.SaveEdit(travel);

                    //await GeneralFunc.UploadToAzure();
                }
                file.Dispose();

                EnableAddMedia = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableAddMedia"));
                #endregion
            }
            else
            {
                #region pick photo from gallery
                if (action==pickPhoto)
                {
                    EnableAddMedia = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableAddMedia"));

                    var itemM = new MediaM();
                    await CrossMedia.Current.Initialize();

                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        return;
                    }

                    var fileName = string.Format("photo_{0}.png", itemM.IdMedia);

                    var file = await CrossMedia.Current.PickPhotoAsync();

                    if (file == null)
                        return;

                    await Pos();
                    if (GeneralFunc.Position != null)
                    {
                        itemM.Type = MediaType.PHOTO.ToString();
                        itemM.Name = fileName;
                        itemM.Path = file.Path;
                        itemM.Latitude = GeneralFunc.Position.Latitude;
                        itemM.Longitude = GeneralFunc.Position.Longitude;
                        itemM.CompleteAddress = await GeneralFunc.GetCurrentAddress(itemM.Latitude, itemM.Longitude);
                        itemM.IdTravel = CurrentTravelID;
                        connMedia.SaveEdit(itemM);

                        ReloadMediaList();

                        var travel = ManageDB.mainConnection.GetTravel(CurrentTravelID);
                        travel.ModType = ModType.UPDATED;
                        travel.Synced = false;
                        connTravel.SaveEdit(travel);
                    }

                    file.Dispose();

                    EnableAddMedia = true;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableAddMedia"));
                }                
                #endregion
            }

        }

        private async void TakeVideoFunc()
        {
            var back = Dictionary.ResourceManager.GetString("Back", Dictionary.Culture);
            var newVideo = Dictionary.ResourceManager.GetString("newVideo", Dictionary.Culture);
            var pickVideo = Dictionary.ResourceManager.GetString("pickVideo", Dictionary.Culture);
            var action = await App.CurrentPage.DisplayActionSheet("", "", null, newVideo, pickVideo);

            if (action == newVideo)
            {
                #region take new video
                EnableAddMedia = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableAddMedia"));

                var itemM = new MediaM();
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
                {
                    return;
                }

                var fileName = string.Format("video_{0}.mp4", itemM.IdMedia);

                var file = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
                {
                    SaveToAlbum = true,
                    Quality = VideoQuality.High,
                    Directory = DependencyService.Get<ILocalSettings>().GetMediaSavePath(),
                    Name = fileName,
                });

                if (file == null)
                    return;

                await Pos();
                if (GeneralFunc.Position != null)
                {

                    itemM.Type = MediaType.VIDEO.ToString();
                    itemM.Name = fileName;
                    itemM.Path = file.AlbumPath;
                    itemM.Latitude = GeneralFunc.Position.Latitude;
                    itemM.Longitude = GeneralFunc.Position.Longitude;
                    itemM.CompleteAddress = await GeneralFunc.GetCurrentAddress(itemM.Latitude, itemM.Longitude);
                    itemM.IdTravel = CurrentTravelID;
                    connMedia.SaveEdit(itemM);

                    ReloadMediaList();

                    var travel = ManageDB.mainConnection.GetTravel(CurrentTravelID);
                    travel.ModType = ModType.UPDATED;
                    travel.Synced = false;
                    connTravel.SaveEdit(travel);

                    //await GeneralFunc.UploadToAzure();
                }

                file.Dispose();

                EnableAddMedia = true;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableAddMedia"));
                #endregion
            }
            else
            {
                #region pick video from galley
                if (action== pickVideo)
                {
                    EnableAddMedia = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableAddMedia"));

                    var itemM = new MediaM();
                    await CrossMedia.Current.Initialize();

                    if (!CrossMedia.Current.IsPickVideoSupported)
                    {
                        return;
                    }

                    var fileName = string.Format("video_{0}.mp4", itemM.IdMedia);

                    var file = await CrossMedia.Current.PickVideoAsync();

                    if (file == null)
                        return;

                    await Pos();
                    if (GeneralFunc.Position != null)
                    {
                        itemM.Type = MediaType.VIDEO.ToString();
                        itemM.Name = fileName;
                        itemM.Path = file.Path;
                        itemM.Latitude = GeneralFunc.Position.Latitude;
                        itemM.Longitude = GeneralFunc.Position.Longitude;
                        itemM.CompleteAddress = await GeneralFunc.GetCurrentAddress(itemM.Latitude, itemM.Longitude);
                        itemM.IdTravel = CurrentTravelID;
                        connMedia.SaveEdit(itemM);

                        ReloadMediaList();

                        var travel = ManageDB.mainConnection.GetTravel(CurrentTravelID);
                        travel.ModType = ModType.UPDATED;
                        travel.Synced = false;
                        connTravel.SaveEdit(travel);
                    }
                       
                    file.Dispose();

                    EnableAddMedia = true;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("EnableAddMedia"));
                }               
                #endregion
            }

            
        }

        private async void EndFunc()
        {
            var title = Dictionary.ResourceManager.GetString("TitleConfMex", Dictionary.Culture);
            var mex = Dictionary.ResourceManager.GetString("MexEndTravel", Dictionary.Culture);
            var yes = Dictionary.ResourceManager.GetString("Yes", Dictionary.Culture);
            var no = Dictionary.ResourceManager.GetString("No", Dictionary.Culture);

            var answer = await App.CurrentPage.DisplayAlert(title, mex, yes, no);
            if (answer)
            {
                if (GeneralFunc.tokenSource != null)
                {
                    GeneralFunc.tokenSource.Cancel(); //smetto ci trovare la posizione
                }

                var travel = ManageDB.mainConnection.GetTravel(CurrentTravelID);
                travel.Ended = true;
                travel.EndDate = DateTime.Now;
                travel.Synced = false;
                travel.ModType = ModType.UPDATED;
                connTravel.SaveEdit(travel);

                App.Current_User.ActiveTravel = "";
                App.Current_User.Synced = false;
                connProfile.SaveEdit(App.Current_User);              

                MessagingCenter.Send(new PageSelected(typeof(Home)), "");
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageSelected"));

                //await GeneralFunc.UploadToAzure();
            }
            
        }

        private async void DelTravelFunc()
        {
            var title = Dictionary.ResourceManager.GetString("TitleConfMex", Dictionary.Culture);
            var mex = Dictionary.ResourceManager.GetString("MexDelTravel", Dictionary.Culture);
            var yes = Dictionary.ResourceManager.GetString("Yes", Dictionary.Culture);
            var no = Dictionary.ResourceManager.GetString("No", Dictionary.Culture);

            var answer = await App.CurrentPage.DisplayAlert(title, mex, yes, no);
            if (answer)
            {
                if (TravelSelected != null) // Viaggio già concluso
                {
                    var medias = ManageDB.mainConnection.GetMediasTravel(TravelSelected.IdTravel);
                    foreach (var item in medias)
                    {
                        item.Synced = false;
                        item.ModType = ModType.DELETED;
                        connMedia.SaveEdit(item);
                    }

                    var travel = ManageDB.mainConnection.GetTravel(TravelSelected.IdTravel);
                    travel.Synced = false;
                    travel.ModType = ModType.DELETED;
                    connTravel.SaveEdit(travel);

                    BackFuncToTravels();

                    //await GeneralFunc.UploadToAzure();
                }
                else  // Viaggio in corso
                {
                    if (GeneralFunc.tokenSource != null)
                    {
                        GeneralFunc.tokenSource.Cancel(); //smetto ci trovare la posizione
                    }

                    var medias = ManageDB.mainConnection.GetMediasTravel(CurrentTravelID);
                    foreach (var item in medias)
                    {
                        item.Synced = false;
                        item.ModType = ModType.DELETED;
                        connMedia.SaveEdit(item);
                    }

                    var travel = ManageDB.mainConnection.GetTravel(CurrentTravelID);
                    travel.Synced = false;
                    travel.ModType = ModType.DELETED;
                    connTravel.SaveEdit(travel);
                    //connTravel.Delete(travel);

                    App.Current_User.ActiveTravel = "";
                    App.Current_User.Synced = false;
                    connProfile.SaveEdit(App.Current_User);                   

                    MessagingCenter.Send(new PageSelected(typeof(HomeTravel)), "");
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageSelected"));

                    //await GeneralFunc.UploadToAzure();
                }                                  
            }
        }   

        private void BackFuncToTravels()
        {
            MessagingCenter.Send(new PageSelected(typeof(Travels)), "");
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageSelected"));
        }

        private void EditTitleFunc()
        {
            DependencyService.Get<ILocalSettings>().HideKeyboard();

            VisEditTitle = !VisEditTitle;
            if (TravelSelected == null)
            {
                VisAddMedia = !VisAddMedia;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("VisAddMedia"));
            }

            if (VisAddMedia)
            {
                if (MediaCurrTravel.Count == 0 && TravelSelected == null)
                {
                    StackInfoEmtyTravel = true;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StackInfoEmtyTravel"));
                }
                else
                {
                    StackInfoEmtyTravel = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StackInfoEmtyTravel"));
                }
            }
            else
            {
                StackInfoEmtyTravel = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StackInfoEmtyTravel"));
            }
                
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("VisEditTitle"));            
        }

        private async void ChangeTitleFunc()
        {          
            if (!String.IsNullOrEmpty(NewTitleTravel))
            {
                NewTitleTravel = NewTitleTravel.Trim();
                if (TravelSelected != null)
                {
                    var travel = ManageDB.mainConnection.GetTravel(TravelSelected.IdTravel);
                    travel.Title = NewTitleTravel;
                    travel.Synced = false;
                    travel.ModType = ModType.UPDATED;
                    connTravel.SaveEdit(travel);

                    BackFuncToTravels();

                    //await GeneralFunc.UploadToAzure();
                }
                else
                {
                    var travel = ManageDB.mainConnection.GetTravel(CurrentTravelID);
                    travel.Title = NewTitleTravel;
                    travel.Synced = false;
                    travel.ModType = ModType.UPDATED;
                    connTravel.SaveEdit(travel);

                    MessagingCenter.Send(new PageSelected(typeof(HomeTravel)), "");
                    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageSelected"));

                    //await GeneralFunc.UploadToAzure();
                }
            }
            else
            {
                var title = Dictionary.ResourceManager.GetString("Alert", Dictionary.Culture);
                var mex = Dictionary.ResourceManager.GetString("ErrNewTitle", Dictionary.Culture);

                var notificator = DependencyService.Get<IToastNotificator>();
                bool tapped = await notificator.Notify(ToastNotificationType.Warning,
                    title, mex, TimeSpan.FromSeconds(2));
            }
            
            
        }

    }
}
