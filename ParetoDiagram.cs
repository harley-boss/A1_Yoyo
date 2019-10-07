/// File:        ParetoDiagram.cs
/// Assigment:   Assignment #2 - Yoyo
/// Class:       Business Intelligence
/// Programmer:  Harley Boss / Spencer Billings
/// Date:        September 28th 2019
/// Description: This class handles constructing and updating the pareto chart with data passed in
///              from the database. The chart shows all the reasons a yoyo can be scrapped for as
///              well as the percentage of occurance


using DevExpress.XtraCharts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace A1_Yoyo {

    class ParetoDiagram {

        private ChartControl chartControl;
        private Boolean isInitialized;
        private String barSeriesTitle;
        private String lineSeriesTitle;
        private Series barSeries;
        private Series lineSeries;


        /// <summary>
        /// Initializes the class variables
        /// </summary>
        /// <param name="chartControl"></param>
        public ParetoDiagram (ChartControl chartControl) {
            this.chartControl = chartControl;
            this.isInitialized = false;
            this.barSeriesTitle = "Occurance Count";
            this.lineSeriesTitle = "Occurance Percentage";
        }


        /// <summary>
        /// Sets up the chart once. Subsquent calls to this method are ignored.
        /// </summary>
        private void InitalizeDiagram () {
            if (isInitialized) {
                Console.WriteLine("Pareto diagam is already initialized");
                return;
            }
            isInitialized = true;
            chartControl.Titles.Add(new ChartTitle() { Text = "Rejection Statistics" });
            barSeries = new Series(barSeriesTitle, ViewType.Bar);
            barSeries.ArgumentDataMember = "Arguments";
            barSeries.ValueDataMembers.AddRange(new string[] { "Reasons" });
            ((SideBySideBarSeriesView)barSeries.View).ColorEach = true;
            ((BarSeriesLabel)barSeries.Label).Position = BarSeriesLabelPosition.Auto;

            lineSeries = new Series(lineSeriesTitle, ViewType.Line);
            lineSeries.ValueDataMembers.AddRange(new string[] { "" });
            lineSeries.View.Color = Color.Black;
            lineSeries.ValueScaleType = ScaleType.Numerical;
            ((LineSeriesView)lineSeries.View).ColorEach = true;
            ((LineSeriesView)lineSeries.View).MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
            ((LineSeriesView)lineSeries.View).LineMarkerOptions.BorderColor = Color.Black;

            chartControl.Series.AddRange(new Series[] { barSeries, lineSeries });
            SecondaryAxisY secondaryAxisY = new SecondaryAxisY("Percentage");
            ((XYDiagram)chartControl.Diagram).SecondaryAxesY.Add(secondaryAxisY);
            ((LineSeriesView)lineSeries.View).AxisY = secondaryAxisY;
            secondaryAxisY.Title.Text = "Percentage";

            ChartTitle chartXAxis = new ChartTitle();
            ChartTitle chartYAxisLeft = new ChartTitle();
            ChartTitle chartYAxisRight = new ChartTitle();

            chartXAxis.Text = "Reason for Rejection";
            chartYAxisLeft.Text = "Incident Count";
            chartYAxisRight.Text = "Percent";
            chartXAxis.Dock = ChartTitleDockStyle.Bottom;
            chartYAxisLeft.Dock = ChartTitleDockStyle.Left;
            chartYAxisRight.Dock = ChartTitleDockStyle.Right;

            chartControl.Titles.AddRange(new ChartTitle[] { chartYAxisLeft, chartXAxis, chartYAxisRight });
        }


        /// <summary>
        /// Updates the diagram, first ensuring that the chart has been initialized. 
        /// </summary>
        /// <param name="yoyos">A list of "rejected" yoyos with which to update the cahrts values</param>
        public void UpdateDiagram (List<Yoyo> yoyos) {
            if (!isInitialized) {
                Console.WriteLine("Diagram must be initialized first, initializing now...");
                InitalizeDiagram();
            }
            Dictionary<string, int> rejections = new Dictionary<string, int>();
            foreach (Yoyo y in yoyos) {
                if (y.State.Contains(YoyoState.SCRAP.ToString())
                    || y.State.Contains(YoyoState.REWORK.ToString())) {
                    String reason = y.Reason;
                    if (rejections.ContainsKey(reason)) {
                        foreach (KeyValuePair<string, int> kvp in rejections) {
                            if (kvp.Key == reason) {
                                rejections = rejections.ToDictionary(w => w.Key, w => w.Key == reason ? w.Value + 1 : w.Value);
                            }
                        }
                    } else {
                        rejections.Add(reason, 1);
                    }
                }
            }

            // Sort the dictionary
            var sortedList = from entry in rejections orderby entry.Value descending select entry;
            double cumulativeScore = 0;
            foreach (KeyValuePair<string, int> kvp in sortedList) {
                cumulativeScore += kvp.Value;
            }
            foreach (Series series in chartControl.Series) {
                series.Points.Clear();
            }
            double previousScore = 0;
            foreach (KeyValuePair<string, int> kvp in sortedList) {
                chartControl.Series[barSeriesTitle].Points.Add(new SeriesPoint(kvp.Key, kvp.Value));
                previousScore += kvp.Value;
                double newPoint = Math.Round(((previousScore / cumulativeScore) * 100), 2);
                chartControl.Series[lineSeriesTitle].Points.Add(new SeriesPoint(kvp.Key, newPoint));
            }
        }


        /// <summary>
        /// Destroys all chart resources
        /// </summary>
        public void Destroy () {
            try {
                ((XYDiagram)chartControl.Diagram).SecondaryAxesY.Clear();
                chartControl.Series.Clear();
                chartControl.Titles.Clear();
                isInitialized = false;
                chartControl.DataSource = null;
            } catch (Exception e) {
                Console.WriteLine("Caught exception destroying chart " + e.Message);
            }
        }
    }
}
