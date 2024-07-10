using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;

namespace AEWFF_Launcher
{
    public partial class FolderPathSelector : Window
    {
        public FolderPathSelector(string initialPath1 = "", string initialPath2 = "")
        {
            InitializeComponent();

            Path1TextBox.Text = initialPath1;
            Path2TextBox.Text = initialPath2;
            if (Path1TextBox.Text == Path2TextBox.Text)
            {
                SamePathCheckBox.IsChecked = true;
            }
        }

        private void BrowsePath1_Click(object sender, RoutedEventArgs e)
        {
            string selectedPath = OpenFolderDialog();
            if (selectedPath != null)
            {
                Path1TextBox.Text = selectedPath;
                if (SamePathCheckBox.IsChecked == true)
                {
                    Path2TextBox.Text = selectedPath;
                }
            }
        }

        private void BrowsePath2_Click(object sender, RoutedEventArgs e)
        {
            string selectedPath = OpenFolderDialog();
            if (selectedPath != null)
            {
                Path2TextBox.Text = selectedPath;
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string path1 = Path1TextBox.Text;
            string path2 = Path2TextBox.Text;

            if (string.IsNullOrEmpty(path1) || (string.IsNullOrEmpty(path2) && SamePathCheckBox.IsChecked == false))
            {
                MessageBox.Show("Please select both folder paths.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Config config = new Config
            {
                VanillaGamePath = path1,
                ModdedGamePath = path2
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(config, options);
            File.WriteAllText("Config.json", jsonString);

            MessageBox.Show($"Paths saved.", "Paths Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();

        }

        private static void SaveConfig()
        {
        }

        private void SamePathCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Path1TextBox != null && Path2TextBox != null && BrowsePath2Button != null)
            {
                Path2TextBox.Text = Path1TextBox.Text;
                Path2TextBox.IsEnabled = false;
                BrowsePath2Button.IsEnabled = false;
            }
        }

        private void SamePathCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Path2TextBox.IsEnabled = true;
            BrowsePath2Button.IsEnabled = true;
            Path2TextBox.Text = string.Empty;
        }

        private string OpenFolderDialog()
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection."
            };

            if (dialog.ShowDialog() == true)
            {
                return Path.GetDirectoryName(dialog.FileName);
            }
            return null;
        }
    }

}
