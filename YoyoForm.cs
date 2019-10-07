/// File:        YoyoForm.cs
/// Assigment:   Assignment #2 - Yoyo
/// Class:       Business Intelligence
/// Programmer:  Harley Boss / Spencer Billings
/// Date:        September 28th 2019
/// Description: This is the UI class that handles all events driven by the user, updating the
///              gridview and chart with data returned from the database as well as informing
///              the user of any error conditions

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace A1_Yoyo {

    public partial class YoyoForm : Form {
        private Boolean databaseInitialized;
        private ParetoDiagram pareto;
        private Boolean isRunning;


        /// <summary>
        /// Initializes the class variables
        /// </summary>
        public YoyoForm() {
            InitializeComponent();
            databaseInitialized = false;
            isRunning = false;
            new Thread(() =>{
                Thread.CurrentThread.IsBackground = true;
                MessageQueueManager.Init();
            }).Start();
            InitializeProductComboBox();
            InitializeGridView();
            pareto = new ParetoDiagram(paretoChartControl);
            txtMachineName.Text = Environment.MachineName;
            cmbProducts.Enabled = false;
            btnRefresh.Enabled = false;
        }


        /// <summary>
        /// Method that handles the start click event
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguements passed in on the click event</param>
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
                btnRefresh.Enabled = true;
                txtUsername.Enabled = false;
                txtPassword.Enabled = false;
            } else {
                btnStart.Text = "START";
                btnStart.ForeColor = Control.DefaultForeColor;
                btnStart.BackColor = Control.DefaultBackColor;
                MessageQueueManager.Stop();
                lblConnectionState.Text = "Disconnected from database";
                lblConnectionState.ForeColor = Control.DefaultForeColor;
                databaseInitialized = false;
                cmbProducts.Enabled = false;
                txtUsername.Enabled = true;
                txtPassword.Enabled = true;
                cmbProducts.SelectedIndex = -1;
                btnRefresh.Enabled = false;
                pareto.Destroy();
            }          
        }


        /// <summary>
        /// Method called when the selected item in the combo box is changed
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguements passed in on the click event</param>
        private async void cmbProducts_SelectedIndexChanged(object sender, EventArgs e) {
            if (cmbProducts.SelectedIndex == -1) {
                return;
            }
            List<Yoyo> yoyos = new List<Yoyo>();
            List<Yoyo> rejections = new List<Yoyo>();
            if (cmbProducts.SelectedItem.ToString() == "-- Select All --") {
                yoyos = await DatabaseManager.SelectAll();
                rejections = await DatabaseManager.SelectAllRejections();
            } else {
                YoyoType type = YoyoTypeMethods.GetYoyoFromString(cmbProducts.SelectedItem.ToString());
                yoyos = await DatabaseManager.Select(type);
                rejections = await DatabaseManager.SelectTypeRejections(type);
            }
            CalculateNewGridViewValues(yoyos);
            pareto.UpdateDiagram(rejections);
        }


        /// <summary>
        /// Method that will calculate data to be presented in the gridview
        /// </summary>
        /// <param name="yoyos">List of yoyos used to calculate yields, etc.</param>
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
        }


        /// <summary>
        /// Populates the gridview
        /// </summary>
        /// <param name="data">Gridviewdata holding the values</param>
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


        /// <summary>
        /// Populates values int the product combo box
        /// </summary>
        private void InitializeProductComboBox() {
            String[] yoyoTypes = Enum.GetNames(typeof(YoyoType));
            for (int i = 0; i < yoyoTypes.Length; i++) {
                if (i == 0) {
                    cmbProducts.Items.Add("-- Select All --");
                    continue;
                }
                cmbProducts.Items.Add(YoyoTypeMethods.GetYoyoFromInt(i));
            }
        }


        /// <summary>
        /// Initializes and populates defualt gridview data
        /// </summary>
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



        /// <summary>
        /// Handles the on click event for refreshing data
        /// </summary>
        /// <param name="sender">Object sending the event</param>
        /// <param name="e">Event arguements passed in on the click event</param>
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
