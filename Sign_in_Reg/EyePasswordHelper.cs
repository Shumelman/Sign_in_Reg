using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sign_in_Reg.Helpers
{
    public static class EyePasswordHelper
    {
        private static readonly Geometry EyeOpenGeometry = Geometry.Parse("M12,9A3,3 0 0,0 9,12A3,3 0 0,0 12,15A3,3 0 0,0 15,12A3,3 0 0,0 12,9M12,17A5,5 0 0,1 7,12A5,5 0 0,1 12,7A5,5 0 0,1 17,12A5,5 0 0,1 12,17M12,4.5C7,4.5 2.73,7.61 1,12C2.73,16.39 7,19.5 12,19.5C17,19.5 21.27,16.39 23,12C21.27,7.61 17,4.5 12,4.5Z");
        private static readonly Geometry EyeClosedGeometry = Geometry.Parse("M11.83,9L15,12.16C15,12.11 15,12.05 15,12A3,3 0 0,0 12,9C11.94,9 11.89,9 11.83,9M7.53,9.8L9.08,11.35C9.03,11.56 9,11.77 9,12A3,3 0 0,0 12,15C12.22,15 12.44,14.97 12.65,14.92L14.2,16.47C13.53,16.8 12.79,17 12,17A5,5 0 0,1 7,12C7,11.21 7.2,10.47 7.53,9.8M2,4.27L4.28,6.55L4.73,7C3.08,8.3 1.78,10 1,12C2.73,16.39 7,19.5 12,19.5C13.55,19.5 15.03,19.2 16.38,18.66L16.81,19.08L19.73,22L21,20.73L3.27,3L2,4.27Z");

        public static void InitializeTiggelePasswordButton(Button b1, PasswordBox passwordBox, TextBox textBox)
        {
            SetOpenEyeIcon(b1);

            b1.Click += (s, e) => TogglePasswordVisibility(b1, passwordBox, textBox);
        }

        private static void SetOpenEyeIcon(Button button)
        {
            button.Content = new Path
            {
                Data = EyeOpenGeometry,
                Fill = Brushes.White,
                Stretch = Stretch.Uniform,
                Width = 16,
                Height = 16
            };
        }

        private static void SetClosedEyeIcon(Button button)
        {
            button.Content = new Path
            {
                Data = EyeClosedGeometry,
                Fill = Brushes.White,
                Stretch = Stretch.Uniform,
                Width = 16,
                Height = 16
            };
        }

        public static void TogglePasswordVisibility(Button b1, PasswordBox passwordBox, TextBox textBox)
        {
            if (passwordBox.Visibility == Visibility.Visible)
            {
                // Показать пароль
                textBox.Text = passwordBox.Password;
                passwordBox.Visibility = Visibility.Collapsed;
                textBox.Visibility = Visibility.Visible;

                SetClosedEyeIcon(b1);
            }
            else
            {
                // Скрыть пароль
                passwordBox.Password = textBox.Text;
                textBox.Visibility = Visibility.Collapsed;
                passwordBox.Visibility = Visibility.Visible;

                SetOpenEyeIcon(b1);
            }
        }
    }
}
