/// File:        YoyoType.cs
/// Assigment:   Assignment #2 - Yoyo
/// Class:       Business Intelligence
/// Programmer:  Harley Boss / Spencer Billings
/// Date:        September 28th 2019
/// Description: An enum class to hold all of the different product types of Yoyos

using System;

namespace A1_Yoyo {

    /// <summary>
    /// All the current different types of yoyos
    /// </summary>
    enum YoyoType {
        Unnamed,
        Original_Sleeper,
        Black_Beauty,
        Firecracker,
        Lemon_Yellow,
        Midnight_Blue,
        Screaming_Orange,
        Gold_Glitter,
        White_Lightning
    }


    /// <summary>
    /// Methods for converting a yoyo type integer value to a string and vice versa
    /// </summary>
    static class YoyoTypeMethods {

        public static string GetYoyoFromInt(int id) {
            String productName = Enum.GetNames(typeof(YoyoType))[id];
            return productName.Replace("_", " ");
        }

        public static YoyoType GetYoyoFromString(String name) {
            YoyoType type = YoyoType.Unnamed;
            String formattedName = name.Replace(" ", "_");
            int counter = 0;
            foreach (String s in Enum.GetNames(typeof(YoyoType))) {
                if (s == formattedName) {
                    type = (YoyoType)counter;
                }
                counter++;
            }
            return type;
        }
    }
}

