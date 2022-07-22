using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using System;
using System.Threading.Tasks;

namespace MyLand.Services
{
    public class SNSService
    {
        private IAmazonSimpleNotificationService _snsClient;

        public SNSService()
        {
            _snsClient = new AmazonSimpleNotificationServiceClient();
        }

        public async Task<bool> AddSubscriptionAsync(string email, string topicArn)
        {
            try
            {
                //add email as the subscriber
                var emailRequest = new SubscribeRequest(topicArn, "email", email);
                var emailSubscribeResponse = await _snsClient.SubscribeAsync(emailRequest);
                var emailRequestId = emailSubscribeResponse.ResponseMetadata.RequestId;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> CreateTopic(string topicName)
        {
            try
            {
                var s = topicName.Replace(" ", "_");
                var topicRequest = new CreateTopicRequest(s);
                var topicResponse = await _snsClient.CreateTopicAsync(topicRequest);
                return topicResponse.TopicArn;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task<string> DeleteTopic(string topicName)
        {
            try
            {
                var s = topicName.Replace(" ", "_");
                var topicRequest = new DeleteTopicRequest(s);
                var topicResponse = await _snsClient.DeleteTopicAsync(topicRequest);
                return topicResponse.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
