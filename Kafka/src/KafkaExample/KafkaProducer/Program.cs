using Confluent.Kafka;

namespace KafkaProducer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Kafka producer example");

            while (true)
            {
                Console.WriteLine("Enter your message:");

                var message = Console.ReadLine();

                await PublishMessage(message);
            }
            
        }

        private static async Task PublishMessage(string message)
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync("test-topic", new Message<Null, string> { Value = message });
                    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
    }
}