using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
    {
        public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
        {
            Console.WriteLine("AuctionCreatedFaultConsumer: " + context.Message.Message.Id);

            var exception = context.Message.Exceptions.FirstOrDefault();

            Console.WriteLine(exception.Message);

            if (exception.ExceptionType == "System.ArgumentException")
            {
                Console.WriteLine("ArgumentException");

                context.Message.Message.Model = "Bar";

                await context.Publish<AuctionCreated>(context.Message.Message);
            }
            else
            {
                Console.WriteLine("Not ArgumentException");

                await Task.CompletedTask;

            }
        }
    }
}
