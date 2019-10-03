using System;

namespace A1_Yoyo {
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

