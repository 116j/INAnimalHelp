using Xamarin.Forms;

namespace INAnimalHelp.Models.PlacesSearchBar
{
    /// <summary>
    /// Обработчик события получения информации о местах.
    /// </summary>
    public delegate void PlacesRetrievedEventHandler(object sender, AutoCompleteResult result);

    /// <summary>
    /// Places bar.
    /// </summary>
    public class PlacesBar : SearchBar
    {
        /// <summary>
        /// Резервное хранилище для свойства ApiKey.
        /// </summary>
        public static readonly BindableProperty ApiKeyProperty = BindableProperty.Create(nameof(ApiKey), typeof(string), typeof(PlacesBar), string.Empty, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate)null, (BindableProperty.BindingPropertyChangedDelegate)null, (BindableProperty.BindingPropertyChangingDelegate)null, (BindableProperty.CoerceValueDelegate)null, (BindableProperty.CreateDefaultValueDelegate)null);

        /// <summary>
        /// Резервное хранилище для свойства MinimumSearchText.
        /// </summary>
        public static readonly BindableProperty MinimumSearchTextProperty = BindableProperty.Create(nameof(MinimumSearchText), typeof(int), typeof(PlacesBar), 2, BindingMode.OneWay, (BindableProperty.ValidateValueDelegate)null, (BindableProperty.BindingPropertyChangedDelegate)null, (BindableProperty.BindingPropertyChangingDelegate)null, (BindableProperty.CoerceValueDelegate)null, (BindableProperty.CreateDefaultValueDelegate)null);

        #region Property accessors
        public string ApiKey
        {
            get {
                return (string)this.GetValue(PlacesBar.ApiKeyProperty);

            }
            set {
                this.SetValue(PlacesBar.ApiKeyProperty, (object)value);
            }
        }

        public int MinimumSearchText
        {
            get {
                return (int)this.GetValue(PlacesBar.MinimumSearchTextProperty);

            }
            set {
                this.SetValue(PlacesBar.MinimumSearchTextProperty, (object)value);
            }
        }

        #endregion

        /// <summary>
        /// Событие получения информации о местах.
        /// </summary>
        public event PlacesRetrievedEventHandler PlacesRetrieved;

        protected virtual void OnPlacesRetrieved(AutoCompleteResult e)
        {
            PlacesRetrievedEventHandler handler = PlacesRetrieved;
            handler?.Invoke(this, e);
        }

        public PlacesBar()
        {
            TextChanged += OnTextChanged;
        }

        /// <summary>
        /// Событие изменения текста.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length >= MinimumSearchText)
            {
                var predictions = await Places.GetPlaces(e.NewTextValue, ApiKey);
                if (PlacesRetrieved != null && predictions != null)
                    OnPlacesRetrieved(predictions);
                else
                    OnPlacesRetrieved(new AutoCompleteResult());
            }
            else
            {
                OnPlacesRetrieved(new AutoCompleteResult());
            }
        }
    }
}
