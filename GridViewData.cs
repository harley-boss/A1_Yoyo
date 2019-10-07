/// File:        GridViewData.cs
/// Assigment:   Assignment #2 - Yoyo
/// Class:       Business Intelligence
/// Programmer:  Harley Boss / Spencer Billings
/// Date:        September 28th 2019
/// Description: A shallow class to represent that data with which to fill in the gridview

using System;

namespace A1_Yoyo {

    class GridViewData {

        double totPartsMolded = 0;
        double totPartsSuccPainted = 0;
        double totPartsSuccMolded = 0;
        double totPartsSuccAssembled = 0;
        double totPartsPackaged = 0;

        public void IncreasePartsMolded() {
            totPartsMolded++;
        }

        public void IncreasePartsSuccessfullyPainted() {
            totPartsSuccPainted++;
        }

        public void IncreasePartsSuccessfullyMolded() {
            totPartsSuccMolded++;
        }

        public void IncreasePartsSuccessfullyAssembled() {
            totPartsSuccAssembled++;
        }

        public void IncreaseTotalPartsPackaged() {
            totPartsPackaged++;
        }

        public double GetTotalPartsMolded() {
            return totPartsMolded;
        }

        public double GetTotalPartsSuccessfullyMolded() {
            return totPartsSuccMolded;
        }

        public double GetTotalPartsSuccessfullyPainted() {
            return totPartsSuccPainted;
        }

        public double GetTotalPartsSuccessfullyAssembled() {
            return totPartsSuccAssembled;
        }

        public double GetTotalPartsPackaged() {
            return totPartsPackaged;
        }

        public double CalculateYieldAtMold() {
            if (totPartsMolded == 0) {
                return 0;
            }
            return Math.Round(((totPartsSuccMolded / totPartsMolded) * 100), 2);
        }

        public double CalculateYieldAtAssembly() {
            if (totPartsSuccPainted == 0) {
                return 0;
            }
            return Math.Round(((totPartsSuccAssembled / totPartsSuccPainted) * 100), 2);
        }

        public double CalculateYieldAtPaint() {
            if (totPartsSuccMolded == 0) {
                return 0;
            }
            return Math.Round(((totPartsSuccPainted / totPartsSuccMolded) * 100), 2);
        }

        public double CalculateYieldTotal() {
            if (totPartsMolded == 0) {
                return 0;
            }
            return Math.Round(((totPartsPackaged / totPartsMolded) * 100), 2);
        }
    }
}
