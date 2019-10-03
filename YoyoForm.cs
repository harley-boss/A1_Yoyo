/// File:        YoyoForm.cs
/// Assigment:   Assignment #2 - Yoyo
/// Class:       Business Intelligence
/// Programmer:  Harley Boss
/// Date:        September 28th 2019
/// Description: This is the UI class that handles all events driven by the user, updating the
///              gridview and chart with data returned from the database as well as informing
///              the user of any error conditions

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A1_Yoyo {
    public partial class YoyoForm : Form {
        private Boolean databaseInitialized;
        private ParetoDiagram pareto;
        private Boolean isRunning;

        public YoyoForm() {
            InitializeComponent();
            databaseInitialized = false;
            isRunning = false;
            MessageQueueManager.Init();
            InitializeComboBoxes();
            InitializeGridView();
            pareto = new ParetoDiagram(paretoChartControl);
            pareto.InitalizeDiagram();
            txtMachineName.Text = Environment.MachineName;
            cmbProducts.Enabled = false;
        }

/*        private async void SetupPeriodicUpdates(Boolean selectAll) {
            List<Yoyo> yoyos;
            if (selectAll) {
                yoyos = await DatabaseManager.SelectAll();
            } else {
                yoyos = await DatabaseManager.Select(YoyoTypeMethods.GetYoyoFromString(cmbProducts.SelectedItem.ToString()));
            }
            // TODO: figure out how to cancel previous task
*//*            if (periodicTask != null) {
                source.Cancel();
            }*//*
            source = new CancellationTokenSource();
            cancellationToken = source.Token;
            periodicTask = PeriodicTaskFactory.Start(() => {
                pareto.UpdateDiagram(yoyos);
            }, intervalInMilliseconds: 2000, maxIterations: -1, cancelToken: cancellationToken);    
        }*/




        private void btnStart_Click(object sender, EventArgs e) {
            isRunning = !isRunning;
            if (!databaseInitialized) {
                String machineName = txtMachineName.Text;
                String userName = txtUsername.Text;
                String password = txtPassword.Text;
                DatabaseManager.Init(machineName, userName, password);
                if (!DatabaseManager.IsConnected()) {
                    lblConnectionState.Text = "Failed to establish connection to database";
                    lblConnectionState.ForeColor = Color.Red;
                    return;
                }
                lblConnectionState.Text = "Connected to database";
                lblConnectionState.ForeColor = Control.DefaultForeColor;
                databaseInitialized = true;

            }
            if (isRunning) {
                btnStart.Text = "STOP";
                btnStart.ForeColor = Color.White;
                btnStart.BackColor = Color.Red;
                MessageQueueManager.Start();
                cmbProducts.Enabled = true;

            } else {
                btnStart.Text = "START";
                btnStart.ForeColor = Control.DefaultForeColor;
                btnStart.BackColor = Control.DefaultBackColor;
                MessageQueueManager.Stop();
                lblConnectionState.Text = "Disconnected from database";
                lblConnectionState.ForeColor = Control.DefaultForeColor;
                databaseInitialized = false;
                cmbProducts.Enabled = false;
            }          
        }




        private async void cmbProducts_SelectedIndexChanged(object sender, EventArgs e) {
            List<Yoyo> yoyos = new List<Yoyo>();
            if (cmbProducts.SelectedItem.ToString() == "-- Select All --") {
                yoyos = await DatabaseManager.SelectAll();
            } else {
                YoyoType type = YoyoTypeMethods.GetYoyoFromString(cmbProducts.SelectedItem.ToString());
                yoyos = await DatabaseManager.Select(type);
            }
            CalculateNewGridViewValues(yoyos);
        }




        private void CalculateNewGridViewValues(List<Yoyo> yoyos) {
            GridViewData data = new GridViewData();

            foreach (Yoyo y in yoyos) {
                data.IncreasePartsMolded();

                // totPartsSuccMolded is Paint or higher
                if (!y.State.Contains(YoyoState.PAINT.ToString())) {
                    data.IncreasePartsSuccessfullyMolded();
                }

                // totPartsSuccPainted is Assembly or higher
                if (y.State.Contains(YoyoState.ASSEMBLY.ToString()) ||
                    y.State.Contains(YoyoState.PACKAGE.ToString())) {
                    data.IncreasePartsSuccessfullyPainted();
                }

                // totPartsSuccAssembed and totPartsPackaged are in PACKAGE state
                if (y.State.Contains(YoyoState.PACKAGE.ToString())) {
                    data.IncreasePartsSuccessfullyAssembled();
                    data.IncreaseTotalPartsPackaged();
                }
            }
            UpdateGridView(data);
            pareto.UpdateDiagram(yoyos);
        }




        private void UpdateGridView(GridViewData data) {
            dgYoyoDetails[1, 0].Value = data.GetTotalPartsMolded();
            dgYoyoDetails[1, 1].Value = data.GetTotalPartsSuccessfullyMolded();
            dgYoyoDetails[1, 2].Value = data.GetTotalPartsSuccessfullyPainted();
            dgYoyoDetails[1, 3].Value = data.GetTotalPartsSuccessfullyAssembled();
            dgYoyoDetails[1, 4].Value = data.GetTotalPartsPackaged();
            dgYoyoDetails[1, 5].Value = data.CalculateYieldAtMold() + "%";
            dgYoyoDetails[1, 6].Value = data.CalculateYieldAtAssembly() + "%";
            dgYoyoDetails[1, 7].Value = data.CalculateYieldAtPaint() + "%";
            dgYoyoDetails[1, 8].Value = data.CalculateYieldTotal() + "%";
        }




        private void InitializeComboBoxes() {
            String[] yoyoTypes = Enum.GetNames(typeof(YoyoType));
            for (int i = 0; i < yoyoTypes.Length; i++) {
                if (i == 0) {
                    cmbProducts.Items.Add("-- Select All --");
                    continue;
                }
                cmbProducts.Items.Add(YoyoTypeMethods.GetYoyoFromInt(i));
            }
        }




        private void InitializeGridView() {
            DataGridView gridView = dgYoyoDetails;
            string[] row = new string[] { "Total Parts Molded" };
            gridView.Rows.Add(row);
            row = new string[] { "Total Parts Successfully Molded" };
            gridView.Rows.Add(row);
            row = new string[] { "Total Parts Successfully Painted" };
            gridView.Rows.Add(row);
            row = new string[] { "Total Parts Successfully Assembled" };
            gridView.Rows.Add(row);
            row = new string[] { "Total Parts Packaged" };
            gridView.Rows.Add(row);
            row = new string[] { "Yield at Mold" };
            gridView.Rows.Add(row);
            row = new string[] { "Yield at Assembly" };
            gridView.Rows.Add(row);
            row = new string[] { "Yield at Paint" };
            gridView.Rows.Add(row);
            row = new string[] { "Total Yield" };
            gridView.Rows.Add(row);
        }

        private async void btnRefresh_Click(object sender, EventArgs e) {
            if (cmbProducts.SelectedItem != null) {
                List<Yoyo> yoyos;
                Boolean selectAll = cmbProducts.SelectedItem.ToString() == "-- Select All --";
                if (selectAll) {
                    yoyos = await DatabaseManager.SelectAll();
                } else {
                    yoyos = await DatabaseManager.Select(YoyoTypeMethods.GetYoyoFromString(cmbProducts.SelectedItem.ToString()));
                }
                CalculateNewGridViewValues(yoyos);
            }
        }
    }
}
