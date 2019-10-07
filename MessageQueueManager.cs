/// File:        MessageQueueManager.cs
/// Assigment:   Assignment #2 - Yoyo
/// Class:       Business Intelligence
/// Programmer:  Harley Boss / Spencer Billings
/// Date:        September 28th 2019
/// Description: Handles reading from the message queue and calling the appropriate method
///              from the DatabaseManager class to insert data into the database

using System;
using System.Messaging;
using System.Windows.Forms;

namespace A1_Yoyo {
    class MessageQueueManager {

        private static MessageQueue queue;
        private static Boolean isReading;
        private static String queueName;


        /// <summary>
        /// Initialize the variables of the class
        /// </summary>
        public static void Init() {
            queueName = "\\private$\\yoyo";
            queue = new MessageQueue();
            queue.Formatter = new ActiveXMessageFormatter();
            queue.MessageReadPropertyFilter.LookupId = true;
            queue.ReceiveCompleted += new ReceiveCompletedEventHandler(QueueUpdated);
        }


        /// <summary>
        /// Called to start reading from the message queue. If the manager is already reading
        /// a subsequent beginRecieve is not called
        /// </summary>
        public static void Start() {
            if (isReading) {
                Console.WriteLine("Message queue has already started");
                return;
            }
            isReading = true;
            queue.Path = "Formatname:Direct=os:" + Environment.MachineName + queueName;
            queue.BeginReceive();
        }


        /// <summary>
        /// Stops reading from the message queue
        /// </summary>
        public static void Stop() {
            if (!isReading) {
                Console.WriteLine("Message queue has already stopped");
            }
            isReading = false;
        }


        /// <summary>
        /// Handles updates from the message queue. The sender args are parsed to pull out the 
        /// data to construct a new yoyo object. That data is then inserted into the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void QueueUpdated(object sender, ReceiveCompletedEventArgs e) {
            try {
                String data = e.Message.Body.ToString();
                Yoyo yoyo = new Yoyo();
                DatabaseManager.Insert(yoyo.Build(data));
                Application.DoEvents();
                if (isReading) {
                    queue.BeginReceive();
                }
            } catch (Exception ex) {
                Console.WriteLine("Caught exception :" + ex.Message);
            }
        }
    }
}
