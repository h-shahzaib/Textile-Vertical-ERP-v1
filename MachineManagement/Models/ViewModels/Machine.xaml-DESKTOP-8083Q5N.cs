using MachineManagement.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static GlobalLib.SqliteDataAccess;

namespace MachineManagement.Models.ViewModels
{
    /// <summary>
    /// Interaction logic for Machine.xaml
    /// </summary>
    public partial class Machine : UserControl
    {
        List<MchStock> Programs { get; set; }
        public string MachineID { get; set; }
        public string OperatorName { get; set; }
        public string HelperName { get; set; }
        public int HeadCount { get; set; }

        public Machine(List<MchStock> Programs, string MachineID, string OperatorName, string HelperName, int HeadCount)
        {
            InitializeComponent();

            this.Programs = Programs;
            this.MachineID = MachineID;
            this.OperatorName = OperatorName;
            this.HelperName = HelperName;
            this.HeadCount = HeadCount;

            Loaded += Machine_Loaded;
        }

        private void Machine_Loaded(object sender, RoutedEventArgs e)
        {
            MachineIDBox.Text = MachineID;
            OperatorNameBox.Text = OperatorName;
            HelperNameBox.Text = HelperName;
            //CalculateTotalStitch();
        }

        private void CalculateTotalStitch()
        {
            int totalProgramStitch = 0;
            foreach (MchStock mchStock in Programs)
            {
                Stock stock = MainWindow.rawDataManager.Stocks.Where(i => i.DiaryNumber == mchStock.StockID).FirstOrDefault();
                Design design = MainWindow.rawDataManager.Designs.Where(i => i.ID == stock.DesignId.ToString()).FirstOrDefault();

                string[] CommaSeprateds = stock.RepString.Split(',');
                int totalReps = 0;
                foreach (string CommaSeprated in CommaSeprateds)
                {
                    string[] MinusSeprateds = CommaSeprated.Split('-');

                    int additive = 0;
                    if (stock.DesignMany == "true")
                        int.TryParse(MinusSeprateds[2], out additive);
                    else if (stock.DesignMany == "false")
                        int.TryParse(MinusSeprateds[1], out additive);

                    if (stock.RepType == "REPEATS")
                        totalReps += additive;
                    else if (stock.RepType == "YARD")
                    {
                        double Reps = 0;
                        if (HeadCount == 24)
                            Reps = additive / 8.5;
                        else if (HeadCount == 28)
                            Reps = additive / 10;

                        totalReps += (int)Math.Ceiling(Reps);
                    }
                    else if (stock.RepType == "FIXED-UNITS")
                    {
                        // Not implemented yet...
                    }
                    else if (stock.RepType == "UNFIXED-UNITS")
                    {
                        string[] commaSeprateds = design.AccLength.Split(',');

                        string DesignHeightStr = commaSeprateds.Where(i => i.Split('-')[0] == "DS" && i.Split('-')[1] == "DESIGN_HEIGHT").FirstOrDefault();
                        string DistanceStr = commaSeprateds.Where(i => i.Split('-')[0] == "DS" && i.Split('-')[1] == "DISTANCE").FirstOrDefault();
                        string BaseHeightStr = commaSeprateds.Where(i => i.Split('-')[0] == "DS" && i.Split('-')[1] == "BASE_HEIGHT").FirstOrDefault();

                        if (!string.IsNullOrWhiteSpace(DesignHeightStr)
                            && !string.IsNullOrWhiteSpace(DistanceStr)
                            && !string.IsNullOrWhiteSpace(BaseHeightStr))
                        {
                            double UnitDesignHeight;
                            double.TryParse(DesignHeightStr.Split('-')[2], out UnitDesignHeight);
                            UnitDesignHeight = UnitDesignHeight / 25.4;

                            int Distance;
                            int.TryParse(DistanceStr.Split('-')[2], out Distance);

                            int BaseHeight;
                            int.TryParse(BaseHeightStr.Split('-')[2], out BaseHeight);

                            double BottomMargin = 2.25;

                            if (UnitDesignHeight != 0 && Distance != 0 && BaseHeight != 0)
                            {
                                double EstimatedReps = (BaseHeight / (UnitDesignHeight + Distance)) + BottomMargin;
                                totalReps += (int)Math.Ceiling(EstimatedReps);
                            }
                            else MessageBox.Show("Design parameters are WRONG...");
                        }
                        else
                            MessageBox.Show("Design parameters are NOT PRESENT...");
                    }
                }

                totalProgramStitch += design.UnitStitch * totalReps;
            }

            programStitchBox.Text = totalProgramStitch.ToString("#,##0");
        }

        private void AddProgram_Click(object sender, RoutedEventArgs e)
        {
            AddProgram add = new AddProgram();
            add.Title = MachineIDBox.Text;
            add.Show();
        }
    }
}
