using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Client.UWP.Controls
{
    public sealed partial class RotatingIcon : UserControl
    {
        public RotatingIcon()
        {
            InitializeComponent();
            SetEnabledState();
        }

        #region Icon Property
        public Symbol Icon
        {
            get { return (Symbol)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Symbol), typeof(RotatingIcon), new PropertyMetadata(Symbol.Sync));
        #endregion

        #region DisabledColor Property
        public Brush DisabledColor
        {
            get { return (Brush)GetValue(DisabledColorProperty); }
            set { SetValue(DisabledColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisabledColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisabledColorProperty =
            DependencyProperty.Register("DisabledColor", typeof(Brush), typeof(RotatingIcon), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region Click Event
        public delegate void ClickHandler(object sender, RoutedEventArgs e);
        public event ClickHandler Click;

        /// <summary>
        /// Event Handler for the click event
        /// </summary>
        private void RIIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!IsEnabled)
                return;
            Click?.Invoke(this, null);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Enabled/Disabled Event Handler
        /// </summary>
        private void userControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetEnabledState();
        }

        /// <summary>
        /// Helper method to set the enabled state properly
        /// </summary>
        private void SetEnabledState()
        {
            if (IsEnabled)
            {
                RIAnimation.Stop();
                RIIcon.IsTapEnabled = true;
                RIIcon.Foreground = Foreground;
            }
            else
            {
                RIAnimation.Begin();
                RIIcon.IsTapEnabled = false;
                RIIcon.Foreground = DisabledColor;
            }
        }
        #endregion

    }
}
