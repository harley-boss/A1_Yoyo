/// File:        ParetoDiagram.cs
/// Assigment:   Assignment #2 - Yoyo
/// Class:       Business Intelligence
/// Programmer:  Harley Boss
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

        public ParetoDiagram(ChartControl chartControl) {
            this.chartControl = chartControl;
            this.isInitialized = false;
            this.barSeriesTitle = "Occurance Count";
            this.lineSeriesTitle = "Occurance Percentage";
        }

        public void InitalizeDiagram() {
            if (isInitialized) {
                Console.WriteLine("Pareto diagam is already initialized");
                return;
            }
            isInitialized = true;
            chartControl.Titles.Add(new ChartTitle() { Text = "Rejection Statistics" });
            barSeries = new Series(barSeriesTitle, ViewType.Bar);
            lineSeries = new Series(lineSeriesTitle, ViewType.Line);
            lineSeries.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            lineSeries.View.Color = Color.Black;
            barSeries.ArgumentDataMember = "Arguments";
            barSeries.ValueDataMembers.AddRange(new string[] { "Reasons" });
            barSeries.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            ((SideBySideBarSeriesView)barSeries.View).ColorEach = true;
            ((LineSeriesView)lineSeries.View).ColorEach = true;
            chartControl.Series.Add(barSeries);
            chartControl.Series.Add(lineSeries);
            ((BarSeriesLabel)barSeries.Label).Position = BarSeriesLabelPosition.Auto;
        
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

        public void UpdateDiagram(List<Yoyo> yoyos) {
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
    }
}
