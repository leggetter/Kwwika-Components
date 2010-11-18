
namespace Kwwika.QueueComponents
{
    public interface IMessageConsumer
    {
        bool ProcessMessage(object msg);
    }
}
