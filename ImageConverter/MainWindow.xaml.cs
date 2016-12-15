using ImageConverter.Enms;
using ImageConverter.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Diagnostics;

namespace ImageConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private ImageConversion selectedImage;
        private DirectoryInfo projectDirectory;

        /// <summary>
        /// Constructor: Setup UI elements and assign event handlers.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            lbSourceFiles.AllowDrop = true;
            lbSourceFiles.SelectionMode = SelectionMode.Single;

            lbSourceFiles.Drop += LbSourceFiles_Drop;
            lbSourceFiles.SelectionChanged += LbSourceFiles_SelectionChanged;

            tbTargetName.TextChanged += TbTargetName_TextChanged;

            drpResolution.ItemsSource = new List<TargetResolution> 
            {
                new TargetResolution { Name = "MDPI",   Value = TargetResolutionOptions.Mdpi },
                new TargetResolution { Name = "HDPI",   Value = TargetResolutionOptions.Hdpi },
                new TargetResolution { Name = "XHDPI",  Value = TargetResolutionOptions.Xhdpi },
                new TargetResolution { Name = "XXHDPI", Value = TargetResolutionOptions.Xxhdpi },
                new TargetResolution { Name = "XXXHDPI", Value = TargetResolutionOptions.Xxxhdpi }
            };

            drpResolution.SelectedIndex = 2; //TODO: What should be defa◄lt?

            txtProjectDirectory.Text = Properties.Settings.Default.ProjectDirectory;
            projectDirectory = new DirectoryInfo(Properties.Settings.Default.ProjectDirectory);
        }


        private void btnProcessQueue_Click(object sender, RoutedEventArgs e)
        {
            List<ImageConversion> queue = lbSourceFiles.Items.Cast<ImageConversion>().ToList();

            try
            {
                ImageProcessor.ProcessQueue(queue, projectDirectory);
            }
            catch(ConvertException ex)
            {
                MessageBox.Show(this, ex.Message, "Error during image conversion.");
            }
        }

        /// <summary>
        /// Event handler: When a image file is dragged and dropped in the list via mouse input, it is added to the queue for later processing.
        /// The ListBox called lbSourceFiles is defined in XAML. It is used as a GUI element and its ItemCollection is used to
        /// keep track of the incomming processing queue.
        /// </summary>
        /// <param name="sender">The ListBox</param>
        /// <param name="e">Information about the item that is dropped in the list</param>
        private void LbSourceFiles_Drop(object sender, DragEventArgs e)
        {
            string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            AddFilesToQueue(droppedFiles);
        }


        /// <summary>
        /// Event handler: When a specific file is selected in the list extended information
        /// about that file is shown in the GUI.
        /// </summary>
        /// <param name="sender">The ListBox</param>
        /// <param name="e">Information about the selected item</param>
        private void LbSourceFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO: consider worker method?

            if(e.AddedItems.Count > 0)
            {
                ImageConversion image = (ImageConversion)e.AddedItems[0];

                cvsImageOptions.IsEnabled = true;

                selectedImage = image;
                txtFileName.Text = image.SourceFile.FullName;
                tbTargetName.Text = image.TargetName;
                txtWidth.Text = image.TargetWidth.ToString();
                txtHeight.Text = image.TargetHeight.ToString();

                if (image.ResizeBy == ImageConversionOptions.Width)
                {
                    rbWidth.IsChecked = true;
                    txtWidth.IsEnabled = true;
                    txtHeight.IsEnabled = false;
                }
                else
                {
                    rbHeight.IsChecked = true;
                    txtWidth.IsEnabled = false;
                    txtHeight.IsEnabled = true;
                }

                foreach (object item in drpResolution.Items)
                {
                    TargetResolution option = (TargetResolution)item;
                    if (image.TargetResolution == option.Value)
                    {
                        drpResolution.SelectedItem = option;
                    }
                }

                btnRemoveImage.IsEnabled = true;
            }
        }

        /// <summary>
        /// Event Handler: When the target filename of an image is entered
        /// save the changes to the internal data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbTargetName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(selectedImage != null)
            {
                selectedImage.TargetName = tbTargetName.Text;
            }
        }

        private void rbWidth_Checked(object sender, RoutedEventArgs e)
        {
            selectedImage.ResizeBy = ImageConversionOptions.Width;
            txtWidth.IsEnabled = true;
            txtHeight.IsEnabled = false;
        }

        private void rbHeight_Checked(object sender, RoutedEventArgs e)
        {
            selectedImage.ResizeBy = ImageConversionOptions.Height;
            txtWidth.IsEnabled = false;
            txtHeight.IsEnabled = true;
        }

        private void txtWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox origin = (TextBox)sender;
                selectedImage.TargetWidth = int.Parse(origin.Text);
            }
            catch
            {
                MessageBox.Show("Please specify width in nmber of pixels.");
            }
        }

        private void txtHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox origin = (TextBox)sender;
                selectedImage.TargetHeight = int.Parse(origin.Text);
            }
            catch
            {
                MessageBox.Show("Please specify width in nmber of pixels.");
            }
        }

        private void txtWidth_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        private void txtHeight_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        private void drpResolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(selectedImage != null)
            {
                TargetResolution selectedOption = (TargetResolution)e.AddedItems[0];
                selectedImage.TargetResolution = selectedOption.Value;
            }
        }

        private void btnChangeAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This will chanange the resize options for all other images in the conversion queue. Do you wish to proceed?", "Change options", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach(object control in lbSourceFiles.Items)
                {
                    ImageConversion listedImage = (ImageConversion)control;

                    listedImage.ResizeBy = selectedImage.ResizeBy;
                    listedImage.TargetWidth = selectedImage.TargetWidth;
                    listedImage.TargetHeight = selectedImage.TargetHeight;
                    listedImage.TargetResolution = selectedImage.TargetResolution;
                    listedImage.Crop = selectedImage.Crop;
                    listedImage.CropSize = selectedImage.CropSize;
                }
            }
        }

        private void btnSetProjectDirectory_Click(object sender, RoutedEventArgs e)
        {
            var directoryPicker = new System.Windows.Forms.FolderBrowserDialog();
            directoryPicker.SelectedPath = Properties.Settings.Default.ProjectDirectory;
            System.Windows.Forms.DialogResult result = directoryPicker.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                projectDirectory = new DirectoryInfo(directoryPicker.SelectedPath);
                Properties.Settings.Default.ProjectDirectory = projectDirectory.FullName;
                Properties.Settings.Default.Save();
                txtProjectDirectory.Text = projectDirectory.FullName;
            }
        }

        private void btnAddImage_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new System.Windows.Forms.OpenFileDialog();
            filePicker.Multiselect = true;
            System.Windows.Forms.DialogResult result = filePicker.ShowDialog();

            if(result == System.Windows.Forms.DialogResult.OK)
            {
                string[] filesToAdd = filePicker.FileNames;
                AddFilesToQueue(filesToAdd);
            }
            
        }

        private void btnRemoveImage_Click(object sender, RoutedEventArgs e)
        {
            int index = lbSourceFiles.SelectedIndex;
            lbSourceFiles.Items.RemoveAt(lbSourceFiles.SelectedIndex);

            if(lbSourceFiles.Items.Count > 0)
            {
                lbSourceFiles.SelectedIndex = index - 1;
            }
            else
            {
                cvsImageOptions.IsEnabled = false;
                btnProcessQueue.IsEnabled = false;
                btnRemoveImage.IsEnabled = false;
            }
        }

        private void AddFilesToQueue(string[] filePaths)
        {
            //TODO: Only accept image files.
            btnProcessQueue.IsEnabled = true;

            foreach (string filePath in filePaths)
            {
                FileInfo info = new FileInfo(filePath);
                ImageConversion conversion = new ImageConversion(info);
                System.Drawing.Image imageData = System.Drawing.Image.FromFile(filePath);


                conversion.TargetName = info.Name;
                conversion.TargetWidth = imageData.Width;
                conversion.TargetHeight = imageData.Height;
                conversion.TargetResolution = TargetResolutionOptions.Xhdpi;

                lbSourceFiles.Items.Add(conversion);
            }
        }

        private void cbCrop_Checked(object sender, RoutedEventArgs e)
        {
            txtCropValue.IsEnabled = true;
            selectedImage.Crop = true;
        }

        private void cbCrop_Unchecked(object sender, RoutedEventArgs e)
        {
            txtCropValue.IsEnabled = false;
            selectedImage.Crop = false;
        }

        private void txtCropValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                selectedImage.CropSize = int.Parse(txtCropValue.Text);
            }
            catch
            {
                MessageBox.Show("Crop image: Enter a pixel val]e in numbers");
            }
        }

        private void txtCropValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }
    }
}