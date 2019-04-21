//Title          :   MainWindow
//Author         :   .....
//URL/Mail       :   onsaurav@yahoo.com/onsaurav@gmail.com/onsaurav@hotmail.com
//Description    :   MainWindow.
//Created        :   Saurav Biswas / Jun-10-2011
//Modified       :   
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.Xml.Linq;

namespace Cabinet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Member
        int contentCount = 1;
        #endregion
        #region Method
        public MainWindow()
        {
            //Summary    :   Constructor.
            //Created    :   Saurav Biswas / Jun-10-2011
            //Modified   :   
            //Parameters :  

            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //Summary    :   Method will used to close this program.
            //Created    :   Saurav Biswas / Jun-10-2011
            //Modified   :   
            //Parameters : 

            if (MessageBox.Show("Are you sure, you want to close this program?", "Close", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            Application.Current.Shutdown();
        }

        private void btnCaninet_Click(object sender, RoutedEventArgs e)
        {
            //Summary    :   Method will used to add the user control.
            //Created    :   Saurav Biswas / Jun-10-2011
            //Modified   :   
            //Parameters :

            try
            {
                MyCabinet oMyCabinet = new MyCabinet();
                oMyCabinet.Sl = contentCount;
                oMyCabinet.MyName = "Cabinet" + contentCount.ToString("000");
                oMyCabinet.Height = 100;
                oMyCabinet.Width = 50;

                oMyCabinet.Image_Name = "Default";
                var background = new ImageBrush();
                background.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/cabinet.png"));
                oMyCabinet.Background = background;
                oMyCabinet.ShowName();
                MainGrid.Children.Add(oMyCabinet);
                contentCount++;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveLayout();            
        }

        private void SaveLayout()
        {
            //Summary    :   Method will used to save the user control loyet.
            //Created    :   Saurav Biswas / Jun-10-2011
            //Modified   :   
            //Parameters :

            try
            {
                DependencyObject dependencyObject = this.MainGrid as DependencyObject;
                if (dependencyObject != null)
                {
                    XmlTextWriter textWriter = new XmlTextWriter("layout.xml", null);
                    textWriter.WriteStartDocument();
                    textWriter.WriteStartElement("Layout");

                    foreach (object child in LogicalTreeHelper.GetChildren(dependencyObject))
                    {
                        if (child.GetType().Name == "MyCabinet")
                        {
                            MyCabinet oMyCbt = new MyCabinet();
                            oMyCbt = (MyCabinet)child;
                            if (oMyCbt.Visibility != System.Windows.Visibility.Hidden)
                            {
                                string sss = "";
                                foreach (Drawer s in oMyCbt.MyDrawer)
                                {
                                    if (sss.Trim() != "") sss = sss + "#ONS#";
                                    sss = sss + s._Name;

                                    string subStr="#ABZ#";
                                    foreach (string ss in s._List)
                                    {
                                        if (subStr.Trim() != "") subStr = subStr + "#SUB#";
                                        subStr = subStr + ss;
                                    }
                                    sss = sss + subStr;
                                }

                                if (oMyCbt.Img_Change == true)
                                {
                                    string savedPath = System.IO.Directory.GetCurrentDirectory() + "\\Images\\" + oMyCbt.Image_Name;
                                    string filename = oMyCbt.Image_Path;
                                    System.IO.File.Copy(filename, @"" +savedPath);                                    
                                }
                                
                                Point coordinates = oMyCbt.TransformToAncestor(this).Transform(new Point(0, 0));
                                textWriter.WriteStartElement("ABC" + oMyCbt.Sl.ToString("000") + " Top='" + coordinates.Y + "' Left='" + coordinates.X + "' Width='" + oMyCbt.ActualWidth + "' Height='" + oMyCbt.ActualHeight + "' LST='" + sss + "' Name='" + oMyCbt.MyName + "' IMG='" + oMyCbt.Image_Name + "'");
                                textWriter.WriteEndElement();
                            }
                        }
                    }
                    textWriter.WriteEndDocument();
                    textWriter.Close();
                    MessageBox.Show("Cabinet(s) loyet save successfuy.", "Save", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Summary    :   Method will used to load the program with the saved format.
            //Created    :   Saurav Biswas / Jun-10-2011
            //Modified   :   
            //Parameters :

            try
            {
                if (File.Exists("layout.xml"))
                {
                    XElement layout = XElement.Load("layout.xml");
                    var elements = layout.Elements().Select(el => new { Name = (string)el.Attribute("Name"), Top = (double)el.Attribute("Top"), Left = (double)el.Attribute("Left"), Width = (double)el.Attribute("Width"), Height = (double)el.Attribute("Height"), LST = (string)el.Attribute("LST"), IMG = (string)el.Attribute("IMG") });
                    foreach (var c in elements)
                    {
                        MyCabinet oMyCabinet = new MyCabinet();
                        oMyCabinet.MyName = c.Name;
                        oMyCabinet.Height = c.Height;
                        oMyCabinet.Width = c.Width;
                        Grid.SetColumn(oMyCabinet, 0);
                        Grid.SetRow(oMyCabinet, 0);
                        var initialLocation = oMyCabinet.TranslatePoint(new Point(0, 0), this);

                        double slacX = 0; double slacY = 0;
                        slacX = 83 + (Math.Abs(100 - c.Height) / 2);
                        slacY = 50 + (Math.Abs(50 - c.Width)/2);

                        string LstVal = "";
                        LstVal = c.LST;
                        string[] sep = { "#ONS#" };
                        string[] words = LstVal.Split(sep, StringSplitOptions.None);
                        
                        oMyCabinet.MyDrawer.Clear();
                        for (int j = 0; j < words.Length; j++)
                        {
                            string[] sub = { "#ABZ#" };
                            string[] words_Sub = words[j].Split(sub, StringSplitOptions.None);
                            if (words_Sub.Length > 0)
                            {
                                Drawer oD = new Drawer();
                                oD._Name = words_Sub[0];
                                oD._List = new List<string>();
                                if (words_Sub.Length > 1)
                                {
                                    string[] sp = { "#SUB#" };
                                    string[] words_Sp = words_Sub[1].Split(sp, StringSplitOptions.None);
                                    for (int k = 0; k < words_Sp.Length; k++)
                                    {
                                        oD._List.Add(words_Sp[k]);
                                    }
                                }                                
                                oMyCabinet.MyDrawer.Add(oD);
                            }
                        }
                        oMyCabinet.LoadDrawer();

                        oMyCabinet.Image_Name = c.IMG;
                        try
                        {
                            if (oMyCabinet.Image_Name.Trim() != "" && oMyCabinet.Image_Name.Trim() != "Default")
                            {
                                var background = new ImageBrush();
                                string iPath = System.IO.Directory.GetCurrentDirectory() + "\\Images\\" + c.IMG;
                                background.ImageSource = new BitmapImage(new Uri(iPath, UriKind.Absolute));
                                oMyCabinet.Background = background;
                            }
                            else
                            {
                                oMyCabinet.Image_Name = "Default";
                                var background = new ImageBrush();
                                background.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/cabinet.png"));
                                oMyCabinet.Background = background;
                            }
                        }
                        catch { }
                        oMyCabinet.RenderTransform = new TranslateTransform(c.Left - (MainGrid.ActualWidth / 2) + slacX, c.Top - (MainGrid.ActualHeight / 2) + slacY);
                        oMyCabinet.ShowName();
                        MainGrid.Children.Add(oMyCabinet);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK);
            }
        }
        #endregion
    }
}
