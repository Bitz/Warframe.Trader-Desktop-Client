namespace WFTDC.Payloads.WebsocketChat
{
    using System;
    using J = Newtonsoft.Json.JsonPropertyAttribute;

    public class ChatPayload
    {
        [J("payload")] public Payload Payload { get; set; }
        [J("type")] public string Type { get; set; }
    }

    public class Payload
    {
        [J("send_date")] public DateTimeOffset SendDate { get; set; }
        [J("message")] public string Message { get; set; }
        [J("message_from")] public string MessageFrom { get; set; }
        [J("chat_id")] public string ChatId { get; set; }
        [J("id")] public string Id { get; set; }
    }
}
