using System.Diagnostics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Client.UWP.Controls
{
    public sealed partial class ClickableIcon : UserControl
    {
        public ClickableIcon()
        {
            Debug.WriteLine($"[ClickableIcon$constructor] Entry");
            InitializeComponent();
            SetEnabledState();
            Debug.WriteLine($"[ClickableIcon$constructor] Exit");
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
        private void CIIcon_Tapped(object sender, TappedRoutedEventArgs e)
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
            Debug.WriteLine($"[ClickableIcon$userControl_IsEnabledChanged] Entry");
            SetEnabledState();
            Debug.WriteLine($"[ClickableIcon$userControl_IsEnabledChanged] Exit");
        }

        /// <summary>
        /// Helper method to set the enabled state properly
        /// </summary>
        private void SetEnabledState()
        {
            Debug.WriteLine($"[ClickableIcon$SetEnabledState] Entry");
            if (IsEnabled)
            {
                Debug.WriteLine($"[ClickableIcon$SetEnabledState] IsEnabled = true");
                CIIcon.IsTapEnabled = true;
                Debug.WriteLine($"[ClickableIcon$SetEnabledState] Set Icon Color to {((SolidColorBrush)Foreground).Color}");
                CIIcon.Foreground = Foreground;
            }
            else
            {
                Debug.WriteLine($"[ClickableIcon$SetEnabledState] IsEnabled = false");
                CIIcon.IsTapEnabled = false;
                Debug.WriteLine($"[ClickableIcon$SetEnabledState] Set Icon Color to {((SolidColorBrush)DisabledColor).Color}");
                CIIcon.Foreground = DisabledColor;
            }
            Debug.WriteLine($"[ClickableIcon$SetEnabledState] Exit");
        }
        #endregion

    }
}
