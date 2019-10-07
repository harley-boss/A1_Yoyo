/// File:        Yoyo.cs
/// Assigment:   Assignment #2 - Yoyo
/// Class:       Business Intelligence
/// Programmer:  Harley Boss / Spencer Billings
/// Date:        September 28th 2019
/// Description: A shallow class representing a phyiscal yoyo

using System;

namespace A1_Yoyo {

    class Yoyo {
        private String workArea;        // Work area description
        private String serialNumber;    // UUID
        private String lineNumber;      // Line number
        private String state;           // Last known state of the yoyo
        private String reason;          // The reason of yoyo rejection (optional)
        private DateTime timeStamp;     // Last update of the yoyo
        private int productId;          // Id representing type of yoyo

        public Yoyo Build(String data) {
            String[] yoyoData = data.Split(',');
            WorkArea = yoyoData[0];
            SerialNumber = yoyoData[1];
            LineNumber = yoyoData[2];
            State = yoyoData[3];
            Reason = yoyoData[4];
            TimeStamp = DateTime.Parse(yoyoData[5]);
            ProductId = int.Parse(yoyoData[6]);       
            return this;
        }

        public string WorkArea { 
            get => workArea; 
            set => workArea = value; 
        }

        public string SerialNumber { 
            get => serialNumber; 
            set => serialNumber = value; 
        }

        public string LineNumber { 
            get => lineNumber; 
            set => lineNumber = value; }

        public string Reason { 
            get => reason; 
            set => reason = value; 
        }

        public DateTime TimeStamp { 
            get => timeStamp; 
            set => timeStamp = value; 
        }

        public int ProductId { 
            get => productId; 
            set => productId = value; 
        }
        public string State { 
            get => state; 
            set => state = value; 
        }
    }
}
