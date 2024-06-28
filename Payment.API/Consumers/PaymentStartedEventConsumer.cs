using MassTransit;
using Shared.PaymentEvents;
using Shared.Settings;

namespace Payment.API.Consumers
{
    public class PaymentStartedEventConsumer(ISendEndpointProvider sendEndpointProvider) : IConsumer<PaymentStartedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentStartedEvent> context)
        {
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));
            Random random = new();
            bool paymentIsSuccesfull = random.Next(2) == 0;
            if (paymentIsSuccesfull)
            {

                PaymentCompletedEvent paymentCompletedEvent = new(context.Message.CorrelationId)
                {

                };

                await sendEndpoint.Send(paymentCompletedEvent);
            }
            else
            {
                PaymentFailedEvent paymentFailedEvent = new(context.Message.CorrelationId)
                {
                    Message = "Insufficient balance...",
                    OrderItems = context.Message.OrderItems
                };

                await sendEndpoint.Send(paymentFailedEvent);
            }
        }
    }
}
