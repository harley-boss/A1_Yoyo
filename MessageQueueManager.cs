using System;
using System.Messaging;
using System.Windows.Forms;

namespace A1_Yoyo {
    class MessageQueueManager {

        private static MessageQueue queue;
        private static Boolean isReading;
        private static String queueName;

        public static void Init() {
            queueName = "\\private$\\yoyo";
            isReading = true;
            queue = new MessageQueue();
            queue.Formatter = new ActiveXMessageFormatter();
            queue.MessageReadPropertyFilter.LookupId = true;
            queue.ReceiveCompleted += new ReceiveCompletedEventHandler(QueueUpdated);
        }

        public static void Start() {
            if (isReading) {
                Console.WriteLine("Message queue has already started");
                return;
            }
            isReading = true;
            queue.Path = "Formatname:Direct=os:" + Environment.MachineName + queueName;
            queue.BeginReceive();
        }

        public static void Stop() {
            if (!isReading) {
                Console.WriteLine("Message queue has already stopped");
            }
            isReading = false;
        }

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
