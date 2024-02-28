using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xaml;

namespace FileVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte[] filecontent;
        private string filepath;
        private int x = 32;
        private int y = 32;
        private int number =32;
        public MainWindow()
        {
            InitializeComponent();
            X.Visibility = Visibility.Hidden;
            Y.Visibility = Visibility.Hidden;
            Number.Visibility = Visibility.Hidden;
            lx.Visibility = Visibility.Hidden;
            ly.Visibility = Visibility.Hidden;
            lnumber.Visibility = Visibility.Hidden;
            vbutton.Visibility = Visibility.Hidden;
            sbutton.Visibility = Visibility.Hidden;
            sbbutton.Visibility = Visibility.Hidden;
                }

        static bool[] CreateBitArray(byte[] byteArray)
        {
            bool[] bitArray = new bool[byteArray.Length * 8]; 

            for (int i = 0; i < byteArray.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bitArray[i * 8 + j] = (byteArray[i] & (1 << (7 - j))) != 0;
                }
            }

            return bitArray;
        }

        byte[] ReadBytes(string filePath, int numberOfBytesToRead)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            byte[] bytes;

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                bytes = new byte[numberOfBytesToRead];
                int bytesRead = fs.Read(bytes, 0, numberOfBytesToRead);

            }

            return bytes;
        }

        private void X_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;


            if (!string.IsNullOrEmpty(textBox.Text) && textBox.Text[0] == '0')
            {
                textBox.Text = "";
            }
        }

        private void X_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }


        private void sbutton_Click(object sender, RoutedEventArgs e)
        {
            
            StringWriter sw = new StringWriter();
            var bits = CreateBitArray(filecontent);

            int len = bits.Length;

            int x1 = 0, y1 = 0;

            for (int i = 0; i < len; i++)
            {
                if (x1 == x)
                {
                    y1++;
                    x1 = 0;

                    sw.WriteLine();

                    if (y1 == y) break;

                }

                sw.Write(bits[i] ? "1" : "0");

                x1++;
            }

            sw = new StringWriter();

            len = filecontent.Length;

            x1 = 0;
            y1 = 0;

            for (int i = 0; i < len; i++)
            {
                if (x1 == x / 8)
                {
                    y1++;
                    x1 = 0;

                    sw.WriteLine();

                    if (y1 == y) break;

                }

                sw.Write(filecontent[i] + " ");

                x1++;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.Title = "Save";
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {

                string filePath = saveFileDialog.FileName;
                File.WriteAllText(filePath, sw.ToString());
                
            }

        }

        private void selbutton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Title = "Open a file";
            bool? result = file.ShowDialog();
            if (result == true)
            {
                
                 filepath = file.FileName;
                string[] con = filepath.Split('\\');
                 sl.Content= con[con.Length-1];

                X.Visibility = Visibility.Visible;
                Y.Visibility = Visibility.Visible;
                Number.Visibility = Visibility.Visible;
                lx.Visibility = Visibility.Visible;
                ly.Visibility = Visibility.Visible;
                lnumber.Visibility = Visibility.Visible;
                vbutton.Visibility = Visibility.Visible;
            }
        }

        private void vbutton_Click(object sender, RoutedEventArgs e)
        {
            
            if (int.TryParse(X.Text, out int a))
            {
                x = a;
                if (int.TryParse(Y.Text, out int a1))
                {
                    y = a1;
                    if (int.TryParse(Number.Text, out int a2))
                    {
                        if(x<32)
                        {
                            x = 32;
                            X.Text = "32";
                        }
                        if (y < 32)
                        {
                            y = 32;
                            Y.Text = "32";
                        }
                        number = a2;
                        im.Height = y ;
                        im.Width = x ;




                        Bitmap bitmap = new Bitmap(x, y);
                        filecontent = ReadBytes(filepath, number);
                        var bits = CreateBitArray(filecontent);

                        for (int i = 0; i < x; i++)
                        {
                            for (int j = 0; j < y; j++)
                            {
                                bitmap.SetPixel(i, j, System.Drawing.Color.Blue);
                            }
                        }

                        int len = bits.Length;

                        int x1 = 0, y1 = 0;

                        for (int i = 0; i < len; i++)
                        {
                            if (x1 == x)
                            {
                                y1++;
                                x1 = 0;

                                if (y1 == y) break;

                            }

                            bitmap.SetPixel(x1, y1, bits[i] ? System.Drawing.Color.White : System.Drawing.Color.Black);


                            x1++;
                        }
                        BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                            bitmap.GetHbitmap(),
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                        im.Source = bitmapSource;
                        sbutton.Visibility = Visibility.Visible;
                        sbbutton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show("Number is not a int");

                    }
                }
                else
                {
                    MessageBox.Show("Y is not a int");
                }
            }
            else
            {
                MessageBox.Show("X is not a int");
            }

            
            
            
        }

        private void sbbutton_Click(object sender, RoutedEventArgs e)
        {

            string sw = "";
            var bits = CreateBitArray(filecontent);
            double d = x / 8;
            int newc = (int)Math.Floor(d);
            if(newc ==0)
            {
                newc = 1;
            }
            int sl = 0;
            int sl1 = 0;
            for(int i =0;i!=bits.Length;i++)
            {
                
                if(sl==8) 
                {
                    sw+=" ";
                    sl= 0;
                    sl1++;
                }
                if(sl1==newc)
                {
                    sw+="\n";
                   
                    sl1 = 0;
                }
                if (bits[i])
                {
                    sw+="1";
                }
                else
                {
                    sw+="0";
                }
                sl++;
            }
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.Title = "Save";
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {

                string filePath = saveFileDialog.FileName;
                File.WriteAllText(filePath, sw.ToString());

            }
        }
    }
}
