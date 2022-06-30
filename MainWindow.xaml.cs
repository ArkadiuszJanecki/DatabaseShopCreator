using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using System.Data.Common;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Diagnostics;


namespace Database_Shop_Creator_Optimalized
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public DataTable dataTable = new DataTable();
        class mydbcontext : DbContext
        {
            public DbSet<Product> As { get; set; }
            public DbSet<Opinion> Bs { get; set; }
        }
        private void Exit_App(object sender, RoutedEventArgs e)
        {
            this.Close();
        }//wylaczanie

        private void Import_Database(object sender, RoutedEventArgs e)
        {
            HardResetCategories();
            string connString = @"Server = localhost; Database = master; Trusted_Connection = True;";
            //autowczytywanie bazy autostart

            try
            {
                //sql connection object
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"SELECT Id_Category,Parent,Name FROM Category ORDER BY Parent";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                    dataTable.Columns.Add("Added", typeof(bool));
                    foreach (DataRow row in dataTable.Rows)//dodawanie przyciskow kategorii do stackpanelu
                    {
                        if (row["Parent"].ToString() == "")
                        {
                            string name = row["ID_Category"].ToString();
                            string header = row["Name"].ToString();
                            Button newButton = new Button();
                            newButton.Height = 30;
                            newButton.Content = header;
                            newButton.Name = name;
                            newButton.Click += new RoutedEventHandler(button_Click);
                            CategoryList.Children.Add(newButton);
                            row["Added"] = false;
                        }
                        else
                        {
                            row["Added"] = false;
                        }
                    }
                    dataGrid1.DataContext = dataTable.DefaultView;
                    conn.Close();
                    da.Dispose();
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

        }//import bazy

        void button_Click(object sender, System.EventArgs e)
        {
            SoftResetCategories();
            Button button = sender as Button;
            string name = button.Name;
            StackPanel SubPanel = new StackPanel()
            {
                Name = "panel_" + name
            };

            Subcategories.Children.Add(SubPanel);
            //dodanie buttona pojedynczej podkategorii ktora bedzie miala podkategorie pod sobą

            for (int i = 0; i < 5; i++) //to by zmienic na jakies dynamiczne
            {
                //dodanie jednej podkategorii
                foreach (DataRow ParentRow in dataTable.Rows)
                {
                    if ((button.Name == ParentRow["Parent"].ToString()) && ParentRow["Added"].Equals(false))
                    {
                        Button subCategory = new Button();
                        subCategory.Name = ParentRow["Id_Category"].ToString();
                        subCategory.Content = ParentRow["Name"].ToString();
                        subCategory.Height = 35;
                        subCategory.Click += new RoutedEventHandler(button_goToCategory);
                        SubPanel.Children.Add(subCategory);
                        ParentRow["Added"] = true;
                        break;
                    }
                }
                //dodanie wszystkich podpodkategorii pod button wyzej
                foreach (DataRow ParentRow in dataTable.Rows)
                {
                    foreach (Button btn in SubPanel.Children)
                    {
                        if ((btn.Name == ParentRow["Parent"].ToString()) && ParentRow["Added"].Equals(false))
                        {
                            Button specCategory = new Button();
                            specCategory.Name = ParentRow["Id_Category"].ToString();
                            specCategory.Content = ParentRow["Name"].ToString();
                            specCategory.Height = 20;
                            specCategory.Click += new RoutedEventHandler(button_goToCategory);
                            SubPanel.Children.Add(specCategory);//tu jest blad
                            ParentRow["Added"] = true;
                            break;
                        }
                    }
                }
            }

        }//przyciski kategorii

        void button_goToCategory(object sender, System.EventArgs e)//konkretne wyszykiwanie produktow po kategorii
        {
            Button button = sender as Button;
            string name = button.Name;
            // string query = "SELECT * FROM dbo.Product as p INNER JOIN dbo.ProductWithCategory" +
            //" as pwc ON p.ID_Product = pwc.ID_Product WHERE ID_Category = '" + name + "'";
            DataTable allProductTable = new DataTable();
            List<Product> p = new List<Product>();
            using (masterEntities context = new masterEntities())
            {
                var products = context.Product.Where(c => c.Description.Contains(name));
                //p = products.ToList();
                dataGrid2.ItemsSource = products.ToList();
            }

           
        }

        void HardResetCategories()
        {
            dataTable.Reset();
            CategoryList.Children.Clear();
            Subcategories.Children.Clear();
        }//do resetowania 
        void SoftResetCategories()
        {
            Subcategories.Children.Clear();
            foreach (DataRow row in dataTable.Rows)
            {
                row["Added"] = false;
            }
        }//do resetowania
        void AddSampleProducts(string Name, string Desc, float Weight, float Height, float Width, float Depth, int ProdID, string ProdName, string Warranty)
        {
            //image
            string filePath = @"D:\ogor.jpg";
            FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);
            byte[] photo = reader.ReadBytes((int)stream.Length);
            reader.Close();
            stream.Close();
            //image
            string connString = @"Server = localhost; Database = makro; Trusted_Connection = True;";
            //sprawdzenie ostatniego ID by dynamicznie dodawac produkty
            Int32 count = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Product", conn);
                    count = (Int32)cmd.ExecuteScalar();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = @"SELECT COUNT(*) FROM Product";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT into makro.dbo.Product (ID_Product,Name,Description,Weight,Height,Width,Depth,Producent_ID,Producent_Name,Warranty,Image)" +
                        "VALUES(@ID_Product, @Name, @Description, @Weight, @Height, @Width, @Depth, @Producent_ID, @Producent_Name, @Warranty, @Image)";
                    cmd.Parameters.AddWithValue("@ID_Product", count);
                    cmd.Parameters.AddWithValue("@Name", Name);
                    cmd.Parameters.AddWithValue("@Description", Desc);
                    cmd.Parameters.AddWithValue("@Weight", Weight);
                    cmd.Parameters.AddWithValue("@Height", Height);
                    cmd.Parameters.AddWithValue("@Width", Width);
                    cmd.Parameters.AddWithValue("@Depth", Depth);
                    cmd.Parameters.AddWithValue("@Producent_ID", ProdID);
                    cmd.Parameters.AddWithValue("@Producent_Name", ProdName);
                    cmd.Parameters.AddWithValue("@Warranty", Warranty);
                    cmd.Parameters.AddWithValue("@Image", photo);
                    cmd.ExecuteNonQuery();
                    conn.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }//dodawanie produktow


        private void Window_Initialized(object sender, EventArgs e)
        {
            HardResetCategories(); //dorobic usuwanie guzikow 

            using (masterEntities context = new masterEntities())
            {
                var categories = context.Category.ToList();
            }
            string connString = @"Server = localhost; Database = master; Trusted_Connection = True;";
            //autowczytywanie bazy autostart
            try
            {
                //sql connection object
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = @"SELECT Id_Category,Parent,Name FROM Category ORDER BY Parent";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dataTable);
                    dataTable.Columns.Add("Added", typeof(bool));
                    foreach (DataRow row in dataTable.Rows)//dodawanie przyciskow kategorii do stackpanelu
                    {
                        if (row["Parent"].ToString() == "")
                        {
                            string name = row["ID_Category"].ToString();
                            string header = row["Name"].ToString();
                            Button newButton = new Button();
                            newButton.Height = 30;
                            newButton.Content = header;
                            newButton.Name = name;
                            newButton.Click += new RoutedEventHandler(button_Click);
                            CategoryList.Children.Add(newButton);
                            row["Added"] = false;
                        }
                        else
                        {
                            row["Added"] = false;
                        }
                    }
                    dataGrid1.DataContext = dataTable.DefaultView;
                    conn.Close();
                    da.Dispose();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            //for (int i = 0; i < 2000; i++)
            //{
            //    AddSampleProducts("Ogorek zielony" + i, "Swiezy ", i, i, i, i, i, "Polands", "4 dni ");
            //}


        }//po wczytaniu okna

        private void ClickSearch(object sender, RoutedEventArgs e)
        {
            //wyszukiwanie all EF LINQ Method   ///////////////////////////////////// zrobione
            //string searchProduct = SearchBar.Text;
            //DataTable allProductTable = new DataTable();
            //var watch = Stopwatch.StartNew();
            //using (masterEntities context = new masterEntities())
            //{
            //    var products = context.Product.Where(c => c.Name.Contains(searchProduct)).ToList();
            //    dataGrid2.ItemsSource = products;
            //}
            //watch.Stop();
            //var elapsedTime = watch.ElapsedMilliseconds;
            //MessageBox.Show(elapsedTime.ToString());

            //wyszukiwanie all EF LINQ Query   ///////////////////////////////////// to do
            string searchProduct = SearchBar.Text;
            DataTable allProductTable = new DataTable();
            var watch = Stopwatch.StartNew();
            using (masterEntities context = new masterEntities())
            { 
                var products = (from st in context.Product
                               where st.Name.Contains(searchProduct)
                               select st).ToList();
                dataGrid2.ItemsSource = products;
            }
            watch.Stop();
            var elapsedTime = watch.ElapsedMilliseconds;
            MessageBox.Show(elapsedTime.ToString());
        }

        private void Add_Category(object sender, RoutedEventArgs e)
        {

        }//to do

        private void Add_Product(object sender, RoutedEventArgs e)
        {

        }//to do

        private void ShowData(object sender, MouseButtonEventArgs e)//wyswietlanie danego przedmiotu
        {
            MessageBox.Show(sender.GetType().ToString());
            
            DataRowView row = dataGrid2.SelectedItem as DataRowView;
            LabelID.Content = row[0];
            LabelName.Content = row[1];
            LabelDesc.Content = row[2];
            LabelDims.Content = row[3] + "x" + row[4] + "x" + row[5] + "x" + row[6];
            LabelProdID.Content = row[7];
            LabelProdName.Content = row[8];
            LabelWarr.Content = row[9];
            try
            {
                byte[] photo = (byte[])row[10];
                System.Drawing.Image img = null;
                BitmapImage bi = new BitmapImage();
                using (MemoryStream ImageDataStream = new MemoryStream())
                {
                    ImageDataStream.Write(photo, 0, photo.Length);
                    ImageDataStream.Position = 0;
                    photo = System.Text.UnicodeEncoding.Convert(Encoding.Unicode, Encoding.Default, photo);
                    img = System.Drawing.Image.FromStream(ImageDataStream);

                    bi.BeginInit();
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();

                }
                ProductImage.Source = bi;
            }
            catch (Exception ex)
            {
                ProductImage.Source = null;
                Console.WriteLine(ex);
            }
        }

        private void AdvanceSearch(object sender, RoutedEventArgs e)
        {
            DataTable allProductTable = new DataTable();
            var watch = Stopwatch.StartNew();

            //wyszukiwanie  proc skł EF LINQ Method ///////////////////////////////////// to do
            using (masterEntities context = new masterEntities())
            {
                var query = context.Product
                    .Include(s => s.Price)
                    .Include(s => s.Promotion)
                    .SelectMany(c => c.StockStatus, (c, i) => new
                    {
                        c.ID_Product,
                        c.Name,
                        c.Description,
                        c.Weight,
                        c.Height,
                        c.Width,
                        c.Producent_ID,
                        c.Producent_Name,
                        c.Image,
                        i.Stock_Status,
                        c.Warranty,
                        Price3 = c.Price.Select(o => o.Price1),
                        c.Opinion
                        //Promotion3 = c.Promotion.Select(o=>o.Price_Percent)

                    })
                    .Where(c => c.Stock_Status > 1 && c.Warranty != null && c.Price3.FirstOrDefault() > 1 /*&& c.Promotion3.FirstOrDefault()>10*/).ToList();

                dataGrid2.DataContext = query;
            }

            //wyszukiwanie proc skł EF LINQ Query ///////////////////////////////////// zrobione
            //using (var context = new masterEntities())
            //{
            //    var query = from p in context.Product
            //                join pr in context.Price
            //                on p.ID_Product equals pr.ID_Product

            //                join ss in context.StockStatus
            //                on p.ID_Product equals ss.ID_Product

            //                join pro in context.Promotion
            //                on p.ID_Product equals pro.ID_Product

            //                join op in context.Opinion
            //                on p.ID_Product equals op.ID_Product

            //                where (pr.Price1 > 10 && ss.Stock_Status > 1 && pro.Price_Percent > 10 && p.Warranty != null)
            //                select new
            //                {
            //                    p.ID_Product,
            //                    p.Name,
            //                    p.Description,
            //                    p.Weight,
            //                    p.Height,
            //                    p.Width,
            //                    p.Depth,
            //                    p.Producent_ID,
            //                    p.Producent_Name,
            //                    p.Image,
            //                    pr.Price1,
            //                    pr.Shipping_Price,
            //                    op.Rating,
            //                    op.ID_Person,
            //                    op.ID_Opinion,
            //                    ss.Stock_Status,
            //                    pro.Price_Percent
            //                };
            //    var products = query.ToList();
            //    dataGrid2.ItemsSource = products;
            //}



            watch.Stop();
            var elapsedTime = watch.ElapsedMilliseconds;
            MessageBox.Show(elapsedTime.ToString());
        }
    }

}
