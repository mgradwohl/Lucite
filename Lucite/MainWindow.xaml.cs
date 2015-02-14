using System;
using System.IO;
using System.Collections.Generic;
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
using System.ComponentModel;

namespace Lucite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Employees employees = new Employees();

        public MainWindow()
        {
            InitializeComponent();
            employeedata.ItemsSource = employees;

            // set default dates in the controls - would be cool to remember these from last time
            datestart.SelectedDate = datestart.DisplayDate = DateTime.Parse("7/23/14");
            dateend.SelectedDate = dateend.DisplayDate = DateTime.Parse("2/12/2015");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Go();
        }

        public void Go()
        {
            string file = BrowseFile();
            headtraxcsv.Text = file;

            ParseFile(file);
            UpdateView();
        }

        public void UpdateView()
        {
            AwardDates();
            AwardText();

            // sort after data flows in
            employeedata.Items.SortDescriptions.Clear();

            employeedata.Items.SortDescriptions.Add(new SortDescription("Award", ListSortDirection.Ascending));
            employeedata.Items.SortDescriptions.Add(new SortDescription("StartDateTime", ListSortDirection.Ascending));

            foreach (var c in employeedata.Columns)
            {
                c.SortDirection = null;
            }
            employeedata.Columns[9].SortDirection = ListSortDirection.Ascending;
            employeedata.Columns[1].SortDirection = ListSortDirection.Ascending;

            employeedata.Items.Refresh();

            // TODO scroll the top of the list into view
        }

        public void AwardDates()
        {
            // project their startdate to be >= the last all hands date (start)
            // if you can project it into to be in between start and end
            // they deserve a cookie
            DateTime begin = datestart.SelectedDate.Value;
            DateTime end = dateend.SelectedDate.Value;

            int[] ai = { 1, 5, 10, 15, 20, 25, 30, 40 };

            foreach (Employee e in employees)
            {
                DateTime award = e.StartDateTime;

                // check for 5, 10, 15, 20, ...
                int j = 0;
                while ((DateTime.Compare(award, begin) < 0) && (j < 8))
                {
                    award = e.StartDateTime.AddYears(ai[j]);
                    j++;
                }

                int r1 = DateTime.Compare(award, begin);
                int r2 = DateTime.Compare(award, end);

                if (r1 >= 0 && r2 <= 0)
                {
                    e.AwardFlag = true;
                    e.Award = award.Year - e.StartDateTime.Year;
                }
                else
                {
                    e.AwardFlag = false;
                }

                e.AwardDateTime = award;
            }
        }

        public void AwardText()
        {
            foreach (Employee e in employees)
            {
                if (e.AwardFlag == true)
                {
                    int j = e.Award;
                    switch (j)
                    {
                        case 0: e.Anniversary = "New Hire";
                            break;
                        case 1: e.Anniversary = "One Year!";
                            break;
                        case 5: e.Anniversary = "Five Years!";
                            break;
                        case 10: e.Anniversary = "Ten Years!";
                            break;
                        case 15: e.Anniversary = "Fifteen Years!";
                            break;
                        case 20: e.Anniversary = "Twenty Years!";
                            break;
                        case 25: e.Anniversary = "Twenty-five Years!";
                            break;
                        case 30: e.Anniversary = "Thirty Years!";
                            break;
                        case 35: e.Anniversary = "Thirty-five Years!";
                            break;
                        case 40: e.Anniversary = "Fourty Years!";
                            break;
                        default: e.Anniversary = j.ToString() + " Years!";
                            break;
                    }
                }
            }
        }

        public void NextAwardDates()
        {
            DateTime end = dateend.SelectedDate.Value;

            foreach (Employee e in employees)
            {
                DateTime next = e.StartDateTime;

                // this year
                int r1 = DateTime.Compare(next, end);
                if (r1 >= 0)
                {
                    e.NextAwardDateTime = next;
                    continue;
                }

                // next year
                next = next.AddYears(1);
                r1 = DateTime.Compare(next, end);
                if (r1 >= 0)
                {
                    e.NextAwardDateTime = next;
                    continue;
                }

                // check for 5, 10, 15, 20, ...
                next = next.AddYears(4);
                while (next.Year <= end.Year)
                {
                    next = next.AddYears(5);
                }

                e.NextAwardDateTime = next;
            }
        }

        public void ParseFile(string file)
        {
            employees.Clear();
            try
            {
                StreamReader reader = new StreamReader(file);

                // make sure there is a Name column "Full Name" and a award date column "Service Award Date"
                // but don't care about the order
                string header = reader.ReadLine();
                string[] hv = header.Split(',');

                int i = 0;
                int nc = -1;
                int sc = -1;

                foreach (string h in hv)
                {
                    if (h == "Full Name")
                    {
                        nc = i;
                    }

                    if (h == "Service Award Date")
                    {
                        sc = i;
                    }
                    i++;
                }

                if ((nc == -1) || (nc == -1))
                {
                    // error columns not found
                }

                // skip the extra field because of the , in the name
                sc++;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    string last = values[nc].Substring(1);
                    string first = values[nc+1].Substring(1, values[nc+1].Length - 2);

                    Employee e = new Employee();
                    e.Name = first + " " + last;
                    e.StartDate = values[sc];

                    employees.Add(e);
                }
            }
            catch
            {
                Console.WriteLine("reading employee list failed: {0}", employeedata.ToString());
            }
        }
        
        public string BrowseFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Comma Separated Values (*.csv)|*.csv|All Files (*.*)|*.*";

            Nullable<bool> result = dlg.ShowDialog();

            string file = "";
            if (result.HasValue && result.Value)
            {
                file = dlg.FileName;
            }

            if (!File.Exists(file))
            {
                // file not found
                return null;
            }
            return file;
        }

        private void datestart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (datestart.IsVisible)
            {
                UpdateView();
            }
        }

        private void dateend_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dateend.IsVisible)
            {
                UpdateView();
            }
        }

        private void employeedata_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (e.Column.DisplayIndex == 8)
            {
                e.Column.SortMemberPath = "Award";
            }

            if (e.Column.DisplayIndex == 2)
            {
                e.Column.SortMemberPath = "StartDateTime";
            }
            e.Handled = false;
        }

        private void employeedata_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // let these columns autogenerate, but have them hidden. We can use them for sorting
            if (( e.PropertyName == "StartDateTime") ||
                (e.PropertyName == "AwardDateTime") ||
                //(e.PropertyName == "AwardDate") ||
                (e.PropertyName == "NextAward") ||
                (e.PropertyName == "NextAwardDateTime") ||
                (e.PropertyName == "AwardFlag") ||
                (e.PropertyName == "Award"))
            {
                e.Column.Visibility = Visibility.Hidden;
            }

            if ( e.PropertyName == "AwardDate")
            {
                e.Column.Header = "Award Date";
            }

            if (e.PropertyName == "StartDate")
            {
                e.Column.Header = "Start Date";
            }
        }
    }
}
