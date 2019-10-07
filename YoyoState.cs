/// File:        YoyoState.cs
/// Assigment:   Assignment #2 - Yoyo
/// Class:       Business Intelligence
/// Programmer:  Harley Boss / Spencer Billings
/// Date:        September 28th 2019
/// Description: An enum class to hold all of the different states a yoyo can be in

using System;

namespace A1_Yoyo {

    /// <summary>
    /// Represents all the different states a Yoyo can be in
    /// </summary>
    enum YoyoState {
        SCRAP,
        INJECTION_MOLD,
        INSPECTION_STATION_1,
        PAINT,
        INSPECTION_STATION_2,
        ASSEMBLY,
        INSPECTION_STATION_3,
        PACKAGE,
        REWORK
    }

    /// <summary>
    /// Methods for converting a yoyo state integer value to a string and vice versa
    /// </summary>
    static class YoyoStateMethods {

        public static string GetYoyoStateFromInt(int id) {
            return Enum.GetNames(typeof(YoyoType))[id];
        }

        public static YoyoState GetYoyoStateFromString(String name) {
            YoyoState type = YoyoState.SCRAP;
            String formattedName = name.Replace(" ", "_");
            int counter = 0;
            foreach (String s in Enum.GetNames(typeof(YoyoState))) {
                if (s == formattedName) {
                    type = (YoyoState)counter;
                }
                counter++;
            }
            return type;
        }
    }
}
